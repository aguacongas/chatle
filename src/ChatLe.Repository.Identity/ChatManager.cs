using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChatLe.Models
{
    /// <summary>
    /// Default <see cref="ChatManager<TKey, TUser, TConversation, TAttendee, TMessage, TNotificationConnection>"/>
    /// </summary>
    public class ChatManager : ChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="store">a chat store</param>
        public ChatManager(IChatStore<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection> store) : base(store) { }
    }
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
        where TUser : IChatUser<TKey>
        where TConversation : Conversation<TKey>, new()
        where TAttendee : Attendee<TKey>, new()
        where TMessage : Message<TKey>, new()
        where TNotificationConnection : NotificationConnection<TKey>, new()
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="store">the store</param>
        public ChatManager(IChatStore<TKey, TUser, TConversation, TAttendee, TMessage, TNotificationConnection> store)
        {
            if (store == null)
            {
                throw new ArgumentNullException("store");
            }
            Store = store;
        }

        public IChatStore<TKey, TUser, TConversation, TAttendee, TMessage, TNotificationConnection> Store { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="connectionId"></param>
        /// <param name="notificationType"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task AddConnectionIdAsync(string userName, string connectionId, string notificationType, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (userName == null)
            {
                throw new ArgumentNullException("userName");
            }
            if (connectionId == null)
            {
                throw new ArgumentNullException("connectionId");
            }

            var user = await Store.FindUserByNameAsync(userName);
            if (user != null)
            {
                var nc = await Store.GetNotificationConnectionAsync(connectionId, notificationType);
                if (nc == null)
                {
                    nc = new TNotificationConnection()
                    {
                        UserId = user.Id,
                        ConnectionId = connectionId,
                        NotificationType = notificationType
                    };
                    await Store.CreateNotificationConnectionAsync(nc, cancellationToken);
                }
                user.NotificationConnections.Add(nc);
            }
        }
        public async Task RemoveConnectionIdAsync(string userName, string connectionId, string notificationType, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (userName == null)
            {
                throw new ArgumentNullException("userName");
            }
            if (connectionId == null)
            {
                throw new ArgumentNullException("connectionId");
            }

            var user = await Store.FindUserByNameAsync(userName);
            if (user != null)
            {
                var nc = await Store.GetNotificationConnectionAsync(connectionId, notificationType);
                if (nc == null)
                {
                    await Store.DeleteNotificationConnectionAsync(nc, cancellationToken);
                    user.NotificationConnections.Remove(nc);
                }
            }
        }
        public async Task<TConversation> AddMessageAsync(string fromName, TKey toConversationId, TMessage message, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (fromName == null)
            {
                throw new ArgumentNullException("fromName");
            }
            if (toConversationId == null)
            {
                throw new ArgumentNullException("toConversationId");
            }

            var user = await Store.FindUserByNameAsync(fromName);
            if (user != null)
            {
                var conv = await Store.GetConversationAsync(toConversationId);
                if (conv != null)
                {
                    // TODO: check if it's necessary when EF7 will be release
                    await Store.GetAttendeesAsync(conv);
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

        public async Task<TConversation> GetOrCreateConversationAsync(string from, string to, string initialMessage = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (from == null)
            {
                throw new ArgumentNullException("from");
            }
            if (to == null)
            {
                throw new ArgumentNullException("to");
            }

            var attendee1 = await Store.FindUserByNameAsync(from);
            var attendee2 = await Store.FindUserByNameAsync(to);
            var conv = await Store.GetConversationAsync(attendee1, attendee2);
            if (conv == null)
            {
                conv = new TConversation();
                await Store.CreateConversationAsync(conv, cancellationToken);
                conv.Attendees.Add(await AddAttendeeAsync(conv.Id, attendee1.Id, cancellationToken));
                conv.Attendees.Add(await AddAttendeeAsync(conv.Id, attendee2.Id, cancellationToken));
            }

            if (initialMessage != null)
                await AddMessageAsync(conv, attendee1, initialMessage, cancellationToken);

            return conv;
        }

        private async Task AddMessageAsync(TConversation conv, TUser user, string text, CancellationToken cancellationToken)
        {
            var message = new TMessage();
            message.ConversationId = conv.Id;
            message.UserId = user.Id;
            message.Text = text;
            await Store.CreateMessageAsync(message, cancellationToken);
            conv.Messages.Add(message);
        }
        private async Task<TAttendee> AddAttendeeAsync(TKey convId, TKey userId, CancellationToken cancellationToken)
        {
            var attendee = new TAttendee();
            attendee.ConversationId = convId;
            attendee.UserId = userId;
            await Store.CreateAttendeeAsync(attendee, cancellationToken);
            return attendee;
        }

        public Task<IEnumerable<TMessage>> GetMessagesAsync(TKey convId)
        {
            return Store.GetMessagesAsync(convId);
        }

        public Task<IEnumerable<TUser>> GetUsersConnectedAsync()
        {
            return Store.GetUsersConnectedAsync();
        }

        public Task<IEnumerable<TNotificationConnection>> GetNotificationConnectionsAsync(TKey userId, string notificationType)
        {
            return Store.GetNotificationConnectionsAsync(userId, notificationType);
        }

        public async Task<IEnumerable<TConversation>> GetConversationsAsync(string userName)
        {
            if (userName == null)
            {
                throw new ArgumentNullException("userName");
            }
            var user = await Store.FindUserByNameAsync(userName);
            var conversations = await Store.GetConversationsAsync(user.Id);
            // TODO: check if it's necessary when EF7 will be release
            foreach (var conv in conversations)
            {
                await Store.GetAttendeesAsync(conv);
                var messages = await Store.GetMessagesAsync(conv.Id);
                foreach (var message in messages)
                {
                    if (!conv.Messages.Any(m => m.Id.Equals(message.Id)))
                        conv.Messages.Add(message);
                }
            }
            return conversations;
        }
    }
}