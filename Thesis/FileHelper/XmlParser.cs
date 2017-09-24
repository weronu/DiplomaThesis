using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Common;
using Domain.DomainClasses;

namespace FileHelper
{
    public class XmlParser
    {
        public HashSet<User> GetUsersFromXML(string path)
        {
            HashSet<User> users = new HashSet<User>();
            try
            {
                using (XmlReader reader = XmlReader.Create(path))
                {
                    while (reader.Read())
                    {
                        if (reader.HasAttributes)
                        {
                            string sender = reader.GetAttribute("Sender");
                            if (!string.IsNullOrEmpty(sender))
                            {
                                User user = new User { Email = sender };

                                if (users.All(x => x.Email != sender))
                                {
                                    users.Add(user);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return users;
        }

        public HashSet<EmailMessage> GetEmailsFromXML(string path)
        {
            HashSet<EmailMessage> emails = new HashSet<EmailMessage>();
            try
            {

                string xmlStr = File.ReadAllText(path);

                XElement str = XElement.Parse(xmlStr);

                HashSet<XElement> result = new HashSet<XElement>(str.Elements("Message").ToList());

                foreach (XElement element in result)
                {
                    EmailMessage emailMessage = new EmailMessage();

                    // mesageId
                    string messageId = element.Attribute("MessageId")?.Value;
                    if (messageId != null) emailMessage.MessageId = Guid.Parse(messageId);

                    // sender
                    XAttribute senderEmail = element.Attribute("Sender");
                    emailMessage.SenderEmail = senderEmail?.Value;

                    // sent
                    XAttribute sent = element.Attribute("Sent");
                    emailMessage.Sent = DateTime.Parse(sent?.Value);

                    // subject
                    XAttribute subject = element.Attribute("Subject");
                    emailMessage.Subject = subject?.Value;

                    ////// TODO
                    //HashSet<User> usersFromXml = GetUsersFromXML(path);
                    //emailMessage.Recipients = new List<EmailRecipient>();
                    //// recipients
                    //List<XElement> recipientElements = element.Elements("Recipient").ToList();
                    //foreach (XElement recipientElement in recipientElements)
                    //{
                    //    emailMessage.Recipients.Add(new EmailRecipient()
                    //    {
                    //        RecipientEmail = recipientElement.Value,
                    //        RecipientType = recipientElement.Attribute("Type")?.Value
                    //    });
                    //}

                    // in reply to
                    string inReplyTo = element.Attribute("InReplyTo")?.Value;
                    if (inReplyTo != null) emailMessage.InReplyToId = Guid.Parse(inReplyTo);

                    emails.Add(emailMessage);
                }
            }
            catch (ThesisException)
            {
                throw new ThesisException("There is a problem with parsing a document.");
            }
            return emails;
        }


    }
}
