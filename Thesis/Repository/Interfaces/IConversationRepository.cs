using System;
using System.Collections.Generic;
using Domain.DomainClasses;

namespace Repository.MSSQL.Interfaces
{
    public interface IConversationRepository
    {
        HashSet<ConversationEmails> ExtractConversationsFromDatabase();
        DateTime GetDateOfFirstConversation();
        DateTime GetDateOfLastConversation();
        HashSet<ConversationEmails> ExtractConversationsFromDatabase(DateTime? fromDate, DateTime? toDate);
    }
}
