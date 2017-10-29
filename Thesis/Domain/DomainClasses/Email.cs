using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DomainClasses
{
    public class EmailMessage : DomainBase
    {
        public Guid? MessageId { get; set; }
        public int? SenderId { get; set; }
        public Guid? InReplyToId { get; set; }

        [MaxLength(254)]
        [Column(TypeName = "VARCHAR")]
        public string Subject { get; set; }
        public DateTime Sent { get; set; }
        public int XMLPosition { get; set; }


        public virtual User Sender { get; set; }
        public virtual List<EmailRecipient> Recipients { get; set; }
    }
}
