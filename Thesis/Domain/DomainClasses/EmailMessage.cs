using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.DomainClasses
{
    public class EmailMessage : DomainBase
    {
        [MaxLength(70)]
        public string MessageId { get; set; }
        public int? SenderId { get; set; }
        [MaxLength(70)]
        public string InReplyToId { get; set; }
        public string Subject { get; set; }
        public DateTime Sent { get; set; }
        public int XMLPosition { get; set; }


        public virtual User Sender { get; set; }
        public virtual List<EmailRecipient> Recipients { get; set; }
    }
}
