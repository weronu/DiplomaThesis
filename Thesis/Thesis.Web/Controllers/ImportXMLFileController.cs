﻿using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Thesis.Services.Interfaces;
using Thesis.Services.ResponseTypes;
using Thesis.Web.Models;

namespace Thesis.Web.Controllers
{
    public class ImportXMLFileController : Controller
    {
        private readonly IGraphService _graphService;

        public ImportXMLFileController(IGraphService graphService)
        {
            _graphService = graphService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string path = "";

                    if (fileName != null)
                    {
                        path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                        file.SaveAs(path);
                    }

                    ServiceResponse response = _graphService.ImportXMLFile(path, "ThesisImportDatabase");

                    if (response.Succeeded)
                    {
                        this.AddToastMessage("Success", response.SuccessMessage, ToastType.Success);
                    }
                    else
                    {
                        this.AddToastMessage("Error", response.Error, ToastType.Error);
                        return View();
                    }
                }

                GraphViewModel model = new GraphViewModel()
                {
                    FileImported = true
                };

                return RedirectToAction("Index", "TeamMembersEmailGraphs", model);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}