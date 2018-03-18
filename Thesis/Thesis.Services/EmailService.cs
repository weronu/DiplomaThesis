using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using ActiveUp.Net.Mail;
using Domain.DTOs;
using Limilabs.Client.IMAP;
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

        public HashSet<EmailXML> DownloadEmailMessagesFromEmailAccount(EmailDownloadDto emailDownloadDto)
        {
            if (emailDownloadDto.ServerAddress == "pop.gmail.com" && emailDownloadDto.UseSSL == false)
            {
                throw new Exception("Connection to GMAIL requires SSL connection.");
            }

            HashSet<EmailXML> emails = new HashSet<EmailXML>();

            try
            {
               
                using (Imap imap = new Imap())
                {
                    imap.Connect(emailDownloadDto.ServerAddress, emailDownloadDto.Port, emailDownloadDto.UseSSL);
                    imap.Login(emailDownloadDto.Username, emailDownloadDto.Password);

                    MailBuilder builder = new MailBuilder();

                    imap.SelectInbox();

                    List<long> emailUIDs = imap.GetAll().Take(10).ToList();

                    foreach (long uid in emailUIDs)
                    //Parallel.ForEach(emailUIDs, new ParallelOptions { MaxDegreeOfParallelism = 4 }, (uid) =>
                    {
                        IMail emailRaw = builder.CreateFromEml(imap.GetMessageByUID(uid));

                        EmailXML emailXml = new EmailXML()
                        {
                            InReplyToId = emailRaw.InReplyTo,
                            MessageId = emailRaw.MessageID,
                            Sender = emailRaw.Sender.Address,
                            RawSender = emailRaw.Sender.Name,
                            Sent = emailRaw.Date,
                            Subject = emailRaw.Subject
                        };

                        emails.Add(emailXml);
                    }
                    imap.Close();
                }

                return emails;
            }
            catch (Exception e)
            {
                throw new Exception($"Download failed with an error: {e}");
            }
        }

        public void CreateEmailXMLFile(HashSet<EmailXML> emails)
        {
            using (XmlWriter writer = XmlWriter.Create("D:/emails.xml"))
            {
                writer.WriteStartElement("Messages");
                foreach (EmailXML emailXml in emails)
                {
                    writer.WriteStartElement("Message");

                    writer.WriteAttributeString("MessageId", emailXml.MessageId);
                    writer.WriteAttributeString("InReplyToId", emailXml.InReplyToId);
                    writer.WriteAttributeString("Sender", emailXml.Sender);
                    writer.WriteAttributeString("RawSender", emailXml.RawSender);
                    if (emailXml.Sent != null) writer.WriteAttributeString("Sent", emailXml.Sent.Value.Date.ToString("yyyy-MM-dd HH:mm:ss"));
                    writer.WriteAttributeString("Subject", emailXml.Subject);

                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.Flush();
            }
        }
    }
}
