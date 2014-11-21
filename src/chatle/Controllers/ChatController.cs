using ChatLe.Hubs;
using ChatLe.Models;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.SignalR.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ChatLe.Controllers
{
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHubContext _hub;

        public ChatController(ApplicationDbContext dbContext, IConnectionManager manager)
        {
            _dbContext = dbContext;
            _hub = manager.GetHubContext<ChatHub>();
        }
        // GET: /<controller>/
        [HttpGet("messages")]
        public IEnumerable<Message> Get(string id)
        {
            return _dbContext.Messages.Where(x => (x.From == id && x.To ==Context.User.Identity.Name) || (x.To == id && x.From == Context.User.Identity.Name)).OrderByDescending(x => x.Date);
        }

        [HttpPost("message")]
        public void SendMessage(Message message)
        {
            _hub.Clients.Group(message.To).messageReceived(message.Text);
        }
    }
}
