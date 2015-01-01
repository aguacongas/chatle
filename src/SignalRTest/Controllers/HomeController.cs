using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using SignalRTest.Hubs;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace SignalRTest.Controllers
{
    public class HomeController : Controller
    {
        IHubContext _hub;
        public HomeController(IConnectionManager manager)
        {
            _hub = manager.GetHubContext<TestHub>();
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        public void Test()
        {
            _hub.Clients.All.test("coucou");
        }
    }
}
