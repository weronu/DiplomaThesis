namespace Domain.DomainClasses
{
    public class Conversation : DomainBase
    {
        public int ConversationId { get; set; }
        public int EmailMessageId { get; set; }

        public virtual EmailMessage EmailMessage { get; set; }
    }
}
