using System;
using System.Web.Mvc;
using Domain.DTOs;
using Thesis.Services.Interfaces;
using Thesis.Services.ResponseTypes;
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
            try
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

                FetchListServiceResponse<EmailXML> downloadedEmails = _emailService.DownloadEmailMessagesFromEmailAccount(emailDownloadDto);
                this.AddToastMessage("Success", "Emails were downloaded.", ToastType.Success);
            }
            catch (Exception e)
            {
                this.AddToastMessage("Error", e.Message, ToastType.Error);
            }
            
            return View("Index", model);
        }
    }
}