ALTER PROCEDURE SP_ParseXML(@filePath nvarchar(254))
AS
DECLARE @WorkingTable Table
(Data XML);


INSERT INTO @WorkingTable
SELECT * FROM OPENROWSET (BULK 'C:\Users\veronika.uhrova\Desktop\Diplomka\teamnet_anonymized.xml', SINGLE_BLOB) AS data
DECLARE @ImportData AS XML, @DocumentId AS INT
EXEC sp_xml_preparedocument @DocumentId OUTPUT, @ImportData
SELECT @ImportData = Data FROM @WorkingTable


DECLARE @TempUser Table (Email NVARCHAR(254));

INSERT INTO @TempUser (Email)
   SELECT col.value(N'(@Sender)', N'NVARCHAR(400)') AS [Email]
   FROM   @ImportData.nodes(N'/Messages/Message') tab(col)

INSERT INTO @TempUser (Email)
   SELECT col.value(N'(.)', 'NVARCHAR(400)') AS [Email]
   FROM   @ImportData.nodes(N'/Messages/Message/Recipient') tab(col)

INSERT INTO dbo.Users(Email)
SELECT DISTINCT Email from @TempUser
WHERE Email IS NOT NULL

DECLARE @TMP_Messages TABLE (
COL_MessageId INT NOT NULL,
MessageId UNIQUEIDENTIFIER, 
SenderId int,
InReplyToId UNIQUEIDENTIFIER, 
Subject nvarchar(254),
Sent datetime,
COL_XMLPosition int not null
)

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
	EmailMessages.value('@MessageId', 'UNIQUEIDENTIFIER'),
	(SELECT Id FROM dbo.Users WHERE Email = EmailMessages.value('@Sender', N'NVARCHAR(400)')),
	EmailMessages.value('@InReplyTo', 'UNIQUEIDENTIFIER'),
	EmailMessages.value('@Subject', 'Nvarchar(254)'),
    EmailMessages.value('@Sent', 'DATETIME'),
	 row_number() OVER (ORDER BY EmailMessages) AS EmailPosition
FROM @ImportData.nodes(N'/Messages/Message') tab(EmailMessages)	


INSERT dbo.EmailRecipients
(
	UserId,
	EmailMessageId,
	RecipientType
)
SELECT 
	T0.UserId,
	t1.COL_MessageId,
	T0.RecipientType
FROM
	(
		SELECT 
			 DENSE_RANK() OVER (ORDER BY EmailMessages) AS Position
			,(SELECT Id FROM dbo.Users WHERE Email = Recipients.value('(.)', 'varchar(254)')) as UserId
			,Recipients.value('@Type', 'varchar(254)') as RecipientType
		FROM
			@ImportData.nodes(N'/Messages/Message') tab(EmailMessages)	
		OUTER APPLY
			EmailMessages.nodes(N'Recipient') AS tbl(Recipients)
	) as T0
JOIN 
	@TMP_Messages T1
ON 
	t0.Position = t1.COL_XMLPosition
