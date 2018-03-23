﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Domain.DTOs;
using Limilabs.Client.IMAP;
using Limilabs.Mail;
using Repository.MSSQL.Interfaces;
using Thesis.Services.Interfaces;
using Thesis.Services.ResponseTypes;

namespace Thesis.Services
{
    public class EmailService : ServiceBase, IEmailService
    {
        public EmailService(IUnitOfWorkFactory unitOfWorkFactory) : base(unitOfWorkFactory)
        {
        }

        public FetchListServiceResponse<EmailXML> DownloadEmailMessagesFromEmailAccount(EmailDownloadDto emailDownloadDto)
        {
            FetchListServiceResponse<EmailXML> response = new FetchListServiceResponse<EmailXML>();
            HashSet<EmailXML> emails = new HashSet<EmailXML>();

            if (emailDownloadDto.ServerAddress == "pop.gmail.com" && emailDownloadDto.UseSSL == false)
            {
                response.AddError("Connection to GMAIL requires SSL connection.");
            }

            try
            {
                using (Imap imap = new Imap())
                {
                    imap.Connect(emailDownloadDto.ServerAddress, emailDownloadDto.Port, emailDownloadDto.UseSSL);
                    imap.Login(emailDownloadDto.Username, emailDownloadDto.Password);

                    MailBuilder builder = new MailBuilder();

                    imap.SelectInbox();

                    List<long> emailUIDs = imap.GetAll().Take(100).ToList();

                    foreach (long uid in emailUIDs)
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

                response.AddSuccessMessage("Emails were successfully downloaded.");
                response.Items = emails;
                response.Succeeded = true;

                CreateEmailXMLFile(emails, response);
            }
            catch (Exception e)
            {
                response.Succeeded = false;
                response.AddError($"Download failed with an error: {e.Message}");

                if (e.InnerException != null)
                {
                    response.AddError($"Additional error: {e.InnerException.Message}");
                }
            }

            return response;
        }

        private static void CreateEmailXMLFile(HashSet<EmailXML> emails, FetchListServiceResponse<EmailXML> response)
        {
            try
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
            catch (Exception e)
            {
                response.Succeeded = false;
                response.AddError($"Creating XML file was not successful: {e.Message}");

                if (e.InnerException != null)
                {
                    response.AddError($"Additional error: {e.InnerException.Message}");
                }
            }
        }
    }
}
