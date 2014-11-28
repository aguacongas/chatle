using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChatLe.Models
{
    public class ChatManager : ChatManager<string, ChatLeUser, Conversation, Attendee, Message>
    {
        public ChatManager(IChatStore<string, ChatLeUser, Conversation, Attendee, Message> store) : base(store) { }
    }

    public class ChatManager<TKey, TUser, TConversation, TAttendee, TMessage> : IChatManager<TKey, TUser, TConversation, TAttendee, TMessage> 
        where TUser : IChatUser<TKey>
        where TConversation : Conversation<TKey>, new()
        where TAttendee : Attendee<TKey>, new()
        where TMessage : Message<TKey>, new()
    {
        public ChatManager(IChatStore<TKey, TUser, TConversation, TAttendee, TMessage> store)
        {
            if (store == null)
            {
                throw new ArgumentNullException("store");
            }
            Store = store;
        }

        public IChatStore<TKey, TUser, TConversation, TAttendee, TMessage> Store { get; private set; }

        public async Task AddConnectionIdAsync(string userName, string connectionId, CancellationToken cancellationToken = default(CancellationToken))
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
                if (!user.SignalRConnectionIds.Contains(connectionId))
                {
                    user.SignalRConnectionIds.Add(connectionId);
                }
                await Store.UpdateUserAsync(user, cancellationToken);
            }
        }
        public async Task RemoveConnectionIdAsync(string userName, string connectionId, CancellationToken cancellationToken = default(CancellationToken))
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
                user.SignalRConnectionIds.Remove(connectionId);
                await Store.UpdateUserAsync(user, cancellationToken);
            }
        }
        public async Task AddMessageAsync(string fromName, TKey toConversationId, TMessage message)
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
                    message.ConversationId = toConversationId;
                    message.UserId = user.Id;
                    await Store.CreateMessageAsync(message);
                    conv.Messages.Add(message);
                }
            }
        }

        public async Task<TConversation> GetOrCreateConversationAsync(string from, string to, string initialMessage = null)
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
                await Store.CreateConversationAsync(conv);
                conv.Attendees.Add(await AddAttendeeAsyn(conv.Id, attendee1.Id));
                conv.Attendees.Add(await AddAttendeeAsyn(conv.Id, attendee2.Id));
            }

            if (initialMessage != null)
                await AddMessageAsync(conv, attendee1, initialMessage);

            return conv;
        }

        private async Task AddMessageAsync(TConversation conv, TUser user, string text)
        {
            var message = new TMessage();
            message.ConversationId = conv.Id;
            message.UserId = user.Id;
            message.Text = text;
            await Store.CreateMessageAsync(message);
            conv.Messages.Add(message);
        }
        private async Task<TAttendee> AddAttendeeAsyn(TKey convId, TKey userId)
        {
            var attendee = new TAttendee();
            attendee.ConversationId = convId;
            attendee.UserId = userId;
            await Store.CreateAttendeeAsync(attendee);
            return attendee;
        }

        public async Task<IEnumerable<TMessage>> GetMessagesAsync(TKey convId)
        {
            return await Store.GetMessagesAsync(convId);
        }

        public Task<IEnumerable<TUser>> GetUsersConnectedAsync()
        {
            return Store.GetUsersConnectedAsync();
        }
    }
}