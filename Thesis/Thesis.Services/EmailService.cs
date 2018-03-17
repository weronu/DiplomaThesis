using System;
using System.Globalization;
using Limilabs.Client.POP3;
using Limilabs.Mail;
using Repository.MSSQL.Interfaces;
using Thesis.Services.Interfaces;

namespace Thesis.Services
{
    public class EmailService : ServiceBase, IEmailService
    {
        public EmailService(IUnitOfWorkFactory unitOfWorkFactory) : base(unitOfWorkFactory)
        {
        }

        public void DownloadEmailMessagesFromEmailAccount(string emailAccount, string password)
        {
            try
            {
                using (Pop3 pop3 = new Pop3())
                {
                    pop3.Connect("pop3.example.com");  // or ConnectSSL for SSL 
                    pop3.Login("user", "password");

                    // Receive all messages and display the subject 
                    MailBuilder builder = new MailBuilder();
                    foreach (string uid in pop3.GetAll())
                    {
                        IMail email = builder.CreateFromEml(
                            pop3.GetMessageByUID(uid));

                        Console.WriteLine(email.Subject);
                        Console.WriteLine(email.Text);
                    }
                    pop3.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Download failed with an error: {e}");
            }
        }
    }
}
