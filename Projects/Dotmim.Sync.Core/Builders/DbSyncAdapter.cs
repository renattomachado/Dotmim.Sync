﻿using Dotmim.Sync.Core.Context;
using Dotmim.Sync.Core.Enumerations;
using Dotmim.Sync.Core.Log;
using Dotmim.Sync.Core.Manager;
using Dotmim.Sync.Core.Scope;
using Dotmim.Sync.Data;
using Dotmim.Sync.Enumerations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dotmim.Sync.Core.Builders
{

    /// <summary>
    /// The SyncAdapter is the datasource manager for ONE table
    /// Should be implemented by every database provider and provide every SQL action
    /// </summary>
    public abstract class DbSyncAdapter
    {
        public delegate ApplyAction ConflictActionDelegate(SyncConflict conflict, DbConnection connection, DbTransaction transaction = null);

        public ConflictActionDelegate ConflictActionInvoker = null;

        /// <summary>
        /// Gets the table description, a dmTable with no rows
        /// </summary>
        public abstract DmTable TableDescription { get; set; }

        /// <summary>
        /// Gets or Sets the collection of triggers / procstock names
        /// </summary>
        public DbObjectNames ObjectNames { get; set; }

        /// <summary>
        /// Get or Set the current step (could be only Added, Modified, Deleted)
        /// </summary>
        internal DmRowState applyType { get; set; }

        /// <summary>
        /// Get if the error is a primarykey exception
        /// </summary>
        public abstract bool IsPrimaryKeyViolation(Exception Error);

        /// <summary>
        /// Get a command and set parameters
        /// </summary>
        public abstract void SetCommandSessionParameters(DbCommand command);

        /// <summary>
        /// Execute a batch command
        /// </summary>
        public abstract void ExecuteBatchCommand(DbCommand cmd, DmTable applyTable, DmTable failedRows, ScopeInfo scope);


        public abstract DbConnection Connection { get; }

        public abstract DbTransaction Transaction { get; }
        /// <summary>
        /// Create a Sync Adapter
        /// </summary>
        public DbSyncAdapter()
        {
        }


        /// <summary>
        /// Set command parameters value mapped to Row
        /// </summary>
        internal void SetColumnParameters(DbCommand command, DmRow row)
        {
            foreach (DbParameter parameter in command.Parameters)
            {
                // foreach parameter, check if we have a column 
                if (!string.IsNullOrEmpty(parameter.SourceColumn))
                {
                    if (row.Table.Columns.Contains(parameter.SourceColumn))
                    {
                        object value = null;
                        if (row.RowState == DmRowState.Deleted)
                            value = row[parameter.SourceColumn, DmRowVersion.Original];
                        else
                            value = row[parameter.SourceColumn];

                        DbManager.SetParameterValue(command, parameter.ParameterName, value);
                    }
                }

            }

            // return value
            var syncRowCountParam = DbManager.GetParameter(command, "sync_row_count");

            if (syncRowCountParam != null)
                syncRowCountParam.Direction = ParameterDirection.Output;
        }

        /// <summary>
        /// Insert or update a metadata line
        /// </summary>
        internal int InsertOrUpdateMetadatas(DbCommand command, DmRow row, ScopeInfo scope)
        {
            int rowsApplied = 0;

            if (command == null)
            {
                var exc = $"Missing command for apply metadata ";
                Logger.Current.Error(exc);
                throw new Exception(exc);
            }

            // Set the id parameter
            this.SetColumnParameters(command, row);

            DbManager.SetParameterValue(command, "sync_scope_id", scope.Id);
            DbManager.SetParameterValue(command, "sync_row_is_tombstone", row.RowState == DmRowState.Deleted ? 1 : 0);
            DbManager.SetParameterValue(command, "create_timestamp", scope.LastTimestamp);
            DbManager.SetParameterValue(command, "update_timestamp", scope.LastTimestamp);

            try
            {
                var alreadyOpened = Connection.State == ConnectionState.Open;

                // OPen Connection
                if (!alreadyOpened)
                    Connection.Open();

                if (Transaction != null)
                    command.Transaction = Transaction;

                command.ExecuteNonQuery();

                // get the row count
                rowsApplied = DbManager.GetSyncIntOutParameter("sync_row_count", command);
            }
            catch (DbException ex)
            {
                Logger.Current.Error(ex.Message);
                throw;
            }

            return rowsApplied;
        }

        /// <summary>
        /// Try to get a source row
        /// </summary>
        /// <returns></returns>
        private DmTable GetRow(DmRow sourceRow)
        {
            // Get the row in the local repository
            DbCommand selectCommand = GetCommand(DbObjectType.SelectRowProcName);

            var alreadyOpened = Connection.State == ConnectionState.Open;

            // Open Connection
            if (!alreadyOpened)
                Connection.Open();

            if (Transaction != null)
                selectCommand.Transaction = Transaction;

            this.SetColumnParameters(selectCommand, sourceRow);

            var dmTableSelected = new DmTable(this.TableDescription.TableName);
            try
            {

                using (var reader = selectCommand.ExecuteReader())
                    dmTableSelected.Fill(reader);

                // set the pkey since we will need them later
                var pkeys = new DmColumn[this.TableDescription.PrimaryKey.Columns.Length];
                for (int i = 0; i < pkeys.Length; i++)
                {
                    var pkName = this.TableDescription.PrimaryKey.Columns[i].ColumnName;
                    pkeys[i] = dmTableSelected.Columns.First(dm => this.TableDescription.IsEqual(dm.ColumnName, pkName));
                }
                dmTableSelected.PrimaryKey = new DmKey(pkeys);
            }
            catch (Exception ex)
            {
                Logger.Current.Error("Server Error on Getting a row : " + ex.Message);
                throw;
            }

            return dmTableSelected;
        }

        private DbCommand GetCommand(DbObjectType nameType)
        {
            var command = this.Connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = ObjectNames.GetObjectName(nameType);
            command.Connection = Connection;
            if (Transaction != null)
                command.Transaction = Transaction;
            this.SetCommandSessionParameters(command);
            return command;
        }

        /// <summary>
        /// Launch apply bulk changes
        /// </summary>
        /// <returns></returns>
        public int ApplyBulkChanges(DmView dmChanges, ScopeInfo fromScope, List<SyncConflict> conflicts)
        {
            DbCommand bulkCommand = null;

            if (this.applyType == DmRowState.Added)
                bulkCommand = this.GetCommand(DbObjectType.BulkInsertProcName);
            else if (this.applyType == DmRowState.Modified)
                bulkCommand = this.GetCommand(DbObjectType.BulkUpdateProcName);
            else if (this.applyType == DmRowState.Deleted)
                bulkCommand = this.GetCommand(DbObjectType.BulkDeleteProcName);
            else
                throw new Exception("DmRowState not valid during ApplyBulkChanges operation");

            if (Transaction != null && Transaction.Connection != null)
                bulkCommand.Transaction = Transaction;

            DmTable batchDmTable = dmChanges.Table.Clone();
            DmTable failedDmtable = new DmTable { Culture = CultureInfo.InvariantCulture };

            // Create the schema for failed rows (just add the Primary keys)
            this.AddSchemaForFailedRowsTable(batchDmTable, failedDmtable);

            int batchCount = 0;
            int rowCount = 0;

            foreach (var dmRow in dmChanges)
            {
                // Cancel the delete state to be able to get the row, more simplier
                if (applyType == DmRowState.Deleted)
                    dmRow.RejectChanges();

                // Load the datarow
                DmRow dataRow = batchDmTable.LoadDataRow(dmRow.ItemArray, false);

                // Apply the delete
                // is it mandatory ?
                if (applyType == DmRowState.Deleted)
                    dmRow.Delete();

                batchCount++;
                rowCount++;

                if (batchCount != 500 && rowCount != dmChanges.Count)
                    continue;

                // Since the update and create timestamp come from remote, change name for the bulk operations
                batchDmTable.Columns["update_timestamp"].ColumnName = "update_timestamp";
                batchDmTable.Columns["create_timestamp"].ColumnName = "create_timestamp";

                // execute the batch, through the provider
                ExecuteBatchCommand(bulkCommand, batchDmTable, failedDmtable, fromScope);

                // Clear the batch
                batchDmTable.Clear();

                // Recreate a Clone
                // TODO : Evaluate if it's necessary
                batchDmTable = dmChanges.Table.Clone();
                batchCount = 0;
            }

            // Update table progress 
            //tableProgress.ChangesApplied = dmChanges.Count - failedDmtable.Rows.Count;

            if (failedDmtable.Rows.Count == 0)
                return dmChanges.Count;


            // Check all conflicts raised
            var failedFilter = new Predicate<DmRow>(row =>
            {
                if (row.RowState == DmRowState.Deleted)
                    return failedDmtable.FindByKey(row.GetKeyValues(DmRowVersion.Original)) != null;
                else
                    return failedDmtable.FindByKey(row.GetKeyValues()) != null;
            });



            // New View
            var dmFailedRows = new DmView(dmChanges, failedFilter);

            // Generate a conflict and add it
            foreach (var dmFailedRow in dmFailedRows)
                conflicts.Add(GetConflict(dmFailedRow));

            int failedRows = dmFailedRows.Count;

            // Dispose the failed view
            dmFailedRows.Dispose();

            // return applied rows - failed rows (generating a conflict)
            return dmChanges.Count - failedRows;
        }

        /// <summary>
        /// Try to apply changes on the server.
        /// Internally will call ApplyInsert / ApplyUpdate or ApplyDelete
        /// </summary>
        /// <param name="dmChanges">Changes from remote</param>
        /// <returns>every lines not updated on the server side</returns>
        internal int ApplyChanges(DmView dmChanges, ScopeInfo scope, List<SyncConflict> conflicts)
        {
            int appliedRows = 0;

            foreach (var dmRow in dmChanges)
            {
                bool operationComplete = false;

                if (applyType == DmRowState.Added)
                    operationComplete = this.ApplyInsert(dmRow);
                else if (applyType == DmRowState.Modified)
                    operationComplete = this.ApplyUpdate(dmRow, scope, false);
                else if (applyType == DmRowState.Deleted)
                    operationComplete = this.ApplyDelete(dmRow, scope, false);

                if (operationComplete)
                    // if no pb, increment then go to next row
                    appliedRows++;
                else
                    // Generate a conflict and add it
                    conflicts.Add(GetConflict(dmRow));
            }

            return appliedRows;
        }

        /// <summary>
        /// Apply a single insert in the current data source
        /// </summary>
        internal bool ApplyInsert(DmRow remoteRow)
        {
            var command = this.GetCommand(DbObjectType.InsertProcName);

            // Set the parameters value from row
            SetColumnParameters(command, remoteRow);

            var alreadyOpened = Connection.State == ConnectionState.Open;

            // Open Connection
            if (!alreadyOpened)
                Connection.Open();

            int rowInsertedCount = 0;
            try
            {
                if (Transaction != null)
                    command.Transaction = Transaction;

                command.ExecuteNonQuery();

                // get the row count
                rowInsertedCount = DbManager.GetSyncIntOutParameter("sync_row_count", command);
            }
            catch (ArgumentException ex)
            {
                Logger.Current.Error(ex.Message);
                throw;
            }
            catch (DbException ex)
            {
                Logger.Current.Error(ex.Message);
                return false;
            }
            finally
            {
                // Open Connection
                if (!alreadyOpened)
                    Connection.Close();

            }

            return rowInsertedCount > 0;

        }

        /// <summary>
        /// Apply a delete on a row
        /// </summary>
        internal bool ApplyDelete(DmRow sourceRow, ScopeInfo scope, bool forceWrite)
        {
            var command = this.GetCommand(DbObjectType.DeleteProcName);

            // Set the parameters value from row
            SetColumnParameters(command, sourceRow);

            // special parameters for update
            DbManager.SetParameterValue(command, "sync_force_write", (forceWrite ? 1 : 0));
            DbManager.SetParameterValue(command, "sync_min_timestamp", scope.LastTimestamp);

            var alreadyOpened = Connection.State == ConnectionState.Open;

            int rowInsertedCount = 0;
            try
            {
                // OPen Connection
                if (!alreadyOpened)
                    Connection.Open();

                if (Transaction != null)
                    command.Transaction = Transaction;

                command.ExecuteNonQuery();

                // get the row count
                rowInsertedCount = DbManager.GetSyncIntOutParameter("sync_row_count", command);
            }
            catch (ArgumentException ex)
            {
                Logger.Current.Error(ex.Message);
                throw;
            }
            catch (DbException ex)
            {
                Logger.Current.Error(ex.Message);
                return false;
            }
            finally
            {
                if (!alreadyOpened)
                    Connection.Close();
            }

            return rowInsertedCount > 0;

        }

        /// <summary>
        /// Apply a single update in the current datasource. if forceWrite, override conflict situation and force the update
        /// </summary>
        internal bool ApplyUpdate(DmRow sourceRow, ScopeInfo scope, bool forceWrite)
        {
            var command = this.GetCommand(DbObjectType.UpdateProcName);

            // Set the parameters value from row
            SetColumnParameters(command, sourceRow);

            // special parameters for update
            DbManager.SetParameterValue(command, "sync_force_write", (forceWrite ? 1 : 0));
            DbManager.SetParameterValue(command, "sync_min_timestamp", scope.LastTimestamp);

            var alreadyOpened = Connection.State == ConnectionState.Open;

            int rowInsertedCount = 0;
            try
            {
                // OPen Connection
                if (!alreadyOpened)
                    Connection.Open();

                if (Transaction != null)
                    command.Transaction = Transaction;

                command.ExecuteNonQuery();

                // get the row count
                rowInsertedCount = DbManager.GetSyncIntOutParameter("sync_row_count", command);
            }
            catch (ArgumentException ex)
            {
                Logger.Current.Error(ex.Message);
                throw;
            }
            catch (DbException ex)
            {
                Logger.Current.Error(ex.Message);
                return false;
            }
            finally
            {
                if (!alreadyOpened)
                    Connection.Close();
            }

            return rowInsertedCount > 0;
        }

        /// <summary>
        /// We have a conflict, try to get the source (server) row and generate a conflict
        /// </summary>
        private SyncConflict GetConflict(DmRow dmRow)
        {
            DmRow destinationRow = null;

            // Problem during operation
            // Getting the row involved in the conflict 
            var dmTableSelected = GetRow(dmRow);

            ConflictType dbConflictType = ConflictType.ErrorsOccurred;

            // Can't find the row on the local datastore
            if (dmTableSelected.Rows.Count == 0)
            {
                var errorMessage = "Change Application failed due to Row not Found on the server";

                Logger.Current.Error($"Conflict detected with error: {errorMessage}");

                if (applyType == DmRowState.Added)
                    dbConflictType = ConflictType.LocalNoRowRemoteInsert;
                else if (applyType == DmRowState.Modified)
                    dbConflictType = ConflictType.LocalNoRowRemoteUpdate;
                else if (applyType == DmRowState.Deleted)
                    dbConflictType = ConflictType.LocalNoRowRemoteDelete;
            }
            else
            {
                // We have a problem and found the row on the server side
                destinationRow = dmTableSelected.Rows[0];

                var isTombstone = (bool)destinationRow["sync_row_is_tombstone"];

                // the row on local is deleted
                if (isTombstone)
                {
                    if (applyType == DmRowState.Added)
                        dbConflictType = ConflictType.LocalDeleteRemoteInsert;
                    else if (applyType == DmRowState.Modified)
                        dbConflictType = ConflictType.LocalDeleteRemoteUpdate;
                    else if (applyType == DmRowState.Deleted)
                        dbConflictType = ConflictType.LocalDeleteRemoteDelete;
                }
                else
                {
                    var isLocallyCreated = destinationRow["create_scope_id"] == DBNull.Value;
                    var islocallyUpdated = destinationRow["update_scope_id"] == DBNull.Value;

                    if (applyType == DmRowState.Added && islocallyUpdated)
                        dbConflictType = ConflictType.LocalUpdateRemoteInsert;
                    else if (applyType == DmRowState.Added && isLocallyCreated)
                        dbConflictType = ConflictType.LocalInsertRemoteInsert;
                    else if (applyType == DmRowState.Modified && islocallyUpdated)
                        dbConflictType = ConflictType.LocalUpdateRemoteUpdate;
                    else if (applyType == DmRowState.Modified && isLocallyCreated)
                        dbConflictType = ConflictType.LocalInsertRemoteUpdate;
                    else if (applyType == DmRowState.Deleted && islocallyUpdated)
                        dbConflictType = ConflictType.LocalUpdateRemoteDelete;
                    else if (applyType == DmRowState.Deleted && isLocallyCreated)
                        dbConflictType = ConflictType.LocalInsertRemoteDelete;

                }
            }


            // Generate the conflict
            var conflict = new SyncConflict(dbConflictType);
            conflict.AddRemoteRow(dmRow);
            if (destinationRow != null)
                conflict.AddLocalRow(destinationRow);

            dmTableSelected.Clear();

            return conflict;
        }


        /// <summary>
        /// Adding failed rows when used by a bulk operation
        /// </summary>
        private void AddSchemaForFailedRowsTable(DmTable applyTable, DmTable failedRows)
        {
            if (failedRows.Columns.Count == 0)
            {
                foreach (var rowIdColumn in this.TableDescription.PrimaryKey.Columns)
                    failedRows.Columns.Add(rowIdColumn.ColumnName, rowIdColumn.DataType);

                DmColumn[] keys = new DmColumn[this.TableDescription.PrimaryKey.Columns.Length];

                for (int i = 0; i < this.TableDescription.PrimaryKey.Columns.Length; i++)
                    keys[i] = failedRows.Columns[i];

                failedRows.PrimaryKey = new DmKey(keys);
            }
        }


        internal ApplyAction ConflictApplyAction { get; set; } = ApplyAction.Continue;

        /// <summary>
        /// Handle a conflict
        /// </summary>
        internal ChangeApplicationAction HandleConflict(SyncConflict conflict, ScopeInfo scope, long timestamp, out DmRow finalRow)
        {
            finalRow = null;

            // overwrite apply action if we handle it (ie : user wants to change the action)
            if (this.ConflictActionInvoker != null)
                ConflictApplyAction = this.ConflictActionInvoker(conflict, Connection, Transaction);

            // Default behavior and an error occured
            if (ConflictApplyAction == ApplyAction.Rollback)
            {
                Logger.Current.Info("Rollback all operation");

                return ChangeApplicationAction.Rollback;
            }

            // Server wins
            if (ConflictApplyAction == ApplyAction.Continue)
            {
                Logger.Current.Info("Local Wins, update metadata");


                // COnflict on a line that is not present on the datasource
                if (conflict.LocalChange == null || conflict.LocalChange.Rows == null || conflict.LocalChange.Rows.Count == 0)
                {
                    return ChangeApplicationAction.Continue;
                }

                if (conflict.LocalChange != null && conflict.LocalChange.Rows != null && conflict.LocalChange.Rows.Count > 0)
                {
                    var localRow = conflict.LocalChange.Rows[0];
                    // TODO : Différencier le timestamp de mise à jour ou de création
                    var updateMetadataCommand = GetCommand(DbObjectType.UpdateMetadataProcName);

                    // create a localscope to override values
                    var localScope = new ScopeInfo { Name = null, LastTimestamp = timestamp };

                    var rowsApplied = this.InsertOrUpdateMetadatas(updateMetadataCommand, localRow, localScope);

                    if (rowsApplied < 1)
                        throw new Exception("No metadatas rows found, can't update the server side");

                    finalRow = localRow;

                    return ChangeApplicationAction.Continue;
                }

                // tableProgress.ChangesFailed += 1;
                return ChangeApplicationAction.Rollback;
            }

            // We gonna apply with force the remote line
            if (ConflictApplyAction == ApplyAction.RetryWithForceWrite)
            {
                if (conflict.RemoteChange.Rows.Count == 0)
                {
                    Logger.Current.Error("Cant find a remote row");
                    return ChangeApplicationAction.Rollback;
                }

                var row = conflict.RemoteChange.Rows[0];
                bool operationComplete = false;

                // create a localscope to override values
                var localScope = new ScopeInfo { Name = scope.Name, LastTimestamp = timestamp };


                if (conflict.Type == ConflictType.LocalNoRowRemoteUpdate || conflict.Type == ConflictType.LocalNoRowRemoteInsert)
                    operationComplete = this.ApplyInsert(row);
                else if (applyType == DmRowState.Added)
                    operationComplete = this.ApplyInsert(row);
                else if (applyType == DmRowState.Modified)
                    operationComplete = this.ApplyUpdate(row, localScope, true);
                else if (applyType == DmRowState.Deleted)
                    operationComplete = this.ApplyDelete(row, localScope, true);

                var insertMetadataCommand = GetCommand(DbObjectType.InsertMetadataProcName);
                var rowsApplied = this.InsertOrUpdateMetadatas(insertMetadataCommand, row, localScope);
                if (rowsApplied < 1)
                    throw new Exception("No metadatas rows found, can't update the server side");

                finalRow = row;

                //After a force update, there is a problem, so raise exception
                if (!operationComplete)
                {
                    var ex = $"Can't force operation for applyType {applyType}";
                    Logger.Current.Error(ex);
                    finalRow = null;
                    return ChangeApplicationAction.Rollback;
                }

                // tableProgress.ChangesApplied += 1;
                return ChangeApplicationAction.Continue;
            }

            return ChangeApplicationAction.Rollback;

        }


        /// <summary>
        /// Trace info
        /// </summary>
        private void TraceRowInfo(DmRow row, bool succeeded)
        {
            string pKstr = "";
            foreach (var rowIdColumn in this.TableDescription.PrimaryKey.Columns)
            {
                object obj = pKstr;
                object[] item = { obj, rowIdColumn.ColumnName, "=\"", row[rowIdColumn], "\" " };
                pKstr = string.Concat(item);
            }

            string empty = string.Empty;
            switch (applyType)
            {
                case DmRowState.Added:
                    {
                        empty = (succeeded ? "Inserted" : "Failed to insert");
                        Logger.Current.Debug($"{empty} row with PK using bulk apply: {pKstr} on {Connection.Database}");
                        return;
                    }
                case DmRowState.Modified:
                    {
                        empty = (succeeded ? "Updated" : "Failed to update");
                        Logger.Current.Debug($"{empty} row with PK using bulk apply: {pKstr} on {Connection.Database}");
                        return;
                    }
                case DmRowState.Deleted:
                    {
                        empty = (succeeded ? "Deleted" : "Failed to delete");
                        Logger.Current.Debug($"{empty} row with PK using bulk apply: {pKstr} on {Connection.Database}");
                        break;
                    }
                default:
                    {
                        return;
                    }
            }
        }

      

    }

}
