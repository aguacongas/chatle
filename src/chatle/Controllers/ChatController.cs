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
        private readonly IChatManager<string, ChatLeUser, Conversation, Attendee, Message> _chatManager;
        private readonly IHubContext _hub;

        public ChatController(IChatManager<string, ChatLeUser, Conversation, Attendee, Message> chatManager, IConnectionManager manager)
        {
            _chatManager = chatManager;
            _hub = manager.GetHubContext<ChatHub>();
        }
        // GET: /<controller>/
        [HttpGet("{id}")]
        public async Task<IEnumerable<Message>> Get(string id)
        {
            return await _chatManager.GetMessagesAsync(id);
        }

        [HttpPost()]
        public async Task SendMessage(string to, string text)
        {
            var message = new Message() { ConversationId = to, Text = text, Date = DateTime.Now };
            await _chatManager.AddMessageAsync(Context.User.Identity.Name, to, message);
            _hub.Clients.Group(to).messageReceived(message);
        }

        [HttpPost("conv")]
        public async Task<string> CreateConversation(string to, string text)
        {
            var userName = Context.User.Identity.Name;
            var conversation = await _chatManager.GetOrCreateConversationAsync(userName, to, text);
            _hub.Clients.Group(to).joinConversation(conversation);
            return conversation.Id;
        }

    }
}
