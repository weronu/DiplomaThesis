using System;
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

        public HashSet<ConversationEmails> ExtractConversationsFromDatabase(DateTime? fromDate, DateTime? toDate)
        {
            return new HashSet<ConversationEmails>(from conversation in _context.Conversations
                where conversation.EmailMessage.Sent >= fromDate && conversation.EmailMessage.Sent <= toDate
                group conversation.EmailMessage by conversation.ConversationId
                into grp
                select new ConversationEmails()
                {
                    ConverationId = grp.Key,
                    Emails = grp.ToList()
                });
        }

        public DateTime GetDateOfFirstConversation()
        {
            DateTime dateTime = _context.Conversations.Min(x => x.EmailMessage.Sent);
            return dateTime;
        }

        public DateTime GetDateOfLastConversation()
        {
            DateTime dateTime = _context.Conversations.Max(x => x.EmailMessage.Sent);
            return dateTime;
        }
    }
}
