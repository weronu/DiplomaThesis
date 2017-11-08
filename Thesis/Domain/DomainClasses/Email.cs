using System;
using System.Collections.Generic;

namespace Domain.DomainClasses
{
    public class EmailMessage : DomainBase
    {
        public Guid? MessageId { get; set; }
        public int? SenderId { get; set; }
        public Guid? InReplyToId { get; set; }
        public string Subject { get; set; }
        public DateTime Sent { get; set; }
        public int XMLPosition { get; set; }


        public virtual User Sender { get; set; }
        public virtual List<EmailRecipient> Recipients { get; set; }
    }
}
