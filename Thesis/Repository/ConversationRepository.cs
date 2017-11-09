using System.Collections.Generic;
using System.Linq;
using Domain.DomainClasses;
using Repository.MSSQL.Interfaces;

namespace Repository.MSSQL
{
    public class ConversationRepository : RepositoryBase, IConversationRepository
    {
        private readonly ThesisDbContext _context;
        public ConversationRepository(ThesisDbContext context) : base(context)
        {
            this._context = context;
        }

        public HashSet<ConversationEmails> ExtractConversationsFromDatabase()
        {
            return new HashSet<ConversationEmails>(from conversation in _context.Conversations
                group conversation.EmailMessage by conversation.ConversationId
                into grp
                select new ConversationEmails()
                {
                    ConverationId = grp.Key,
                    Emails = grp.ToList()
                });
        }
    }
}
