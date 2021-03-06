﻿using Dotmim.Sync.Core.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Dotmim.Sync.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Dotmim.Sync.Core.Common;
using System.Linq;
using Dotmim.Sync.Core.Log;
using System.Data;

namespace Dotmim.Sync.SqlServer.Builders
{
    public class SqlBuilderTrackingTable : IDbBuilderTrackingTableHelper
    {
        private ObjectNameParser tableName;
        private ObjectNameParser trackingName;
        private DmTable tableDescription;
        private SqlConnection connection;
        private SqlTransaction transaction;
        public List<DmColumn> FilterColumns { get; set; }

        public DmTable TableDescription
        {
            get
            {
                return this.tableDescription;
            }
            set
            {
                this.tableDescription = value;
                (this.tableName, this.trackingName) = SqlBuilder.GetParsers(TableDescription);

            }
        }


        public SqlBuilderTrackingTable(DbConnection connection, DbTransaction transaction = null)
        {
            this.connection = connection as SqlConnection;
            this.transaction = transaction as SqlTransaction;
        }


        public void CreateIndex()
        {
            bool alreadyOpened = this.connection.State == ConnectionState.Open;

            try
            {
                using (var command = new SqlCommand())
                {
                    if (!alreadyOpened)
                        this.connection.Open();

                    if (this.transaction != null)
                        command.Transaction = this.transaction;

                    command.CommandText = this.CreateIndexCommandText();
                    command.Connection = this.connection;
                    command.ExecuteNonQuery();

                }
            }
            catch (Exception ex)
            {
                Logger.Current.Error($"Error during CreateIndex : {ex}");
                throw;

            }
            finally
            {
                if (!alreadyOpened && this.connection.State != ConnectionState.Closed)
                    this.connection.Close();

            }

        }
        private string CreateIndexCommandText()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"CREATE NONCLUSTERED INDEX [{trackingName.UnquotedStringWithUnderScore}_timestamp_index] ON {trackingName.QuotedString} (");
            stringBuilder.AppendLine($"\t[update_timestamp] ASC");
            stringBuilder.AppendLine($"\t,[update_scope_id] ASC");
            stringBuilder.AppendLine($"\t,[sync_row_is_tombstone] ASC");
            // Filter columns
            if (this.FilterColumns != null)
            {
                for (int i = 0; i < this.FilterColumns.Count; i++)
                {
                    var filterColumn = this.FilterColumns[i];

                    if (TableDescription.PrimaryKey.Columns.Any(c => c.ColumnName == filterColumn.ColumnName))
                        continue;

                    ObjectNameParser columnName = new ObjectNameParser(filterColumn.ColumnName);
                    stringBuilder.AppendLine($"\t,{columnName.QuotedString} ASC");
                }
            }

            foreach (var pkColumn in TableDescription.PrimaryKey.Columns)
            {
                ObjectNameParser columnName = new ObjectNameParser(pkColumn.ColumnName);
                stringBuilder.AppendLine($"\t,{columnName.QuotedString} ASC");
            }
            stringBuilder.Append(")");
            return stringBuilder.ToString();
        }

        public string CreateIndexScriptText()
        {
            string str = string.Concat("Create index on Tracking Table ", trackingName.QuotedString);
            return SqlBuilder.WrapScriptTextWithComments(this.CreateIndexCommandText(), str);
        }

        public void CreatePk()
        {
            bool alreadyOpened = this.connection.State == ConnectionState.Open;

            try
            {
                using (var command = new SqlCommand())
                {
                    if (!alreadyOpened)
                        this.connection.Open();

                    if (transaction != null)
                        command.Transaction = transaction;

                    command.CommandText = this.CreatePkCommandText();
                    command.Connection = this.connection;
                    command.ExecuteNonQuery();

                }
            }
            catch (Exception ex)
            {
                Logger.Current.Error($"Error during CreateIndex : {ex}");
                throw;

            }
            finally
            {
                if (!alreadyOpened && this.connection.State != ConnectionState.Closed)
                    this.connection.Close();

            }

        }
        public string CreatePkScriptText()
        {
            string str = string.Concat("Create Primary Key on Tracking Table ", trackingName.QuotedString);
            return SqlBuilder.WrapScriptTextWithComments(this.CreatePkCommandText(), str);
        }

        public string CreatePkCommandText()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"ALTER TABLE {trackingName.QuotedString} ADD CONSTRAINT [PK_{trackingName.UnquotedStringWithUnderScore}] PRIMARY KEY (");

            for (int i = 0; i < TableDescription.PrimaryKey.Columns.Length; i++)
            {
                DmColumn pkColumn = TableDescription.PrimaryKey.Columns[i];
                var quotedColumnName = new ObjectNameParser(pkColumn.ColumnName, "[", "]").QuotedString;

                stringBuilder.Append(quotedColumnName);

                if (i < TableDescription.PrimaryKey.Columns.Length - 1)
                    stringBuilder.Append(", ");
            }
            stringBuilder.Append(")");

            return stringBuilder.ToString();
        }

        public void CreateTable()
        {
            bool alreadyOpened = this.connection.State == ConnectionState.Open;

            try
            {
                using (var command = new SqlCommand())
                {
                    if (!alreadyOpened)
                        this.connection.Open();

                    if (this.transaction != null)
                        command.Transaction = this.transaction;

                    command.CommandText = this.CreateTableCommandText();
                    command.Connection = this.connection;
                    command.ExecuteNonQuery();

                }
            }
            catch (Exception ex)
            {
                Logger.Current.Error($"Error during CreateIndex : {ex}");
                throw;

            }
            finally
            {
                if (!alreadyOpened && this.connection.State != ConnectionState.Closed)
                    this.connection.Close();

            }


        }

        public string CreateTableScriptText()
        {
            string str = string.Concat("Create Tracking Table ", trackingName.QuotedString);
            return SqlBuilder.WrapScriptTextWithComments(this.CreateTableCommandText(), str);
        }

        public string CreateTableCommandText()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"CREATE TABLE {trackingName.QuotedString} (");

            // Adding the primary key
            foreach (DmColumn pkColumn in TableDescription.PrimaryKey.Columns)
            {
                var quotedColumnName = new ObjectNameParser(pkColumn.ColumnName, "[", "]").QuotedString;
                var quotedColumnType = new ObjectNameParser(pkColumn.GetSqlDbTypeString(), "[", "]").QuotedString;
                quotedColumnType += pkColumn.GetSqlTypePrecisionString();
                var nullableColumn = pkColumn.AllowDBNull ? "NULL" : "NOT NULL";

                stringBuilder.AppendLine($"{quotedColumnName} {quotedColumnType} {nullableColumn}, ");
            }

            // adding the tracking columns
            stringBuilder.AppendLine($"[create_scope_id] [uniqueidentifier] NULL, ");
            stringBuilder.AppendLine($"[update_scope_id] [uniqueidentifier] NULL, ");
            stringBuilder.AppendLine($"[create_timestamp] [bigint] NULL, ");
            stringBuilder.AppendLine($"[update_timestamp] [bigint] NULL, ");
            stringBuilder.AppendLine($"[timestamp] [timestamp] NULL, ");
            stringBuilder.AppendLine($"[sync_row_is_tombstone] [bit] NOT NULL default(0), ");
            stringBuilder.AppendLine($"[last_change_datetime] [datetime] NULL, ");

            // adding the filter columns
            if (this.FilterColumns != null)
                foreach (DmColumn filterColumn in this.FilterColumns)
                {
                    var isPk = TableDescription.PrimaryKey.Columns.Any(dm => TableDescription.IsEqual(dm.ColumnName, filterColumn.ColumnName));
                    if (isPk)
                        continue;

                    var quotedColumnName = new ObjectNameParser(filterColumn.ColumnName, "[", "]").QuotedString;
                    var quotedColumnType = new ObjectNameParser(filterColumn.GetSqlDbTypeString(), "[", "]").QuotedString;
                    quotedColumnType += filterColumn.GetSqlTypePrecisionString();
                    var nullableColumn = filterColumn.AllowDBNull ? "NULL" : "NOT NULL";

                    stringBuilder.AppendLine($"{quotedColumnName} {quotedColumnType} {nullableColumn}, ");
                }
            stringBuilder.Append(")");
            return stringBuilder.ToString();
        }

        public bool NeedToCreateTrackingTable(DbBuilderOption builderOption)
        {

            if (builderOption.HasFlag(DbBuilderOption.CreateOrUseExistingSchema))
                return !SqlManagementUtils.TableExists(connection, transaction, trackingName.QuotedString);

            return false;
        }

        public void PopulateFromBaseTable()
        {
            bool alreadyOpened = this.connection.State == ConnectionState.Open;

            try
            {
                using (var command = new SqlCommand())
                {
                    if (!alreadyOpened)
                        this.connection.Open();

                    if (this.transaction != null)
                        command.Transaction = this.transaction;

                    command.CommandText = this.CreatePopulateFromBaseTableCommandText();
                    command.Connection = this.connection;
                    command.ExecuteNonQuery();

                }
            }
            catch (Exception ex)
            {
                Logger.Current.Error($"Error during CreateIndex : {ex}");
                throw;

            }
            finally
            {
                if (!alreadyOpened && this.connection.State != ConnectionState.Closed)
                    this.connection.Close();

            }

        }

        private string CreatePopulateFromBaseTableCommandText()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(string.Concat("INSERT INTO ", trackingName.QuotedString, " ("));
            StringBuilder stringBuilder1 = new StringBuilder();
            StringBuilder stringBuilder2 = new StringBuilder();
            string empty = string.Empty;
            StringBuilder stringBuilderOnClause = new StringBuilder("ON ");
            StringBuilder stringBuilderWhereClause = new StringBuilder("WHERE ");
            string str = string.Empty;
            string baseTable = "[base]";
            string sideTable = "[side]";
            foreach (var pkColumn in TableDescription.PrimaryKey.Columns)
            {
                var quotedColumnName = new ObjectNameParser(pkColumn.ColumnName, "[", "]").QuotedString;

                stringBuilder1.Append(string.Concat(empty, quotedColumnName));

                stringBuilder2.Append(string.Concat(empty, baseTable, ".", quotedColumnName));

                string[] quotedName = new string[] { str, baseTable, ".", quotedColumnName, " = ", sideTable, ".", quotedColumnName };
                stringBuilderOnClause.Append(string.Concat(quotedName));
                string[] strArrays = new string[] { str, sideTable, ".", quotedColumnName, " IS NULL" };
                stringBuilderWhereClause.Append(string.Concat(strArrays));
                empty = ", ";
                str = " AND ";
            }
            StringBuilder stringBuilder5 = new StringBuilder();
            StringBuilder stringBuilder6 = new StringBuilder();

            if (FilterColumns != null)
                foreach (var filterColumn in this.FilterColumns)
                {
                    var isPk = TableDescription.PrimaryKey.Columns.Any(dm => TableDescription.IsEqual(dm.ColumnName, filterColumn.ColumnName));
                    if (isPk)
                        continue;

                    var quotedColumnName = new ObjectNameParser(filterColumn.ColumnName, "[", "]").QuotedString;

                    stringBuilder6.Append(string.Concat(empty, quotedColumnName));
                    stringBuilder5.Append(string.Concat(empty, baseTable, ".", quotedColumnName));
                }

            // (list of pkeys)
            stringBuilder.Append(string.Concat(stringBuilder1.ToString(), ", "));

            stringBuilder.Append("[create_scope_id], ");
            stringBuilder.Append("[update_scope_id], ");
            stringBuilder.Append("[create_timestamp], ");
            stringBuilder.Append("[update_timestamp], ");
            //stringBuilder.Append("[timestamp], "); // timestamp is not a column we update, it's auto
            stringBuilder.Append("[sync_row_is_tombstone] ");
            stringBuilder.AppendLine(string.Concat(stringBuilder6.ToString(), ") "));
            stringBuilder.Append(string.Concat("SELECT ", stringBuilder2.ToString(), ", "));
            stringBuilder.Append("NULL, ");
            stringBuilder.Append("NULL, ");
            stringBuilder.Append("@@DBTS+1, ");
            stringBuilder.Append("0, ");
            //stringBuilder.Append("@@DBTS+1, "); // timestamp is not a column we update, it's auto
            stringBuilder.Append("0");
            stringBuilder.AppendLine(string.Concat(stringBuilder5.ToString(), " "));
            string[] localName = new string[] { "FROM ", tableName.QuotedString, " ", baseTable, " LEFT OUTER JOIN ", trackingName.QuotedString, " ", sideTable, " " };
            stringBuilder.AppendLine(string.Concat(localName));
            stringBuilder.AppendLine(string.Concat(stringBuilderOnClause.ToString(), " "));
            stringBuilder.AppendLine(string.Concat(stringBuilderWhereClause.ToString(), "; \n"));
            return stringBuilder.ToString();
        }

        public string CreatePopulateFromBaseTableScriptText()
        {
            string str = string.Concat("Populate tracking table ", trackingName.QuotedString, " for existing data in table ", tableName.QuotedString);
            return SqlBuilder.WrapScriptTextWithComments(this.CreatePopulateFromBaseTableCommandText(), str);
        }

        public void PopulateNewFilterColumnFromBaseTable(DmColumn filterColumn)
        {
            throw new NotImplementedException();
        }

        public string ScriptPopulateNewFilterColumnFromBaseTable(DmColumn filterColumn)
        {
            throw new NotImplementedException();
        }

        public void AddFilterColumn(DmColumn filterColumn)
        {
            bool alreadyOpened = this.connection.State == ConnectionState.Open;

            try
            {
                using (var command = new SqlCommand())
                {
                    if (!alreadyOpened)
                        this.connection.Open();

                    if (this.transaction != null)
                        command.Transaction = this.transaction;

                    command.CommandText = this.AddFilterColumnCommandText(filterColumn);
                    command.Connection = this.connection;
                    command.ExecuteNonQuery();

                }
            }
            catch (Exception ex)
            {
                Logger.Current.Error($"Error during CreateIndex : {ex}");
                throw;

            }
            finally
            {
                if (!alreadyOpened && this.connection.State != ConnectionState.Closed)
                    this.connection.Close();

            }

        }

        private string AddFilterColumnCommandText(DmColumn col)
        {
            var quotedColumnName = new ObjectNameParser(col.ColumnName, "[", "]").QuotedString;
            var quotedColumnType = new ObjectNameParser(col.GetSqlDbTypeString(), "[", "]").QuotedString;
            quotedColumnType += col.GetSqlTypePrecisionString();

            return string.Concat("ALTER TABLE ", quotedColumnName, " ADD ", quotedColumnType);
        }
        public string ScriptAddFilterColumn(DmColumn filterColumn)
        {
            var quotedColumnName = new ObjectNameParser(filterColumn.ColumnName, "[", "]");

            string str = string.Concat("Add new filter column, ", quotedColumnName.UnquotedString, ", to Tracking Table ", trackingName.QuotedString);
            return SqlBuilder.WrapScriptTextWithComments(this.AddFilterColumnCommandText(filterColumn), str);
        }


    }
}
