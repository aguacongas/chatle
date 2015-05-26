using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Logging;

namespace ChatLe.Controllers
{
    public class HomeController : Controller
    {
        static ILogger _logger;
        public HomeController(ILoggerFactory factory)
        {
            if (_logger == null)
                _logger = factory.CreateLogger("Unhandled Error");
        }
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
                return View();

            return RedirectToRoute(new { controller= "Account", action= "Index" });
        }

        public IActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            var feature = Context.GetFeature<IErrorHandlerFeature>();
            var error = feature?.Error;
            _logger.LogError("Oops!", error);
            return View("~/Views/Shared/Error.cshtml", error);
        }
    }
}