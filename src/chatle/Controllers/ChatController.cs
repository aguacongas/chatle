using ChatLe.Hubs;
using ChatLe.Models;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.SignalR.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using System;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ChatLe.Controllers
{
    [Route("api/chat")]
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
        [HttpGet("{id}")]
        public IEnumerable<Message> Get(string id)
        {
            return _dbContext.Messages.Where(x => (x.From == id && x.ConversationId ==Context.User.Identity.Name) || (x.ConversationId == id && x.From == Context.User.Identity.Name)).OrderByDescending(x => x.Date);
        }

        [HttpPost()]
        public async Task SendMessage(string to, string text)
        {
            var message = new Message() { From = Context.User.Identity.Name, ConversationId = to, Text = text, Date = DateTime.Now };
            _hub.Clients.Group(to).messageReceived(message);
            await _dbContext.Messages.AddAsync(message);
            await _dbContext.SaveChangesAsync();
        }
    }
}
