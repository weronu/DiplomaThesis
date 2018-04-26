using System;

namespace Domain.DTOs
{
    public class EmailXML
    {
        public string MessageId { get; set; }
        public string Sender { get; set; }
        public string RawSender { get; set; }
        public string InReplyToId { get; set; }
        public string Subject { get; set; }
        public DateTime? Sent { get; set; }
    }
}

