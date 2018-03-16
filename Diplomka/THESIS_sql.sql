
--DROP TABLE #TempUser
--DROP TABLE #WorkingTable
--DROP TABLE #User
--DROP TABLE #EmailMessage
--DROP TABLE #RecipientTemp


CREATE TABLE #WorkingTable
(Data XML)


INSERT INTO #WorkingTable
SELECT * FROM OPENROWSET (BULK 'C:\Users\veronika.uhrova\Desktop\Diplomka\teamnet_anonymized.xml', SINGLE_BLOB) AS data


DECLARE @ImportData AS XML, @DocumentId AS INT
SELECT @ImportData = Data FROM #WorkingTable

EXEC sp_xml_preparedocument @DocumentId OUTPUT, @ImportData



CREATE TABLE #TempUser(Email NVARCHAR(254));

CREATE TABLE #User(
					UserId INT NOT NULL IDENTITY(1, 1) PRIMARY KEY,
					Email NVARCHAR(254)
				  );

CREATE TABLE #EmailMessage  
					   (
					   EmailMessage INT NOT NULL IDENTITY(1, 1) PRIMARY KEY,
					   MessageId UNIQUEIDENTIFIER,
					   SenderId INT,
					   InReplyTo UNIQUEIDENTIFIER,
					   Subject NVARCHAR(254),
                       SentDatetime DATETIME
					   );

CREATE TABLE #RecipientTemp
					   (
				       Position INT,
                       RecipientEmail nvarchar(150)
					   );



  


INSERT INTO #TempUser (Email)
   SELECT col.value(N'(@Sender)', N'NVARCHAR(400)') AS [Email]
   FROM   @ImportData.nodes(N'/Messages/Message') tab(col)

INSERT INTO #TempUser (Email)
   SELECT col.value(N'(Recipient/text())[1]', 'NVARCHAR(400)') AS [Email]
   FROM   @ImportData.nodes(N'/Messages/Message') tab(col)

INSERT INTO #User(Email)
SELECT DISTINCT Email from #TempUser

--SELECT * FROM #User

INSERT INTO #EmailMessage (MessageId, SenderId, InReplyTo, Subject, SentDatetime)
     SELECT  TOP 100
            Messages.value(N'(@MessageId)', 'UNIQUEIDENTIFIER'),
			(SELECT UserId FROM #User WHERE Email = Messages.value(N'(@Sender)', N'NVARCHAR(400)')),
			Messages.value(N'(@InReplyTo)', 'UNIQUEIDENTIFIER'),
			Messages.value(N'(@Subject)', 'Nvarchar(254)'),
            Messages.value(N'(@Sent)', 'DATETIME')	   
 FROM @ImportData.nodes(N'/Messages/Message') tab(Messages)


INSERT INTO #RecipientTemp (Position, RecipientEmail)
     SELECT  TOP 100
	 DENSE_RANK() OVER (ORDER BY Messages) AS Position,
	 (Recipients.value('(.)', 'varchar(max)'))
FROM @ImportData.nodes(N'/Messages/Message') tab(Messages)
CROSS APPLY
    Messages.nodes(N'Recipient') AS tbl(Recipients)


 SELECT top 100* from #EmailMessage
 SELECT top 100* from #RecipientTemp


EXEC sp_xml_removedocument @DocumentID


