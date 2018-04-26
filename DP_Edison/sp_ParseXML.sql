USE [ThesisImportDatabase]
GO
/****** Object:  StoredProcedure [dbo].[sp_ParseXML]    Script Date: 26.4.2018 21:04:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[sp_ParseXML] (@file NVARCHAR(1000))

AS
DECLARE @WorkingTable Table
(Data XML);


INSERT INTO @WorkingTable
exec ('SELECT * FROM OPENROWSET (BULK ''' + @file + ''' , SINGLE_BLOB) AS data')

--INSERT INTO @WorkingTable
--SELECT * FROM OPENROWSET (BULK 'C:\Users\veronika.uhrova\Desktop\Diplomka\maily\vuhrova.xml', SINGLE_BLOB) AS data

DECLARE @ImportData AS XML, @DocumentId AS INT
EXEC sp_xml_preparedocument @DocumentId OUTPUT, @ImportData
SELECT @ImportData = Data FROM @WorkingTable


DECLARE @TempUser Table (Email NVARCHAR(255), 
						[Name] NVARCHAR(255));

INSERT INTO @TempUser (Email, [Name])
   SELECT DISTINCT col.value(N'(@Sender)', N'NVARCHAR(400)') AS [Email],
   col.value(N'(@RawSender)', N'NVARCHAR(400)') AS [Name]
   FROM   @ImportData.nodes(N'/Messages/Message') tab(col)

INSERT INTO dbo.Users([Name])
SELECT DISTINCT concat(SUBSTRING(SUBSTRING(t.Email, 0, CHARINDEX('@', t.Email)), 0, CHARINDEX('.', t.Email)) + ' ',SUBSTRING(SUBSTRING(t.Email, 0, CHARINDEX('@', t.Email)), CHARINDEX('.', t.Email) + 1, LEN(t.Email) - CHARINDEX('.',t.Email))) Collate SQL_Latin1_General_CP1253_CI_AI as NameEmail
FROM @TempUser t
 WHERE t.[Name] NOT LIKE '%(%' AND t.[Name] NOT LIKE '%[%' AND t.Email NOT LIKE '%repl%'AND t.Email NOT LIKE '%atlassian%' AND t.[Name] != ''

INSERT INTO UserEmails(UserId, Email)
SELECT DISTINCT
u.Id,
t.Email
FROM @TempUser t
INNER JOIN Users u on (concat(SUBSTRING(SUBSTRING(t.Email, 0, CHARINDEX('@', t.Email)), 0, CHARINDEX('.',t.Email)) + ' ',SUBSTRING(SUBSTRING(t.Email, 0, CHARINDEX('@', t.Email)), CHARINDEX('.',t.Email) + 1, LEN(t.Email) - CHARINDEX('.',t.Email))) Collate SQL_Latin1_General_CP1253_CI_AI) = u.[Name]

UPDATE u
SET u.RawSenderName = tt.[Name]
FROM Users u
INNER JOIN UserEmails ue on ue.UserId = u.Id
INNER JOIN @TempUser tt ON tt.Email = ue.Email
WHERE tt.[Name] != ''


DECLARE @TMP_Messages TABLE (
COL_MessageId INT NOT NULL,
MessageId NVARCHAR(200), 
SenderId int,
InReplyToId NVARCHAR(200), 
Subject nvarchar(254),
Sent datetime,
COL_XMLPosition int not null
)

print'debug'

INSERT dbo.EmailMessages 
	(
	MessageId, 
	SenderId,
	InReplyToId, 
	Subject,
	Sent,
	XMLPosition
	)
OUTPUT
	INSERTED.*
INTO
	@TMP_Messages
SELECT
	EmailMessages.value('@MessageId', 'NVARCHAR(200)'),
	(SELECT UserId FROM dbo.UserEmails WHERE Email = EmailMessages.value('@Sender', N'NVARCHAR(400)')),
	CASE 
	WHEN EmailMessages.value('@InReplyTo', 'NVARCHAR(200)') IS NULL AND EmailMessages.value('@InReplyToId', 'NVARCHAR(200)') IS NOT NULL THEN EmailMessages.value('@InReplyToId', 'NVARCHAR(200)')
	WHEN EmailMessages.value('@InReplyToId', 'NVARCHAR(200)') IS NULL AND EmailMessages.value('@InReplyTo', 'NVARCHAR(200)') IS NOT NULL THEN EmailMessages.value('@InReplyTo', 'NVARCHAR(200)')	
	ELSE EmailMessages.value('@InReplyToId', 'NVARCHAR(200)') END,
	EmailMessages.value('@Subject', 'NVARCHAR(254)'),
    EmailMessages.value('@Sent', 'DATETIME'),
	 row_number() OVER (ORDER BY EmailMessages) AS EmailPosition
FROM @ImportData.nodes(N'/Messages/Message') tab(EmailMessages)	
INNER JOIN UserEmails u on u.Email =  EmailMessages.value('@Sender', N'NVARCHAR(400)')


--INSERT dbo.EmailRecipients
--(
--	RecipientId,
--	EmailMessageId,
--	RecipientType
--)
--SELECT 
--	T0.UserId,
--	t1.COL_MessageId,
--	T0.RecipientType
--FROM
--	(
--		SELECT 
--			 DENSE_RANK() OVER (ORDER BY EmailMessages) AS Position
--			,(SELECT Id FROM dbo.Users WHERE Email = Recipients.value('(.)', 'varchar(254)')) as UserId
--			,Recipients.value('@Type', 'varchar(254)') as RecipientType
--		FROM
--			@ImportData.nodes(N'/Messages/Message') tab(EmailMessages)	
--		OUTER APPLY
--			EmailMessages.nodes(N'Recipient') AS tbl(Recipients)
--	) as T0
--JOIN 
--	@TMP_Messages T1
--ON 
--	t0.Position = t1.COL_XMLPosition


