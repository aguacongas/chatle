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
    /// <summary>
    /// Chat API controller
    /// </summary>
    [Route("api/chat")]
    public class ChatController : Controller
    {
        private readonly IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection> _chatManager;
        private readonly IHubContext _hub;
        private readonly UserManager<ChatLeUser> _userManager;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="chatManager">the chat repository manager</param>
        /// <param name="signalRConnectionManager">the SignalR connection manager</param>
        /// <param name="userManager">the Identity user manager</param>
        public ChatController(IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection> chatManager, IConnectionManager signalRConnectionManager, UserManager<ChatLeUser> userManager)
        {
            _chatManager = chatManager;
            _hub = signalRConnectionManager.GetHubContext<ChatHub>();
            _userManager = userManager;
        }
        /// <summary>
        /// Get messages in a conversation asyncronously
        /// </summary>
        /// <param name="id">the conversation id</param>
        /// <returns>a <see cref="Task<IEnumerable<MessageViewModel>>"/> with the messages list as result</returns>
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
                        continue;
                }
                messagesVM.Add(new MessageViewModel() { Date = message.Date, From = user.UserName, Text = message.Text });
            }

            return messagesVM;
        }

        /// <summary>
        /// Send a message in a conversation asyncronously
        /// </summary>
        /// <param name="to">the conversation id</param>
        /// <param name="text">the message content</param>
        /// <returns>a <see cref="Task"/></returns>
        [HttpPost()]
        public async Task SendMessage(string to, string text)
        {
            var userName = Context.User.Identity.Name;
            var message = new Message() { ConversationId = to, Text  = text, Date = DateTime.Now };
            var conv = await _chatManager.AddMessageAsync(userName, to, message);
            if (conv == null)
                return;
            foreach(var attendee in conv.Attendees)
            {
                var user = await _userManager.FindByIdAsync(attendee.UserId);
                if (user != null && user.UserName != userName)
                    _hub.Clients.Group(user.UserName).messageReceived(to, new MessageViewModel() { Date = message.Date, From = Context.User.Identity.Name, Text = text });
            }            
        }

        /// <summary>
        /// Get or create a one to one conversation asyncronously
        /// </summary>
        /// <param name="to">the second user</param>
        /// <param name="text">the 1st message content</param>
        /// <returns>a <see cref="Task<string>"/> with the conversation id as result or null if the user doesn't exist</returns>
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
                        continue;
                }
                messages.Add(new MessageViewModel() { Date = message.Date, From = user.UserName, Text = message.Text });
            }
            _hub.Clients.Group(to).joinConversation(new { Id = conversation.Id, Attendees = attendees, Messages = messages });
            
            return conversation.Id;
        }

    }
}
