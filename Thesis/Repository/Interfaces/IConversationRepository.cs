using System.Collections.Generic;
using Domain.DomainClasses;

namespace Repository.MSSQL.Interfaces
{
    public interface IConversationRepository
    {
        HashSet<ConversationEmails> ExtractConversationsFromDatabase();
    }
}
