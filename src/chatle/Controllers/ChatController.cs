using ChatLe.Hubs;
using ChatLe.Models;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.SignalR.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using chatle.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ChatLe.Controllers
{
    [Route("api/chat")]
    public class ChatController : Controller
    {
        private readonly IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection> _chatManager;
        private readonly IHubContext _hub;
        private readonly UserManager<ChatLeUser> _userManager;

        public ChatController(IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection> chatManager, IConnectionManager manager, UserManager<ChatLeUser> userManager)
        {
            _chatManager = chatManager;
            _hub = manager.GetHubContext<ChatHub>();
            _userManager = userManager;
        }
        // GET: /<controller>/
        [HttpGet("{id}")]
        public async Task<IEnumerable<MessageViewModel>> Get(string id)
        {
            var messages = await _chatManager.GetMessagesAsync(id);
            var users = new List<ChatLeUser>(2);
            var messagesVM = new List<MessageViewModel>(messages.Count());
            foreach (var message in messages)
            {
                var user = users.FirstOrDefault(u => u.Id == message.UserId);
                if (user == null)
                {
                    user = await _userManager.FindByIdAsync(message.UserId);
                    if (user == null)
                        break;
                }
                messagesVM.Add(new MessageViewModel() { Date = message.Date, From = user.UserName, Text = message.Text });
            }

            return messagesVM;
        }

        [HttpPost()]
        public async Task SendMessage(string to, string text)
        {
            var userName = Context.User.Identity.Name;
            var message = new Message() { ConversationId = to, Text = text, Date = DateTime.Now };
            var conv = await _chatManager.AddMessageAsync(userName, to, message);
            foreach(var attendee in conv.Attendees)
            {
                var user = await _userManager.FindByIdAsync(attendee.UserId);
                if (user.UserName != userName)
                    _hub.Clients.Group(user.UserName).messageReceived(to, new MessageViewModel() { Date = message.Date, From = Context.User.Identity.Name, Text = text });
            }            
        }

        [HttpPost("conv")]
        public async Task<string> CreateConversation(string to, string text)
        {
            var userName = Context.User.Identity.Name;
            var conversation = await _chatManager.GetOrCreateConversationAsync(userName, to, text);
            if (conversation == null)
                return null;

            var attendees = new List<AttendeeViewModel>(conversation.Attendees.Count);
            var users = new List<ChatLeUser>(conversation.Attendees.Count);
            foreach(var attendee in conversation.Attendees)
            {
                var u = await _userManager.FindByIdAsync(attendee.UserId);
                if (u == null)
                    return null;

                users.Add(u);
                attendees.Add(new AttendeeViewModel() { UserId = u.UserName });
            }
            var messages = new List<MessageViewModel>(conversation.Messages.Count);
            foreach(var message in conversation.Messages)
            {
                var user = users.FirstOrDefault(u => u.Id == message.UserId);
                if (user == null)
                {
                    user = await _userManager.FindByIdAsync(message.UserId);
                    if (user == null)
                        break;
                }
                messages.Add(new MessageViewModel() { Date = message.Date, From = user.UserName, Text = message.Text });
            }
            _hub.Clients.Group(to).joinConversation(new { Id = conversation.Id, Attendees = attendees, Messages = messages });
            
            return conversation.Id;
        }

    }
}
