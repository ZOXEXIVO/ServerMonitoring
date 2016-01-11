using Microsoft.AspNet.Mvc;

namespace ServerMonitoring.Web.Controllers
{
    public class StartupController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
