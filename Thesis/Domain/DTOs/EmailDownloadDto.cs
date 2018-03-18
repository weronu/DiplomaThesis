namespace Domain.DTOs
{
    public class EmailDownloadDto
    {
        public string Email { get; set; }

        public string Username { get; set; }

        public string ServerAddress { get; set; }

        public bool UseSSL { get; set; }

        public string Password { get; set; }

        public int Port { get; set; }
    }
}