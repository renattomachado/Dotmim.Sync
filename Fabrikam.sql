GO
/****** Object:  UserDefinedTableType [dbo].[ServiceTickets_BulkType]    Script Date: 03/08/2016 11:03:41 ******/
CREATE TYPE [dbo].[ServiceTickets_BulkType] AS TABLE(
	[ServiceTicketID] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[StatusValue] [int] NULL,
	[EscalationLevel] [int] NULL,
	[Opened] [datetime] NULL,
	[Closed] [datetime] NULL,
	[CustomerID] [int] NULL,
	[update_peer_timestamp] [bigint] NULL,
	[create_peer_timestamp] [bigint] NULL,
	PRIMARY KEY CLUSTERED 
(
	[ServiceTicketID] ASC
)WITH (IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  Table [dbo].[scope_config]    Script Date: 03/08/2016 11:03:42 ******/
CREATE TABLE [dbo].[scope_config](
	[config_id] [uniqueidentifier] NOT NULL,
	[config_data] [xml] NOT NULL,
	[scope_status] [char](1) NULL,
 CONSTRAINT [PK_scope_config] PRIMARY KEY CLUSTERED 
(
	[config_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[scope_info]    Script Date: 03/08/2016 11:03:42 ******/
CREATE TABLE [dbo].[scope_info](
	[sync_scope_name] [nvarchar](100) NOT NULL,
	[scope_timestamp] [timestamp] NULL,
	[scope_config_id] [uniqueidentifier] NULL,
	[scope_user_comment] [nvarchar](max) NULL,
 CONSTRAINT [PK_scope_info] PRIMARY KEY CLUSTERED 
(
	[sync_scope_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ServiceTickets]    Script Date: 03/08/2016 11:03:42 ******/
CREATE TABLE [dbo].[ServiceTickets](
	[ServiceTicketID] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[StatusValue] [int] NOT NULL,
	[EscalationLevel] [int] NOT NULL,
	[Opened] [datetime] NULL,
	[Closed] [datetime] NULL,
	[CustomerID] [int] NULL,
 CONSTRAINT [PK_ServiceTickets] PRIMARY KEY CLUSTERED 
(
	[ServiceTicketID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ServiceTickets_tracking]    Script Date: 03/08/2016 11:03:42 ******/
CREATE TABLE [dbo].[ServiceTickets_tracking](
	[ServiceTicketID] [uniqueidentifier] NOT NULL,
	[create_scope_name] [nvarchar](100) NULL,
	[update_scope_name] [nvarchar](100) NULL,
	[create_timestamp] [bigint] NULL,
	[update_timestamp] [bigint] NULL,
	[timestamp] [timestamp] NULL,
	[sync_row_is_tombstone] [int] NOT NULL,
	[last_change_datetime] [datetime] NULL,
	[CustomerID] [int] NULL,
 CONSTRAINT [PK_ServiceTickets_tracking] PRIMARY KEY CLUSTERED 
(
	[ServiceTicketID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
INSERT [dbo].[scope_config] ([config_id], [config_data], [scope_status]) VALUES (N'a153647b-db64-4609-95c2-2edd1b7c8754', N'<SqlSyncProviderScopeConfiguration xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" IsTemplate="false"><Adapter Name="[ServiceTickets]" GlobalName="[ServiceTickets]" TrackingTable="[ServiceTickets_tracking]" SelChngProc="[ServiceTickets_selectchanges]" SelRowProc="[ServiceTickets_selectrow]" InsProc="[ServiceTickets_insert]" UpdProc="[ServiceTickets_update]" DelProc="[ServiceTickets_delete]" InsMetaProc="[ServiceTickets_insertmetadata]" UpdMetaProc="[ServiceTickets_updatemetadata]" DelMetaProc="[ServiceTickets_deletemetadata]" BulkTableType="[ServiceTickets_BulkType]" BulkInsProc="[ServiceTickets_bulkinsert]" BulkUpdProc="[ServiceTickets_bulkupdate]" BulkDelProc="[ServiceTickets_bulkdelete]" InsTrig="[ServiceTickets_insert_trigger]" UpdTrig="[ServiceTickets_update_trigger]" DelTrig="[ServiceTickets_delete_trigger]"><Col name="ServiceTicketID" type="uniqueidentifier" param="@P_1" pk="true" /><Col name="Title" type="nvarchar" size="max" param="@P_2" /><Col name="Description" type="nvarchar" size="max" null="true" param="@P_3" /><Col name="StatusValue" type="int" param="@P_4" /><Col name="EscalationLevel" type="int" param="@P_5" /><Col name="Opened" type="datetime" null="true" param="@P_6" /><Col name="Closed" type="datetime" null="true" param="@P_7" /><Col name="CustomerID" type="int" null="true" param="@P_8" /><FilterClause /></Adapter></SqlSyncProviderScopeConfiguration>', N'C')
INSERT [dbo].[scope_info] ([sync_scope_name], [scope_config_id], [scope_user_comment]) VALUES (N'DefaultScope', N'a153647b-db64-4609-95c2-2edd1b7c8754', N'')
INSERT [dbo].[scope_info] ([sync_scope_name], [scope_config_id], [scope_user_comment]) VALUES (N'DefaultScope_b13f6cdb-31dd-408a-bcc3-1dbed79b6553', N'a153647b-db64-4609-95c2-2edd1b7c8754', NULL)
INSERT [dbo].[ServiceTickets] ([ServiceTicketID], [Title], [Description], [StatusValue], [EscalationLevel], [Opened], [Closed], [CustomerID]) VALUES (N'c3cd62ac-314b-4dbf-b2bb-0d154d59c9b3', N'Titre 3', N'Description 3', 1, 0, CAST(N'2016-07-29T16:36:41.733' AS DateTime), NULL, 1)
INSERT [dbo].[ServiceTickets] ([ServiceTicketID], [Title], [Description], [StatusValue], [EscalationLevel], [Opened], [Closed], [CustomerID]) VALUES (N'04254b03-3dcb-4232-9b74-36f71b7ad26c', N'Titre 4', N'Description 4', 1, 0, CAST(N'2016-07-29T16:36:41.733' AS DateTime), NULL, 1)
INSERT [dbo].[ServiceTickets] ([ServiceTicketID], [Title], [Description], [StatusValue], [EscalationLevel], [Opened], [Closed], [CustomerID]) VALUES (N'3d7c603e-5790-4d99-9737-37a9db95a642', N'Titre Client 1', N'Description Client 1', 1, 0, CAST(N'2016-07-29T17:26:20.720' AS DateTime), NULL, 1)
INSERT [dbo].[ServiceTickets] ([ServiceTicketID], [Title], [Description], [StatusValue], [EscalationLevel], [Opened], [Closed], [CustomerID]) VALUES (N'aba16ee2-50d0-4d0c-870d-607cf88a74b5', N'Titre 6', N'Description 6', 1, 0, CAST(N'2016-07-29T16:36:41.733' AS DateTime), NULL, 1)
INSERT [dbo].[ServiceTickets] ([ServiceTicketID], [Title], [Description], [StatusValue], [EscalationLevel], [Opened], [Closed], [CustomerID]) VALUES (N'f30bb2f9-6cb3-4957-89b1-65246319e3da', N'Titre 7', N'Description 7', 1, 0, CAST(N'2016-07-29T16:36:41.733' AS DateTime), NULL, 1)
INSERT [dbo].[ServiceTickets] ([ServiceTicketID], [Title], [Description], [StatusValue], [EscalationLevel], [Opened], [Closed], [CustomerID]) VALUES (N'55d080ff-713b-456a-b900-8bd6d310593c', N'Titre 5', N'Description 5', 1, 0, CAST(N'2016-07-29T16:36:41.733' AS DateTime), NULL, 1)
INSERT [dbo].[ServiceTickets] ([ServiceTicketID], [Title], [Description], [StatusValue], [EscalationLevel], [Opened], [Closed], [CustomerID]) VALUES (N'036e6ac5-bda9-4c45-aa0e-ab1f603319c3', N'Titre 2', N'Description 2', 1, 0, CAST(N'2016-07-29T16:36:41.733' AS DateTime), NULL, 1)
INSERT [dbo].[ServiceTickets] ([ServiceTicketID], [Title], [Description], [StatusValue], [EscalationLevel], [Opened], [Closed], [CustomerID]) VALUES (N'2ca388cf-8d9a-4b7a-9bd1-b0bc6617a766', N'Titre 1', N'Description 1', 1, 0, CAST(N'2016-07-29T16:36:41.733' AS DateTime), NULL, 1)
INSERT [dbo].[ServiceTickets] ([ServiceTicketID], [Title], [Description], [StatusValue], [EscalationLevel], [Opened], [Closed], [CustomerID]) VALUES (N'c762b922-737a-448a-a9b6-bcf02e803eb5', N'Titre 8', N'Description 8', 1, 0, CAST(N'2016-07-29T16:36:41.733' AS DateTime), NULL, 1)
INSERT [dbo].[ServiceTickets] ([ServiceTicketID], [Title], [Description], [StatusValue], [EscalationLevel], [Opened], [Closed], [CustomerID]) VALUES (N'17114fe9-5106-4299-9e1a-bf84be37d992', N'Titre 9', N'Description 9', 1, 0, CAST(N'2016-07-29T16:36:41.733' AS DateTime), NULL, 1)
INSERT [dbo].[ServiceTickets_tracking] ([ServiceTicketID], [create_scope_name], [update_scope_name], [create_timestamp], [update_timestamp], [sync_row_is_tombstone], [last_change_datetime], [CustomerID]) VALUES (N'c3cd62ac-314b-4dbf-b2bb-0d154d59c9b3', NULL, NULL, 2387, 2388, 0, CAST(N'2016-07-29T18:30:51.940' AS DateTime), 1)
INSERT [dbo].[ServiceTickets_tracking] ([ServiceTicketID], [create_scope_name], [update_scope_name], [create_timestamp], [update_timestamp], [sync_row_is_tombstone], [last_change_datetime], [CustomerID]) VALUES (N'04254b03-3dcb-4232-9b74-36f71b7ad26c', NULL, NULL, 2387, 2388, 0, CAST(N'2016-07-29T18:30:51.940' AS DateTime), 1)
INSERT [dbo].[ServiceTickets_tracking] ([ServiceTicketID], [create_scope_name], [update_scope_name], [create_timestamp], [update_timestamp], [sync_row_is_tombstone], [last_change_datetime], [CustomerID]) VALUES (N'3d7c603e-5790-4d99-9737-37a9db95a642', NULL, NULL, 2387, 2388, 0, CAST(N'2016-07-29T18:30:51.940' AS DateTime), 1)
INSERT [dbo].[ServiceTickets_tracking] ([ServiceTicketID], [create_scope_name], [update_scope_name], [create_timestamp], [update_timestamp], [sync_row_is_tombstone], [last_change_datetime], [CustomerID]) VALUES (N'aba16ee2-50d0-4d0c-870d-607cf88a74b5', NULL, NULL, 2387, 2388, 0, CAST(N'2016-07-29T18:30:51.940' AS DateTime), 1)
INSERT [dbo].[ServiceTickets_tracking] ([ServiceTicketID], [create_scope_name], [update_scope_name], [create_timestamp], [update_timestamp], [sync_row_is_tombstone], [last_change_datetime], [CustomerID]) VALUES (N'f30bb2f9-6cb3-4957-89b1-65246319e3da', NULL, NULL, 2387, 2388, 0, CAST(N'2016-07-29T18:30:51.940' AS DateTime), 1)
INSERT [dbo].[ServiceTickets_tracking] ([ServiceTicketID], [create_scope_name], [update_scope_name], [create_timestamp], [update_timestamp], [sync_row_is_tombstone], [last_change_datetime], [CustomerID]) VALUES (N'55d080ff-713b-456a-b900-8bd6d310593c', NULL, NULL, 2387, 2388, 0, CAST(N'2016-07-29T18:30:51.940' AS DateTime), 1)
INSERT [dbo].[ServiceTickets_tracking] ([ServiceTicketID], [create_scope_name], [update_scope_name], [create_timestamp], [update_timestamp], [sync_row_is_tombstone], [last_change_datetime], [CustomerID]) VALUES (N'036e6ac5-bda9-4c45-aa0e-ab1f603319c3', NULL, NULL, 2387, 2388, 0, CAST(N'2016-07-29T18:30:51.940' AS DateTime), 1)
INSERT [dbo].[ServiceTickets_tracking] ([ServiceTicketID], [create_scope_name], [update_scope_name], [create_timestamp], [update_timestamp], [sync_row_is_tombstone], [last_change_datetime], [CustomerID]) VALUES (N'2ca388cf-8d9a-4b7a-9bd1-b0bc6617a766', NULL, NULL, 2387, 2388, 0, CAST(N'2016-07-29T18:30:51.940' AS DateTime), 1)
INSERT [dbo].[ServiceTickets_tracking] ([ServiceTicketID], [create_scope_name], [update_scope_name], [create_timestamp], [update_timestamp], [sync_row_is_tombstone], [last_change_datetime], [CustomerID]) VALUES (N'c762b922-737a-448a-a9b6-bcf02e803eb5', NULL, NULL, 2387, 2388, 0, CAST(N'2016-07-29T18:30:51.940' AS DateTime), 1)
INSERT [dbo].[ServiceTickets_tracking] ([ServiceTicketID], [create_scope_name], [update_scope_name], [create_timestamp], [update_timestamp], [sync_row_is_tombstone], [last_change_datetime], [CustomerID]) VALUES (N'17114fe9-5106-4299-9e1a-bf84be37d992', NULL, NULL, 2387, 2388, 0, CAST(N'2016-07-29T18:30:51.940' AS DateTime), 1)
/****** Object:  StoredProcedure [dbo].[ServiceTickets_bulkdelete]    Script Date: 03/08/2016 11:03:42 ******/
GO
CREATE PROCEDURE [dbo].[ServiceTickets_bulkdelete]
	@sync_min_timestamp BigInt,
	@sync_scope_name nvarchar(100),
	@changeTable [ServiceTickets_BulkType] READONLY
AS
BEGIN
-- use a temp table to store the list of PKs that successfully got updated/inserted
declare @changed TABLE ([ServiceTicketID] uniqueidentifier, PRIMARY KEY ([ServiceTicketID]));

-- delete all service tickets where timestamp <= syncmintimestamp or update_scope_local_id = remote id
DELETE [ServiceTickets] 
OUTPUT DELETED.[ServiceTicketID] INTO @changed 
FROM [ServiceTickets] base 
JOIN (
		SELECT p.*, t.update_scope_name, t.[timestamp]
		FROM @changeTable p 
		JOIN [ServiceTickets_tracking] t ON p.[ServiceTicketID] = t.[ServiceTicketID]
	  
	  ) as changes ON changes.[ServiceTicketID] = base.[ServiceTicketID] 
WHERE 
-- Last chanegs was from the current scope, so we can delete it since we are sure no one else edit it 
changes.update_scope_name = @sync_scope_name 
-- no change since the last time the current scope has sync (so no one has update the row)
OR 
changes.[timestamp] <= @sync_min_timestamp

-- Update the tacking table
UPDATE side SET
	sync_row_is_tombstone = 1, 
	update_scope_name = @sync_scope_name, 
	update_timestamp = changes.update_peer_timestamp
FROM [ServiceTickets_tracking] side 
JOIN (
		SELECT	p.[ServiceTicketID], 
				p.update_peer_timestamp, 
				p.create_peer_timestamp 
		FROM @changed t JOIN @changeTable p ON p.[ServiceTicketID] = t.[ServiceTicketID]
	  ) AS changes ON changes.[ServiceTicketID] = side.[ServiceTicketID]

--Select all ids not deleted for conflict
SELECT [ServiceTicketID] FROM @changeTable t 
WHERE NOT EXISTS (SELECT [ServiceTicketID] from @changed i WHERE t.[ServiceTicketID] = i.[ServiceTicketID])
END



GO
/****** Object:  StoredProcedure [dbo].[ServiceTickets_bulkinsert]    Script Date: 03/08/2016 11:03:42 ******/

CREATE PROCEDURE [dbo].[ServiceTickets_bulkinsert]
	@sync_min_timestamp BigInt,
	@sync_scope_name nvarchar(100),
	@changeTable [ServiceTickets_BulkType] READONLY
AS
BEGIN
-- use a temp table to store the list of PKs that successfully got updated/inserted
DECLARE @changed TABLE ([ServiceTicketID] uniqueidentifier, PRIMARY KEY ([ServiceTicketID]));

-- update/insert into the base table
MERGE [ServiceTickets] AS base USING
	
	-- join done here against the side table to get the local timestamp for concurrency check
	(SELECT p.*, t.[timestamp] 
	 FROM @changeTable p 
	 LEFT JOIN [ServiceTickets_tracking] t ON p.[ServiceTicketID] = t.[ServiceTicketID]
	 ) AS changes ON changes.[ServiceTicketID] = base.[ServiceTicketID]
	 
	 -- Si la ligne n'existe pas en local et qu'elle a été créé avant le timestamp de référence
WHEN NOT MATCHED BY TARGET 
	 AND changes.[timestamp] <= @sync_min_timestamp OR changes.[timestamp] IS NULL THEN
INSERT ([ServiceTicketID], 
		[Title], 
		[Description], 
		[StatusValue], 
		[EscalationLevel], 
		[Opened], 
		[Closed], 
		[CustomerID]) 
VALUES (changes.[ServiceTicketID], 
		changes.[Title], 
		changes.[Description], 
		changes.[StatusValue], 
		changes.[EscalationLevel], 
		changes.[Opened], 
		changes.[Closed], 
		changes.[CustomerID])
-- populates the temp table with successful PKs
OUTPUT INSERTED.[ServiceTicketID] INTO @changed; 

UPDATE side SET
	update_scope_name = @sync_scope_name, 
	create_scope_name = @sync_scope_name,
	update_timestamp = changes.update_peer_timestamp,
	create_timestamp = changes.create_peer_timestamp
FROM [ServiceTickets_tracking] side 
JOIN (
		SELECT p.[ServiceTicketID], p.update_peer_timestamp, p.create_peer_timestamp 
		FROM @changed t 
		JOIN @changeTable p ON p.[ServiceTicketID] = t.[ServiceTicketID]
	  ) AS changes ON changes.[ServiceTicketID] = side.[ServiceTicketID]

-- Select every keys from the change table that has not been inserted and return them
SELECT [ServiceTicketID] 
FROM @changeTable t 
WHERE NOT EXISTS (SELECT [ServiceTicketID] from @changed i WHERE t.[ServiceTicketID] = i.[ServiceTicketID])
END



GO
/****** Object:  StoredProcedure [dbo].[ServiceTickets_bulkupdate]    Script Date: 03/08/2016 11:03:42 ******/

CREATE PROCEDURE [dbo].[ServiceTickets_bulkupdate]
	@sync_min_timestamp BigInt,
	@sync_scope_name nvarchar(100),
	@changeTable [ServiceTickets_BulkType] READONLY
AS
BEGIN
-- use a temp table to store the list of PKs that successfully got updated
declare @changed TABLE ([ServiceTicketID] uniqueidentifier, PRIMARY KEY ([ServiceTicketID]));

-- update the base table
MERGE [ServiceTickets] AS base USING
	-- join done here against the side table to get the local timestamp for concurrency check
	(	SELECT p.*, 
			t.update_scope_name, 
			t.[timestamp]
		FROM @changeTable p 
		LEFT JOIN [ServiceTickets_tracking] t ON p.[ServiceTicketID] = t.[ServiceTicketID]
	) as changes ON changes.[ServiceTicketID] = base.[ServiceTicketID]
WHEN MATCHED AND (changes.update_scope_name = @sync_scope_name) OR changes.[timestamp] <= @sync_min_timestamp THEN

UPDATE 
	SET [Title] = changes.[Title], 
	[Description] = changes.[Description], 
	[StatusValue] = changes.[StatusValue], 
	[EscalationLevel] = changes.[EscalationLevel], 
	[Opened] = changes.[Opened], 
	[Closed] = changes.[Closed], 
	[CustomerID] = changes.[CustomerID]
OUTPUT INSERTED.[ServiceTicketID] into @changed; -- populates the temp table with successful PKs

UPDATE side 
SET update_scope_name = @sync_scope_name, 
	update_timestamp = changes.update_peer_timestamp
FROM [ServiceTickets_tracking] side 
JOIN 
	(
		SELECT	p.[ServiceTicketID], 
				p.update_peer_timestamp, 
				p.create_peer_timestamp 
		FROM @changed t 
		JOIN @changeTable p ON p.[ServiceTicketID] = t.[ServiceTicketID]
	) as changes ON changes.[ServiceTicketID] = side.[ServiceTicketID]


-- Select every keys from the change table that has not been updated and return them
SELECT [ServiceTicketID] 
FROM @changeTable t 
WHERE NOT EXISTS (SELECT [ServiceTicketID] from @changed i WHERE t.[ServiceTicketID] = i.[ServiceTicketID])
END

GO
/****** Object:  StoredProcedure [dbo].[ServiceTickets_delete]    Script Date: 03/08/2016 11:03:42 ******/

CREATE PROCEDURE [dbo].[ServiceTickets_delete]
	@P_1 UniqueIdentifier,
	@sync_force_write Int,
	@sync_min_timestamp BigInt,
	@sync_row_count Int OUTPUT
AS
BEGIN
SET @sync_row_count = 0; 


DELETE [ServiceTickets] 
FROM [ServiceTickets] [base] 
JOIN [ServiceTickets_tracking] [side] ON [base].[ServiceTicketID] = [side].[ServiceTicketID] 
WHERE ([side].[timestamp] <= @sync_min_timestamp OR @sync_force_write = 1) 
AND ([base].[ServiceTicketID] = @P_1); 

SET @sync_row_count = @@ROWCOUNT;
END



GO
/****** Object:  StoredProcedure [dbo].[ServiceTickets_deletemetadata]    Script Date: 03/08/2016 11:03:42 ******/

CREATE PROCEDURE [dbo].[ServiceTickets_deletemetadata]
	@P_1 UniqueIdentifier,
	@sync_row_count Int OUTPUT
AS
BEGIN
SET @sync_row_count = 0; 

	DELETE [side] 
	FROM [ServiceTickets_tracking] [side] 
	WHERE [ServiceTicketID] = @P_1 ;

SET @sync_row_count = @@ROWCOUNT;
END



GO
/****** Object:  StoredProcedure [dbo].[ServiceTickets_insert]    Script Date: 03/08/2016 11:03:42 ******/

CREATE PROCEDURE [dbo].[ServiceTickets_insert]
	@P_1 UniqueIdentifier,
	@P_2 NVarChar(max),
	@P_3 NVarChar(max),
	@P_4 Int,
	@P_5 Int,
	@P_6 DateTime,
	@P_7 DateTime,
	@P_8 Int,
	@sync_row_count Int OUTPUT
AS
BEGIN
SET @sync_row_count = 0; 

	IF NOT EXISTS (SELECT * FROM [ServiceTickets_tracking] WHERE [ServiceTicketID] = @P_1) 
	BEGIN 
		INSERT INTO [ServiceTickets]([ServiceTicketID], [Title], [Description], [StatusValue], [EscalationLevel], [Opened], [Closed], [CustomerID]) 
		VALUES (@P_1, @P_2, @P_3, @P_4, @P_5, @P_6, @P_7, @P_8);  
		SET @sync_row_count = @@rowcount;  
	END 
END



GO
/****** Object:  StoredProcedure [dbo].[ServiceTickets_insertmetadata]    Script Date: 03/08/2016 11:03:42 ******/

CREATE PROCEDURE [dbo].[ServiceTickets_insertmetadata]
	@P_1 UniqueIdentifier,
	@sync_scope_name nvarchar(100),
	@sync_row_is_tombstone Int,
	@create_timestamp BigInt,
	@update_timestamp BigInt,
	@sync_row_count Int OUTPUT
AS
BEGIN
SET @sync_row_count = 0; 
	UPDATE [ServiceTickets_tracking] 
	SET	[create_scope_name] = @sync_scope_name, 
		[create_timestamp] = @create_timestamp, 
		[update_scope_name] = @sync_scope_name, 
		[update_timestamp] = @update_timestamp, 
		[sync_row_is_tombstone] = @sync_row_is_tombstone 
	WHERE ([ServiceTicketID] = @P_1);
	
	SET @sync_row_count = @@ROWCOUNT;
	
	IF (@sync_row_count = 0) 
	BEGIN 
		INSERT INTO [ServiceTickets_tracking] 
			(	[ServiceTicketID], 
				[create_scope_name], 
				[create_timestamp], 
				[update_scope_name], 
				[update_timestamp], 
				[sync_row_is_tombstone], 
				[last_change_datetime]) 
			VALUES 
			(	@P_1, 
				@sync_scope_name, 
				@create_timestamp, 
				@sync_scope_name, 
				@update_timestamp, 
				@sync_row_is_tombstone, 
				GETDATE());
			
		SET @sync_row_count = @@ROWCOUNT; 
	END;
END


GO
/****** Object:  StoredProcedure [dbo].[ServiceTickets_selectchanges]    Script Date: 03/08/2016 11:03:42 ******/

CREATE PROCEDURE [dbo].[ServiceTickets_selectchanges]
	@sync_min_timestamp BigInt, -- last client select time stamp
	@sync_scope_name nvarchar(100) -- pour quel client on fait le selectchanges
AS
BEGIN
SELECT [side].[ServiceTicketID], [base].[Title], [base].[Description], 
	   [base].[StatusValue], [base].[EscalationLevel], 
	   [base].[Opened], [base].[Closed], [base].[CustomerID], 
	   [side].[sync_row_is_tombstone], 
	   [side].[update_timestamp], 
	   [side].[create_timestamp]

FROM [ServiceTickets] [base] 
RIGHT JOIN [ServiceTickets_tracking] [side] ON [base].[ServiceTicketID] = [side].[ServiceTicketID] 
WHERE  (
			-- update machine
			[side].[update_scope_name] IS NULL 
			-- ou update différent du remote
			OR [side].[update_scope_name] <> @sync_scope_name 
		) 
		-- Et le timestamp est > à celui passé
		AND [side].[timestamp] > @sync_min_timestamp
END
 


GO
/****** Object:  StoredProcedure [dbo].[ServiceTickets_selectrow]    Script Date: 03/08/2016 11:03:42 ******/

CREATE PROCEDURE [dbo].[ServiceTickets_selectrow]
	@P_1 UniqueIdentifier,
	@sync_scope_name nvarchar(100) -- pour quel client on fait le selectchanges

AS
BEGIN
SELECT	[side].[ServiceTicketID], 
		[base].[Title], 
		[base].[Description], 
		[base].[StatusValue], 
		[base].[EscalationLevel], 
		[base].[Opened], 
		[base].[Closed], 
		[base].[CustomerID], 
		[side].[sync_row_is_tombstone], 
	    [side].[update_timestamp], 
	    [side].[create_timestamp]
from [ServiceTickets] [base] 
right join [ServiceTickets_tracking] [side] on [base].[ServiceTicketID] = [side].[ServiceTicketID] 
WHERE [side].[ServiceTicketID] = @P_1

END


GO
/****** Object:  StoredProcedure [dbo].[ServiceTickets_update]    Script Date: 03/08/2016 11:03:42 ******/

CREATE PROCEDURE [dbo].[ServiceTickets_update]
	@P_1 UniqueIdentifier,
	@P_2 NVarChar(max),
	@P_3 NVarChar(max),
	@P_4 Int,
	@P_5 Int,
	@P_6 DateTime,
	@P_7 DateTime,
	@P_8 Int,
	@sync_force_write Int,
	@sync_min_timestamp BigInt,
	@sync_row_count Int OUTPUT
AS
BEGIN
	SET @sync_row_count = 0; 
	UPDATE [ServiceTickets] 
	SET		[Title] = @P_2, 
			[Description] = @P_3, 
			[StatusValue] = @P_4, 
			[EscalationLevel] = @P_5, 
			[Opened] = @P_6, 
			[Closed] = @P_7, 
			[CustomerID] = @P_8 
	FROM [ServiceTickets] [base] 
	JOIN [ServiceTickets_tracking] [side] ON [base].[ServiceTicketID] = [side].[ServiceTicketID] 
	WHERE ([side].[timestamp] <= @sync_min_timestamp OR @sync_force_write = 1) 
	AND ([base].[ServiceTicketID] = @P_1); 
	
	SET @sync_row_count = @@ROWCOUNT;
END

GO
/****** Object:  StoredProcedure [dbo].[ServiceTickets_updatemetadata]    Script Date: 03/08/2016 11:03:42 ******/

CREATE PROCEDURE [dbo].[ServiceTickets_updatemetadata]
	@P_1 UniqueIdentifier,
	@sync_scope_name nvarchar(100),
	@sync_row_is_tombstone Int,
	@create_timestamp BigInt,
	@update_timestamp BigInt,
	@sync_row_count Int OUTPUT
AS
BEGIN
SET @sync_row_count = 0; 

DECLARE @was_tombstone int; 

SELECT @was_tombstone = [sync_row_is_tombstone] 
FROM [ServiceTickets_tracking] 
WHERE ([ServiceTicketID] = @P_1);


-- Si la ligne était supprimée et qu'on décide que non finalement
-- alors on réimplémente la ligne comme étant créée par le remote
IF (@was_tombstone IS NOT NULL AND @was_tombstone = 1 AND @sync_row_is_tombstone = 0) 
BEGIN 
	UPDATE [ServiceTickets_tracking] 
	SET [create_scope_name] = @sync_scope_name, 
		[update_scope_name] = @sync_scope_name, 

		[create_timestamp] = @create_timestamp, 
		[update_timestamp] = @update_timestamp, 

		[sync_row_is_tombstone] = @sync_row_is_tombstone 
	WHERE ([ServiceTicketID] = @P_1); 
END 
ELSE 
BEGIN 
	UPDATE [ServiceTickets_tracking] 
	SET 
		[update_scope_name] = @sync_scope_name, 
		[update_timestamp] = @update_timestamp, 
		
		[sync_row_is_tombstone] = @sync_row_is_tombstone 
	WHERE ([ServiceTicketID] = @P_1); 
END;

SET @sync_row_count = @@ROWCOUNT;
END



GO
/****** Object:  Trigger [dbo].[ServiceTickets_delete_trigger]    Script Date: 03/08/2016 11:03:42 ******/
CREATE TRIGGER [dbo].[ServiceTickets_delete_trigger] ON [dbo].[ServiceTickets] FOR DELETE AS

UPDATE [side] 
SET		[sync_row_is_tombstone] = 1, 
		[update_scope_name] = NULL, 
		[update_timestamp] = @@DBTS + 1,
		[last_change_datetime] = GETDATE(), 
		[CustomerID] = [d].[CustomerID] 
FROM	[ServiceTickets_tracking] [side] 
JOIN	DELETED AS [d] ON [side].[ServiceTicketID] = [d].[ServiceTicketID]



GO
ALTER TABLE [dbo].[ServiceTickets] ENABLE TRIGGER [ServiceTickets_delete_trigger]
GO
/****** Object:  Trigger [dbo].[ServiceTickets_insert_trigger]    Script Date: 03/08/2016 11:03:42 ******/
CREATE TRIGGER [dbo].[ServiceTickets_insert_trigger] ON [dbo].[ServiceTickets] FOR INSERT AS

UPDATE [side] 
SET [sync_row_is_tombstone] = 0, 
	[update_scope_name] = NULL, 
	[last_change_datetime] = GETDATE(), 
	[CustomerID] = [i].[CustomerID] 
FROM [ServiceTickets_tracking] [side] 
JOIN INSERTED AS [i] ON [side].[ServiceTicketID] = [i].[ServiceTicketID]

INSERT INTO [ServiceTickets_tracking] 
	(	[i].[ServiceTicketID], 
		[create_scope_name], 
		[create_timestamp], 
		[update_scope_name], 
		[update_timestamp], 
		[sync_row_is_tombstone], 
		[last_change_datetime], 
		[i].[CustomerID]) 
SELECT	[i].[ServiceTicketID], 
		NULL, 
		@@DBTS+1, 
		NULL, 
		@@DBTS+1,  
		0, 
		GETDATE() , 
		[i].[CustomerID] 
FROM	INSERTED AS [i] 
LEFT JOIN [ServiceTickets_tracking] [side] ON [side].[ServiceTicketID] = [i].[ServiceTicketID] 
WHERE [side].[ServiceTicketID] IS NULL



GO
ALTER TABLE [dbo].[ServiceTickets] ENABLE TRIGGER [ServiceTickets_insert_trigger]
GO
/****** Object:  Trigger [dbo].[ServiceTickets_update_trigger]    Script Date: 03/08/2016 11:03:42 ******/
CREATE TRIGGER [dbo].[ServiceTickets_update_trigger] ON [dbo].[ServiceTickets] FOR UPDATE AS
UPDATE [side] 
	SET [update_scope_name] = NULL, 
		[last_change_datetime] = GETDATE(),
		[update_timestamp] = @@DBTS + 1, 
		[CustomerID] = [i].[CustomerID] 
	FROM [ServiceTickets_tracking] [side] 
	JOIN INSERTED AS [i] ON [side].[ServiceTicketID] = [i].[ServiceTicketID]



GO
ALTER TABLE [dbo].[ServiceTickets] ENABLE TRIGGER [ServiceTickets_update_trigger]
GO
