using System.ComponentModel.DataAnnotations;

namespace Thesis.Web.Models
{
    public class EmailDownloadViewModel
    {
        public string Email { get; set; }

        public string Username { get; set; }

        [Display(Name = "Server address")]
        public string ServerAddress { get; set; }

        [Display(Name = "Use secure connection(SSL)")]
        public bool UseSSL { get; set; }

        public string Password { get; set; }

        public int Port { get; set; }

    }
}