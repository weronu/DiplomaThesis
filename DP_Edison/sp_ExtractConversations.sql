USE [ThesisImportDatabase]
GO
/****** Object:  StoredProcedure [dbo].[sp_ExtractConversations]    Script Date: 26.4.2018 21:05:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[sp_ExtractConversations]
AS
DECLARE @ConversationsTemp Table (
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
FROM @ConversationsTemp c
INNER JOIN EmailMessages em on c.EmailMessageId = em.Id
GROUP BY c.ConversationId
HAVING COUNT(DISTINCT em.SenderId) >= 2 AND COUNT(EmailMessageId) >= 2
) dup
	ON dup.ConversationId = c.ConversationId
INNER JOIN EmailMessages em on em.Id = c.EmailMessageId
ORDER BY c.ConversationId 

