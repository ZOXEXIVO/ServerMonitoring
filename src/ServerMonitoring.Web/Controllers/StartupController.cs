using Microsoft.AspNet.Http.Extensions;
using Microsoft.AspNet.Mvc;

namespace ServerMonitoring.Web.Controllers
{
    public class StartupController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.CurrentUrl = Request.GetDisplayUrl();

            return View();
        }
    }
}
