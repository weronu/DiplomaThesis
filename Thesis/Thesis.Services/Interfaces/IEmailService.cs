using System.Collections.Generic;
using Domain.DTOs;

namespace Thesis.Services.Interfaces
{
    public interface IEmailService
    {
        HashSet<EmailXML> DownloadEmailMessagesFromEmailAccount(EmailDownloadDto emailDownloadDto);
        void CreateEmailXMLFile(HashSet<EmailXML> emails);
    }
}
