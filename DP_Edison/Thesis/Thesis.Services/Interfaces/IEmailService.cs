using System.Collections.Generic;
using Domain.DTOs;
using Thesis.Services.ResponseTypes;

namespace Thesis.Services.Interfaces
{
    public interface IEmailService
    {
        FetchListServiceResponse<EmailXML> DownloadEmailMessagesFromEmailAccount(EmailDownloadDto emailDownloadDto);
    }
}
