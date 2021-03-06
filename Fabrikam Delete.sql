use ClientDB

begin try
	DROP PROCEDURE [dbo].[ServiceTickets_bulkdelete];
	DROP PROCEDURE [dbo].[ServiceTickets_bulkinsert];
	DROP PROCEDURE [dbo].[ServiceTickets_bulkupdate];
	DROP PROCEDURE [dbo].[ServiceTickets_delete];
	DROP PROCEDURE [dbo].[ServiceTickets_insert];
	DROP PROCEDURE [dbo].[ServiceTickets_insertmetadata];
	DROP PROCEDURE [dbo].[ServiceTickets_selectchanges];
	DROP PROCEDURE [dbo].[ServiceTickets_selectrow];
	DROP PROCEDURE [dbo].[ServiceTickets_update];
	DROP PROCEDURE [dbo].[ServiceTickets_updatemetadata];
end try
begin catch
end catch


if (exists (select * from sys.tables where name = 'ServiceTickets_tracking'))
begin
	print 'drop table ServiceTickets_tracking'
	Drop table ServiceTickets_tracking
end


if (exists (select * from sys.tables where name = 'ServiceTickets'))
begin
	print 'drop table ServiceTickets'
	Drop table ServiceTickets;
end

if (exists (select * from sys.tables where name = 'scope_info'))
begin
	print 'drop table scope_info'
	Drop table scope_info
end

if (exists (select * from sys.table_types where name = 'ServiceTickets_BulkType'))
begin
	DROP TYPE [dbo].[ServiceTickets_BulkType]
end

use ServerDB

begin try
	DROP PROCEDURE [dbo].[ServiceTickets_bulkdelete];
	DROP PROCEDURE [dbo].[ServiceTickets_bulkinsert];
	DROP PROCEDURE [dbo].[ServiceTickets_bulkupdate];
	DROP PROCEDURE [dbo].[ServiceTickets_delete];
	DROP PROCEDURE [dbo].[ServiceTickets_insert];
	DROP PROCEDURE [dbo].[ServiceTickets_insertmetadata];
	DROP PROCEDURE [dbo].[ServiceTickets_selectchanges];
	DROP PROCEDURE [dbo].[ServiceTickets_selectrow];
	DROP PROCEDURE [dbo].[ServiceTickets_update];
	DROP PROCEDURE [dbo].[ServiceTickets_updatemetadata];
end try
begin catch
end catch

begin try
	DROP TRIGGER [dbo].[ServiceTickets_delete_trigger];
	DROP TRIGGER [dbo].[ServiceTickets_insert_trigger];
	DROP TRIGGER [dbo].[ServiceTickets_update_trigger];
end try
begin catch
end catch


if (exists (select * from sys.tables where name = 'ServiceTickets_tracking'))
begin
	Drop table ServiceTickets_tracking
end


if (exists (select * from sys.tables where name = 'ServiceTickets'))
begin
	Truncate table ServiceTickets
end

if (exists (select * from sys.tables where name = 'scope_info'))
begin
	Drop table scope_info
end

if (exists (select * from sys.table_types where name = 'ServiceTickets_BulkType'))
begin
	DROP TYPE [dbo].[ServiceTickets_BulkType]
end

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
