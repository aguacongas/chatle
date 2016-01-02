using Microsoft.Extensions.OptionsModel;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChatLe.Models
{    
    /// <summary>
    /// Manage chat repository
    /// </summary>
    /// <typeparam name="TKey">type of primary key</typeparam>
    /// <typeparam name="TUser">type of user, must implement <see cref="IChatUser<TKey>"/></typeparam>
    /// <typeparam name="TConversation">type of conversation, must be a <see cref="Conversation<TKey>"/></typeparam>
    /// <typeparam name="TAttendee">type of attendee, must be a <see cref="Attendee<TKey>"/></typeparam>
    /// <typeparam name="TMessage">type of message, must be a <see cref="Message<TKey>"/></typeparam>
    /// <typeparam name="TNotificationConnection">type of notification's connection, must be a <see cref="NotificationConnection<TKey>"/></typeparam>
    public class ChatManager<TKey, TUser, TConversation, TAttendee, TMessage, TNotificationConnection> : IChatManager<TKey, TUser, TConversation, TAttendee, TMessage, TNotificationConnection>
        where TKey : IEquatable<TKey>
        where TUser : IChatUser<TKey>
        where TConversation : Conversation<TKey>, new()
        where TAttendee : Attendee<TKey>, new()
        where TMessage : Message<TKey>, new()
        where TNotificationConnection : NotificationConnection<TKey>, new()
    {
        private readonly ILogger _logger;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="store">the store</param>
        public ChatManager(IChatStore<TKey, TUser, TConversation, TAttendee, TMessage, TNotificationConnection> store, IOptions<ChatOptions> optionsAccessor, ILoggerFactory loggerFactory = null)
        {
            if (store == null)
                throw new ArgumentNullException("store");
            if (optionsAccessor == null || optionsAccessor.Value == null)
                throw new ArgumentNullException("optionsAccessor");

            Store = store;
            Options = optionsAccessor.Value;
            if (loggerFactory != null)
                _logger = loggerFactory.CreateLogger("ChatLe.Models.ChatManager");
            else
                _logger = new FakeLogger();            
        }
        /// <summary>
        /// Gets the store
        /// </summary>
        public virtual IChatStore<TKey, TUser, TConversation, TAttendee, TMessage, TNotificationConnection> Store { get; private set; }
        /// <summary>
        /// Gets the options
        /// </summary>
        public virtual ChatOptions Options { get; private set; }

        /// <summary>
        /// Adds a notification connection assotiate to a user
        /// </summary>
        /// <param name="userName">The user name</param>
        /// <param name="connectionId">The connection id</param>
        /// <param name="notificationType">The notification type</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a Task</returns>
        public virtual async Task AddConnectionIdAsync(string userName, string connectionId, string notificationType, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (userName == null)
                throw new ArgumentNullException("userName");
            if (connectionId == null)
                throw new ArgumentNullException("connectionId");
            if (notificationType == null)
                throw new ArgumentNullException("notificationType");

            _logger.LogInformation("AddConnectionIdAsync {0} {1} {2}", userName, connectionId, notificationType);

            var user = await Store.FindUserByNameAsync(userName, cancellationToken);
            if (user != null)
            {
                var nc = await Store.GetNotificationConnectionAsync(connectionId, notificationType);
                if (nc == null)
                {
                    nc = new TNotificationConnection()
                    {
                        UserId = user.Id,
                        ConnectionId = connectionId,
                        NotificationType = notificationType,
                        ConnectionDate = DateTime.UtcNow
                    };
                    await Store.CreateNotificationConnectionAsync(nc, cancellationToken);
                }
                user.NotificationConnections.Add(nc);
            }
        }
        /// <summary>
        /// Removes a notification connection assotiate to a user
        /// </summary>
        /// <param name="connectionId">The connection id</param>
        /// <param name="notificationType">the type of notification</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>A Task</returns>
        public virtual async Task<TUser> RemoveConnectionIdAsync(string connectionId, string notificationType, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (connectionId == null)
                throw new ArgumentNullException("connectionId");
            if (notificationType == null)
                throw new ArgumentNullException("notificationType");

            var nc = await Store.GetNotificationConnectionAsync(connectionId, notificationType, cancellationToken);
			if (nc != null)
			{
				await Store.DeleteNotificationConnectionAsync(nc, cancellationToken);
				var user = await Store.FindUserByIdAsync(nc.UserId);
				if (user != null)
				{
					var ret = await Store.UserHasConnectionAsync(user.Id);
					if (!ret && user.PasswordHash == null)
						await Store.DeleteUserAsync(user, cancellationToken);
					return user;
				}
			}
			return default(TUser);
        }
        /// <summary>
        /// Adds a message to a conversation
        /// </summary>
        /// <param name="fromName">The sender name</param>
        /// <param name="toConversationId">The conversation id</param>
        /// <param name="message">The message</param>
        /// <param name="cancellationToken">an  optional cancellation token</param>
        /// <returns>a Task</returns>
        public virtual async Task<TConversation> AddMessageAsync(string fromName, TKey toConversationId, TMessage message, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (fromName == null)
                throw new ArgumentNullException("fromName");
            if (toConversationId == null)
                throw new ArgumentNullException("toConversationId");

            var user = await Store.FindUserByNameAsync(fromName, cancellationToken);
            if (user != null)
            {
                var conv = await Store.GetConversationAsync(toConversationId, cancellationToken);
                if (conv != null)
                {
                    // TODO: check if it's necessary when EF7 will be release
                    await Store.GetAttendeesAsync(conv, cancellationToken);
                    // Add the message
                    message.ConversationId = toConversationId;
                    message.UserId = user.Id;
                    await Store.CreateMessageAsync(message, cancellationToken);
                    conv.Messages.Add(message);
                    return conv;
                }
            }

            return null;
        }
        /// <summary>
        /// Gets or creates a conversation
        /// </summary>
        /// <param name="from">The name of the 1st attendee</param>
        /// <param name="to">The name of the second attendee</param>
        /// <param name="inialMessage">The initial message if any</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a Task</returns>
        public virtual async Task<TConversation> GetOrCreateConversationAsync(string from, string to, string initialMessage = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (from == null)
                throw new ArgumentNullException("from");
            if (to == null)
                throw new ArgumentNullException("to");

            _logger.LogInformation("GetOrCreateConversationAsync from : {0}, to : {1}, initialMessage: {2}", from, to, initialMessage);

            var attendee1 = await Store.FindUserByNameAsync(from, cancellationToken);
            var attendee2 = await Store.FindUserByNameAsync(to, cancellationToken);
            var conv = await Store.GetConversationAsync(attendee1, attendee2, cancellationToken);
            if (conv == null)
            {
                conv = new TConversation();
                await Store.CreateConversationAsync(conv, cancellationToken);
                await AddAttendeeAsync(conv, attendee1.Id, cancellationToken);
                await AddAttendeeAsync(conv, attendee2.Id, cancellationToken);
            }

            if (initialMessage != null)
                await AddMessageAsync(conv, attendee1, initialMessage, cancellationToken);

            return conv;
        }
        /// <summary>
        /// Add a message in a conversation
        /// </summary>
        /// <param name="conv">the conversation</param>
        /// <param name="sender">the sender</param>
        /// <param name="content">the message content</param>
        /// <param name="cancellationToken">a cancellation</param>
        /// <returns>an async task</returns>
        protected virtual async Task AddMessageAsync(TConversation conv, TUser sender, string content, CancellationToken cancellationToken)
        {
            _logger.LogInformation("AddMessage to conversation : {0} content : {1}", conv.Id, content);
            
            var message = new TMessage();
            message.ConversationId = conv.Id;
            message.UserId = sender.Id;
            message.Text = content;
            await Store.CreateMessageAsync(message, cancellationToken);
            var messages = conv.Messages;
            if (!messages.Any(m => m.Id.Equals(message.Id)))
                messages.Add(message);
        }
        /// <summary>
        /// Add an attendee in a conversation by ids
        /// </summary>
        /// <param name="convId">the conversation id</param>
        /// <param name="userId">the user id</param>
        /// <param name="cancellationToken">a cancellation token</param>
        /// <returns>an async task</returns>
        protected virtual async Task<TAttendee> AddAttendeeAsync(TConversation conv, TKey userId, CancellationToken cancellationToken)
        {
            var attendee = new TAttendee();
            attendee.ConversationId = conv.Id;
            attendee.UserId = userId;
            await Store.CreateAttendeeAsync(attendee, cancellationToken);
            var attendees = conv.Attendees;
            if (!attendees.Any(a => a.UserId.Equals(attendee.UserId)))
                attendees.Add(attendee);
            return attendee;
        }
        /// <summary>
        /// Gets the messages list for a conversation
        /// </summary>
        /// <param name="id">the conversation id</param>
        /// <param name="cancellationToken">an optional concellation token</param>
        /// <returns>a Task</returns>
        public virtual Task<IEnumerable<TMessage>> GetMessagesAsync(TKey convId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Store.GetMessagesAsync(convId, cancellationToken: cancellationToken);
        }
        /// <summary>
        /// Gets a page of connected users
        /// </summary>
        /// <param name="pageIndex">the page index</param>
        /// <param name="cancellationToken">an optional concellation token</param>
        /// <returns>a Task</returns>
        public virtual Task<Page<TUser>> GetUsersConnectedAsync(int pageIndex = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Store.GetUsersConnectedAsync(pageIndex, Options.UserPerPage, cancellationToken);
        }

        /// <summary>
        /// Gets the list of conversation for a user
        /// </summary>
        /// <param name="userName">the user name</param>
        /// <param name="cancellationToken">an optional concellation token</param>
        /// <returns>a Task</returns>
        public virtual async Task<IEnumerable<TConversation>> GetConversationsAsync(string userName, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (userName == null)
                throw new ArgumentNullException("userName");

            var user = await Store.FindUserByNameAsync(userName, cancellationToken);
            var conversations = await Store.GetConversationsAsync(user.Id, cancellationToken);
            // TODO: check if it's necessary when EF7 will be release
            foreach (var conv in conversations)
            {
                var attendees = await Store.GetAttendeesAsync(conv, cancellationToken);
                foreach (var attendee in attendees)
                    if (!conv.Attendees.Any(a => a.UserId.Equals(attendee.UserId)))
                        conv.Attendees.Add(attendee);

                var messages = await Store.GetMessagesAsync(conv.Id, cancellationToken: cancellationToken);
                foreach (var message in messages)
                    if (!conv.Messages.Any(m => m.Id.Equals(message.Id)))
                        conv.Messages.Add(message);
            }
            return conversations;
        }

        public async Task RemoveUserAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
                throw new ArgumentNullException("user");

            await Store.DeleteUserAsync(user, cancellationToken);
        }
    }
}