using System.IO;
using System.Web;
using System.Web.Mvc;
using Thesis.Services.Interfaces;

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
            if (file != null && file.ContentLength > 0)
            {
                string fileName = Path.GetFileName(file.FileName);
                string path = "";
                
                if (fileName != null)
                {
                    path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                    file.SaveAs(path);
                }
                _graphService.ImportXMLFile(path, "ThesisImportDatabase");
            }

            return RedirectToAction("Index");
        }
    }
}