using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

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
