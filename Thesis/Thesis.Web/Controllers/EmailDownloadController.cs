using System.Collections.Generic;
using System.Web.Mvc;
using Domain.DTOs;
using Thesis.Services.Interfaces;
using Thesis.Web.Models;

namespace Thesis.Web.Controllers
{
    public class EmailDownloadController : Controller
    {
        private readonly IEmailService _emailService;

        public EmailDownloadController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SubmitDownload(EmailDownloadViewModel model)
        {
            EmailDownloadDto emailDownloadDto = new EmailDownloadDto()
            {
                Email = model.Email,
                Password = model.Password,
                Port = model.Port,
                ServerAddress = model.ServerAddress,
                Username = model.Username,
                UseSSL = model.UseSSL
            };

            HashSet<EmailXML> downloadedEmails = _emailService.DownloadEmailMessagesFromEmailAccount(emailDownloadDto);

            _emailService.CreateEmailXMLFile(downloadedEmails);


            return View("Index", model);
        }
    }
}