CREATE Table Conversations
(
Id INT IDENTITY(1,1),
ConversationId INT NOT NULL,
EmailMessageId INT NOT NULL
)

DELETE from EmailRecipients WHERE RecipientId IS NULL
DELETE FROM EmailMessages WHERE Id NOT IN (SELECT EmailMessageId FROM EmailRecipients)
DELETE FROM Users WHERE Id NOT IN (SELECT SenderId FROM EmailMessages) AND Id NOT IN (SELECT RecipientId FROM EmailRecipients)

Declare @ConversationsTemp Table (
ConversationId INT NOT NULL,
EmailMessageId INT NOT NULL
)

INSERT INTO @ConversationsTemp
SELECT DISTINCT * from (
SELECT rank, list.Email 
FROM
	(
	SELECT DENSE_RANK() OVER   
		(ORDER BY em.Id) as rank, em.Id AS Email, rm.Id as Reply
	FROM EmailMessages em
	LEFT JOIN EmailMessages rm ON em.MessageId = rm.InReplyToId
	WHERE rm.Id is NOT NULL
	--ORDER by em.Id
	) list
UNION ALL
SELECT rank, list.Reply 
	FROM
	(
	SELECT DENSE_RANK() OVER   
		(ORDER BY em.Id) as rank, em.Id AS Email, rm.Id as Reply
	FROM EmailMessages em
	LEFT JOIN EmailMessages rm ON em.MessageId = rm.InReplyToId
	WHERE rm.Id is NOT NULL
	--ORDER by em.Id
	) list
) list2
ORDER by list2.rank

insert INTO Conversations 
SELECT c.ConversationId, c.EmailMessageId
FROM @ConversationsTemp c
INNER JOIN 
(SELECT c.ConversationId, COUNT(DISTINCT em.SenderId) as countOfSenders, COUNT(EmailMessageId) as emails
FROM Conversations c
INNER JOIN EmailMessages em on c.EmailMessageId = em.Id
GROUP BY c.ConversationId
HAVING COUNT(DISTINCT em.SenderId) >= 2 AND COUNT(EmailMessageId) >= 2
order by countOfSenders desc
) dup
	ON dup.ConversationId = c.ConversationId
INNER JOIN EmailMessages em on em.Id = c.EmailMessageId
ORDER BY c.ConversationId 


SELECT * FROM EmailMessages WHERE id = 115995


SELECT * FROM EmailMessages WHERE InReplyToId = 'A875B173-160C-4266-93E9-8E3BCAA7D869'

SELECT c.ConversationId, c.EmailMessageId, em.SenderId  
FROM Conversations c
INNER JOIN EmailMessages em on c.EmailMessageId = em.Id
WHERE ConversationId = 22457

--SELECT c.ConversationId, c.EmailMessageId
--FROM Conversations c
--INNER JOIN 
--(SELECT c.ConversationId, COUNT(DISTINCT em.SenderId) as countOfSenders, COUNT(EmailMessageId) as emails
--FROM Conversations c
--INNER JOIN EmailMessages em on c.EmailMessageId = em.Id
--GROUP BY c.ConversationId
--HAVING COUNT(DISTINCT em.SenderId) >= 2 AND COUNT(EmailMessageId) >= 2
--) dup
--	ON dup.ConversationId = c.ConversationId
--INNER JOIN EmailMessages em on em.Id = c.EmailMessageId
--ORDER BY c.ConversationId 


SELECT c.ConversationId, s1.Id, s2.Id
FROM Conversations c 
INNER JOIN EmailMessages em on c.EmailMessageId = em.Id
LEFT JOIN Users s1 on s1.Id = em.SenderId
LEFT JOIN Users s2 on s2.Id = em.SenderId
--WHERE s1.Id <> s2.Id
ORDER BY c.ConversationId


	USE [Repository.MSSQL.ThesisDbContext]
;with cteAllColumns as (
    select em.SenderId as col
        from Conversations c
		INNER JOIN EmailMessages em on c.EmailMessageId = em.Id
		WHERE c.ConversationId = 5
)

select distinct c1.col, c2.col 
    from cteAllColumns c1 
        cross join cteAllColumns c2 
    where c1.col < c2.col
    order by c1.col, c2.col


USE ThesisDatabase
SELECT * FROM Conversations WHERE ConversationId = 36
SELECT * FROM EmailMessages WHERE Id in (471, 472, 473, 475, 480, 482)
SELECT * FROM Users WHERE Id = 264
SELECT * FROM EmailRecipients

DELETE FROM Conversations
DELETE FROM EmailRecipients
DELETE FROM EmailMessages
DELETE FROM Users

DROP TABLE EmailRecipients
DROP TABLE Conversations
DROP TABLE EmailMessages
DROP TABLE Users


DBCC CHECKIDENT (conversations, RESEED, 0)
DBCC CHECKIDENT (EmailRecipients, RESEED, 0)
DBCC CHECKIDENT (EmailMessages, RESEED, 0)
DBCC CHECKIDENT (Users, RESEED, 0)


