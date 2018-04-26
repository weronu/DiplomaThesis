USE [ThesisImportDatabase]
GO
/****** Object:  StoredProcedure [dbo].[sp_ClearDatabaseData]    Script Date: 26.4.2018 21:05:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[sp_ClearDatabaseData] 

AS

BEGIN

DELETE FROM Conversations

DELETE FROM EmailMessages

DELETE FROM UserEmails

DELETE FROM Users


DBCC CHECKIDENT ('[Conversations]', RESEED, 0)

DBCC CHECKIDENT ('[Users]', RESEED, 0)

DBCC CHECKIDENT ('[UserEmails]', RESEED, 0)

DBCC CHECKIDENT ('[emailMessages]', RESEED, 0)


END