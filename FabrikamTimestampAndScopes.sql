/****** Script for SelectTopNRows command from SSMS  ******/

use ClientDB

SELECT TOP (1000) [sync_scope_id] ,[sync_scope_name] ,convert(bigint, @@DBTS) as DBTS, convert(bigint, [scope_timestamp]) as scope_timestamp ,[scope_is_local]
FROM [scope_info]

use ServerDB

SELECT TOP (1000) [sync_scope_id] ,[sync_scope_name] ,convert(bigint, @@DBTS) as DBTS, convert(bigint, [scope_timestamp]) as scope_timestamp ,[scope_is_local]
FROM [scope_info]
