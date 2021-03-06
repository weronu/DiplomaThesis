USE [ThesisTiborNetwork]
  SELECT count(distinct u.Id)
  FROM users u
  INNER JOIN EmailMessages em on u.Id = em.SenderId
  INNER JOIN Conversations c on c.EmailMessageId = em.Id
  HAVING COUNT(u.Id) >= 1


  SELECT count(DISTINCT em.Id)
  FROM EmailMessages em
  INNER JOIN Users u on u.Id = em.SenderId
  INNER JOIN UserEmails ue on ue.UserId = u.Id
  WHERE ue.Email LIKE '%globallogic.com' OR ue.Email LIKE '%rec-global.com'

  SELECT COUNT(DISTINCT c.Id)
  FROM Conversations c 
  INNER JOIN EmailMessages em ON c.EmailMessageId = em.Id
  INNER JOIN UserEmails ue on ue.UserId = em.SenderId
  WHERE ue.Email LIKE '%globallogic.com' OR ue.Email LIKE '%rec-global.com'