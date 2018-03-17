using System.Web.Mvc;
using Thesis.Web.Models;

namespace Thesis.Web.Controllers
{
    public class EmailDownloadController : Controller
    {
        // GET: Download
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SubmitDownload(EmailDownloadViewModel model)
        {

            return View("Index");
        }
    }
}