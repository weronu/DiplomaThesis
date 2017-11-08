using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.DomainClasses
{
    public class ConversationEmails
    {
        public int ConverationId { get; set; }
        public List<EmailMessage> Emails { get; set; }
        public DateTime ConversationStartDate
        {
            get { return Emails.Min(x => x.Sent); }
        }
    }
}
