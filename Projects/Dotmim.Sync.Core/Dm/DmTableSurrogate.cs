﻿using DmBinaryFormatter;
using Dotmim.Sync.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Dotmim.Sync.Data.Surrogate
{
    /// <summary>
    /// Represents a surrogate of a DmTable object, which DotMim Sync uses during custom binary serialization.
    /// </summary>
    [Serializable]
    public class DmTableSurrogate : IDisposable
    {

        /// <summary>
        /// Gets or sets the locale information used to compare strings within the table.
        /// </summary>
        public String CultureInfoName { get; set; }

        /// <summary>Gets or sets the Case sensitive rul of the DmTable that the DmTableSurrogate object represents.</summary>
        public Boolean CaseSensitive { get; set; }

        /// <summary>
        /// Get or Set the prefix used for the DmTableSurrogate
        /// </summary>
        public String Prefix { get; set; }

        /// <summary>
        /// Gets or sets an array that represents the state of each row in the table.
        /// </summary>
        public int[] RowStates { get; set; }

        /// <summary>
        /// Gets or sets the name of the table that the DmTableSurrogate object represents.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Gets an array of DmColumnSurrogate objects that comprise the table that is represented by the DmTableSurrogate object.
        /// </summary>
        public List<DmColumnSurrogate> Columns { get; set; } = new List<DmColumnSurrogate>();

        /// <summary>
        /// Gets an array of DmColumnSurrogate objects that represent the PrimaryKeys.
        /// </summary>
        public List<string> PrimaryKeys { get; set; } = new List<string>();

        /// <summary>Gets an array of objects that represent the columns and rows of dm in the <see cref="T:Microsoft.Synchronization.Dm.DmTableSurrogate" /> object.</summary>
        public Dictionary<int, List<object>> Records { get; set; }


        public long GetEmptyBytesLength()
        {
            long bytesLength = String.IsNullOrEmpty(CultureInfoName) ? 1L : Encoding.UTF8.GetBytes(CultureInfoName).Length;
            bytesLength += 1L; // CasSensitive
            bytesLength += String.IsNullOrEmpty(Prefix) ? 1L : Encoding.UTF8.GetBytes(Prefix).Length;
            bytesLength += String.IsNullOrEmpty(TableName) ? 1L : Encoding.UTF8.GetBytes(TableName).Length;

            // TODO : Potentially error in bytes length calcul
            bytesLength += Encoding.UTF8.GetBytes(this.GetType().GetAssemblyQualifiedName()).Length; // Type

            return bytesLength;

        }

        /// <summary>
        /// Only used for Serialization
        /// </summary>
        public DmTableSurrogate()
        {

        }

        /// <summary>
        /// Initializes a new instance of the DmTableSurrogate class.
        /// </summary>
        public DmTableSurrogate(DmTable dt)
        {
            if (dt == null)
                throw new ArgumentNullException("dt", "DmTable");

            this.TableName = dt.TableName;
            this.CultureInfoName = dt.Culture.Name;
            this.CaseSensitive = dt.CaseSensitive;
            this.Prefix = dt.Prefix;

            for (int i = 0; i < dt.Columns.Count; i++)
                this.Columns.Add(new DmColumnSurrogate(dt.Columns[i]));

            // Primary Keys
            if (dt.PrimaryKey != null && dt.PrimaryKey.Columns != null && dt.PrimaryKey.Columns.Length > 0)
            {
                for (int i = 0; i < dt.PrimaryKey.Columns.Length; i++)
                    this.PrimaryKeys.Add(dt.PrimaryKey.Columns[i].ColumnName);
            }

            // Fill the rows
            if (dt.Rows.Count <= 0)
                return;

            // the BitArray contains bit values initialized to false. We will use it to store row state
            this.RowStates = new int[dt.Rows.Count];

            // Records in a straightforward object array
            this.Records = new Dictionary<int, List<object>>(dt.Columns.Count);

            for (int j = 0; j < dt.Columns.Count; j++)
                this.Records[j] = new List<object>(dt.Rows.Count);

            for (int k = 0; k < dt.Rows.Count; k++)
            {
                this.RowStates[k] = (int)dt.Rows[k].RowState;
                this.ConvertToSurrogateRecords(dt.Rows[k]);
            }

        }

        /// <summary>
        /// Copies the table schema from a DmTableSurrogate object into a DmTable object.
        /// </summary>
        public void ReadSchemaIntoDmTable(DmTable dt)
        {
            if (dt == null)
                throw new ArgumentNullException("dt", "DmTable");

            dt.TableName = this.TableName;
            dt.Culture = new CultureInfo(this.CultureInfoName);
            dt.Prefix = this.Prefix;
            dt.CaseSensitive = this.CaseSensitive;

            for (int i = 0; i < this.Columns.Count; i++)
            {
                DmColumn dmColumn = this.Columns[i].ConvertToDmColumn();
                dt.Columns.Add(dmColumn);
            }

            if (this.PrimaryKeys != null && this.PrimaryKeys.Count > 0)
            {
                DmColumn[] keyColumns = new DmColumn[this.PrimaryKeys.Count];

                for (int i = 0; i < this.PrimaryKeys.Count; i++)
                {
                    string columnName = this.PrimaryKeys[i];
                    keyColumns[i] = dt.Columns.First(c => dt.IsEqual(c.ColumnName, columnName));
                }

                DmKey key = new DmKey(keyColumns);

                dt.PrimaryKey = key;
            }
        }


        /// <summary>
        /// Copies the table schema from a DmTableSurrogate object into a DmTable object.
        /// </summary>
        public void ReadDatasIntoDmTable(DmTable dt)
        {
            if (this.Records != null && dt != null && dt.Columns.Count > 0)
            {
                int length = Records[0].Count;
                for (int i = 0; i < length; i++)
                    this.ConvertToDmRow(dt, i);
            }
        }

        private DmRow ConvertToDmRow(DmTable dt, int bitIndex)
        {
            DmRowState rowState = (DmRowState)this.RowStates[bitIndex];
            return this.ConstructRow(dt, rowState, bitIndex);
        }

        /// <summary>
        /// Construct a row from a dmTable, a rowState and the bitIndex
        /// </summary>
        private DmRow ConstructRow(DmTable dt, DmRowState rowState, int bitIndex)
        {
            DmRow dmRow = dt.NewRow();
            int count = dt.Columns.Count;

            dmRow.BeginEdit();

            for (int i = 0; i < count; i++)
            {
                object dmRowObject = this.Records[i][bitIndex];

                // Sometimes, a serializer could potentially serialize type into string
                // For example JSON.Net will serialize GUID into STRING
                // So we try to deserialize in correct type
                if (dmRowObject != null)
                {
                    var columnType = dt.Columns[i].DataType;

                    if (columnType == typeof(Guid) && (dmRowObject as string) != null)
                    {
                        dmRowObject = new Guid(dmRowObject.ToString());
                    }
                    else if (columnType == typeof(Int32) && dmRowObject.GetType() != typeof(Int32))
                    {
                        dmRowObject = Convert.ToInt32(dmRowObject);
                    }
                    else if (columnType == typeof(Int16) && dmRowObject.GetType() != typeof(Int16))
                    {
                        dmRowObject = Convert.ToInt16(dmRowObject);
                    }
                    else if (dmRowObject.GetType() != columnType)
                    {
                        Debug.WriteLine($"Can't convert serialized value {dmRowObject.ToString()} to {columnType}");

                        var t = dmRowObject.GetType();
                        var converter = columnType.GetConverter();
                        if (converter.CanConvertFrom(t))
                            dmRowObject = converter.ConvertFrom(dmRowObject);
                        else
                            dmRowObject = Convert.ChangeType(dmRowObject, columnType, CultureInfo.InvariantCulture);
                    }
                }

                if (rowState == DmRowState.Deleted)
                {
                    // Since some columns might be not null (and we have null because the row is deleted)
                    if (this.Records[i][bitIndex] != null)
                        dmRow[i] = dmRowObject;
                }
                else
                {
                    dmRow[i] = dmRowObject;
                }
            }

            //if (rowState == DmRowState.Deleted)
            //{
            //    // we are in a deleted state, so we have only primary keys available
            //    for (int i = 0; i < count; i++)
            //    {
            //        // since some columns might be not null (and we have null because the row is deleted)
            //        if (this.Records[i][bitIndex] != null)
            //            dmRow[i] = this.Records[i][bitIndex];
            //    }

            //}
            //else
            //{
            //    for (int i = 0; i < count; i++)
            //        dmRow[i] = this.Records[i][bitIndex];

            //}

            dt.Rows.Add(dmRow);

            switch (rowState)
            {
                case DmRowState.Unchanged:
                    {
                        dmRow.AcceptChanges();
                        dmRow.EndEdit();
                        return dmRow;
                    }
                case DmRowState.Added:
                    {
                        dmRow.EndEdit();
                        return dmRow;
                    }
                case DmRowState.Deleted:
                    {
                        dmRow.AcceptChanges();
                        dmRow.Delete();
                        dmRow.EndEdit();
                        return dmRow;

                    }
                case DmRowState.Modified:
                    {
                        dmRow.AcceptChanges();
                        dmRow.SetModified();
                        dmRow.EndEdit();
                        return dmRow;
                    }
                default:
                    throw new ArgumentException("InvalidRowState");
            }
        }

        private void ConvertToSurrogateRecords(DmRow row)
        {
            int count = row.Table.Columns.Count;
            DmRowState rowState = row.RowState;
            DmRowVersion rowVersion = rowState == DmRowState.Deleted ? DmRowVersion.Original : DmRowVersion.Current;

            for (int i = 0; i < count; i++)
                this.Records[i].Add(row[i, rowVersion]);
        }


        /// <summary>
        /// Get a row size
        /// </summary>
        public static long GetRowSizeFromDataRow(DmRow row)
        {
            bool isRowDeleted = false;

            if (row.RowState == DmRowState.Deleted)
            {
                row.RejectChanges();
                isRowDeleted = true;
            }

            long byteCount = 0;
            object[] itemArray = row.ItemArray;

            for (int i = 0; i < itemArray.Length; i++)
            {
                // Size for the value
                object obj = itemArray[i];

                if (obj == null)
                    byteCount = byteCount + 5;
                else if (obj is DBNull)
                    byteCount = byteCount + 5;
                else if (obj.GetType() == typeof(string))
                    byteCount = byteCount + Encoding.UTF8.GetByteCount((string)obj);
                else if (obj.GetType() == typeof(byte[]))
                    byteCount = byteCount + ((byte[])obj).Length;
                else
                    byteCount = byteCount + GetSizeForType(obj.GetType());

                // Size for the type
                var typeofobject = row.Table.Columns[i].DataType;
                var byteslengthtype = Encoding.UTF8.GetBytes(DmUtils.GetAssemblyQualifiedName(typeofobject)).Length;
                byteCount += byteslengthtype;

                // State
                byteCount += 4L;

                // Index
                byteCount += 4L;



            }
            if (isRowDeleted)
            {
                row.Delete();
            }
            return byteCount;
        }


        /// <summary>
        /// Gets a size for a given type
        /// </summary>
        public static long GetSizeForType(Type type)
        {

            if (type == typeof(object) || type == typeof(long) || type == typeof(ulong) ||
                type == typeof(double) || type == typeof(DateTime))
                return 8L;

            if (type == typeof(DBNull))
                return 0L;

            if (type == typeof(bool) || type == typeof(sbyte) || type == typeof(byte))
                return 1L;

            if (type == typeof(char) || type == typeof(short) || type == typeof(ushort))
                return 2L;

            if (type == typeof(int) || type == typeof(uint) || type == typeof(float))
                return 4L;

            if (type == typeof(decimal) || type == typeof(Guid))
                return 16L;

            return 0L;

        }
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool cleanup)
        {
            this.Clear();
        }

        public void Clear()
        {
            if (this.Records != null)
            {
                foreach (var d in this.Records)
                    d.Value.Clear();

                this.Records.Clear();
                this.Records = null;
            }
            if (this.PrimaryKeys != null)
            {
                this.PrimaryKeys.Clear();
                this.PrimaryKeys = null;
            }

            if (this.Columns != null)
            {
                this.Columns.Clear();
                this.Columns = null;
            }
        }




    }
}