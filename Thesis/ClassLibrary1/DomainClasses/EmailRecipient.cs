using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DomainClasses
{
    public class EmailRecipient : DomainBase
    {
        public int? RecipientId { get; set; }
        public int EmailMessageId { get; set; }

        [MaxLength(10)]
        [Column(TypeName = "VARCHAR")]
        public string RecipientType { get; set; }

        public virtual User Recipient { get; set; }
        public virtual EmailMessage EmailMessage { get; set; }


    }
}