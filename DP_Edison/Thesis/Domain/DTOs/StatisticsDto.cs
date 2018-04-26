namespace Domain.DTOs
{
    public class NetworkStatisticsDto
    {
        public int NumberOfEmails { get; set; }
        public int NumberOfConversations { get; set; }
        public int NumberOfUsers { get; set; }
        public string PeekHour { get; set; }
        public int TheBiggestNumberOfEmailsInConversation { get; set; }
        public string BiggestEmailSender { get; set; }

    }
}
