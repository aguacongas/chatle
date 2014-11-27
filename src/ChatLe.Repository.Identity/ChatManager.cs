using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChatLe.Models
{
    public class ChatManager : ChatManager<string, ApplicationUser>
    {
    }
    public class ChatManager<TKey, TUser> : IChatManager<TKey, TUser> where TUser : IApplicationUser<TKey>
    {
        public ChatManager(IChatStore<TKey, TUser> store)
        {
            if (store == null)
            {
                throw new ArgumentNullException("store");
            }
            Store = store;
        }

        public IChatStore<TKey, TUser> Store { get; private set; }

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
            if (!user.SignalRConnectionIds.Contains(connectionId))
        {
                user.SignalRConnectionIds.Add(connectionId);
            }
            await Store.UpdateUserAsync(user, cancellationToken);
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
        public async Task AddMessageAsync(string fromName, TKey toConversationId, Message<TKey> message)
        {
            if (fromName == null)
            {
                throw new ArgumentNullException("fromName");
            }
            if (toConversationId == null)
            {
                throw new ArgumentNullException("toConversationId");
        }
            return null;
        }

            var user = await Store.FindUserByNameAsync(fromName);
            if (user != null)
            {
                var conv = await Store.GetConversationAsync(toConversationId);
                message.ConversationId = toConversationId;
                message.UserId = user.Id;
                await Store.AddMessageAsync(message);
                conv.Messages.Add(message);
            }
            }

        public async Task<Conversation<TKey>> GetConversationAsync(string from, string to)
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
            return await Store.GetConversationAsync(attendee1, attendee2);
        }
    }
}