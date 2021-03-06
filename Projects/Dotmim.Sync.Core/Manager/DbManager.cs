﻿using DmBinaryFormatter;
using Dotmim.Sync.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Text;

namespace Dotmim.Sync.Core.Manager
{
    public abstract class DbManager : IDisposable
    {

        public string TableName { get; }

        public DbManager(string tableName)
        {
            this.TableName = tableName;
        }

        /// <summary>
        /// Gets a table manager, who can execute somes queries directly on source database
        /// </summary>
        public abstract IDbManagerTable CreateManagerTable(DbConnection connection, DbTransaction transaction = null);
        
        public IDbManagerTable GetManagerTable(DbConnection connection, DbTransaction transaction = null)
        {
            var mgerTable = CreateManagerTable(connection, transaction);
            mgerTable.TableName = this.TableName;
            return mgerTable;
        }

        /// <summary>
        /// Get a parameter even if it's a @param or :param or param
        /// </summary>
        public static DbParameter GetParameter(DbCommand command, string parameterName)
        {
            if (command == null)
                return null;

            if (command.Parameters.Contains(string.Concat("@", parameterName)))
                return command.Parameters[string.Concat("@", parameterName)];

            if (command.Parameters.Contains(string.Concat(":", parameterName)))
                return command.Parameters[string.Concat(":", parameterName)];

            if (!command.Parameters.Contains(parameterName))
                return null;

            return command.Parameters[parameterName];
        }

        /// <summary>
        /// Set a parameter value
        /// </summary>
        public static void SetParameterValue(DbCommand command, string parameterName, object value)
        {
            DbParameter parameter = GetParameter(command, parameterName);
            if (parameter != null)
                if (value == null)
                    parameter.Value = DBNull.Value;
                else
                    parameter.Value = value;
        }


        public static int GetSyncIntOutParameter(string parameter, DbCommand command)
        {
            DbParameter dbParameter = GetParameter(command, parameter);
            if (dbParameter == null || dbParameter.Value == null || string.IsNullOrEmpty(dbParameter.Value.ToString()))
                return 0;

            return int.Parse(dbParameter.Value.ToString(), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Parse a time stamp value
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static long ParseTimestamp(object obj)
        {
            long timestamp = 0;

            if (obj == DBNull.Value)
                return 0;

            if (obj is long || obj is int || obj is ulong || obj is uint || obj is decimal)
                return Convert.ToInt64(obj, NumberFormatInfo.InvariantInfo);

            string str = obj as string;
            if (str != null)
            {
                long.TryParse(str, NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out timestamp);
                return timestamp;
            }

            byte[] numArray = obj as byte[];
            if (numArray == null)
                return 0;

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < numArray.Length; i++)
            {
                string str1 = numArray[i].ToString("X", NumberFormatInfo.InvariantInfo);
                stringBuilder.Append((str1.Length == 1 ? string.Concat("0", str1) : str1));
            }

            long.TryParse(stringBuilder.ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out timestamp);
            return timestamp;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {

                   
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

     

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
