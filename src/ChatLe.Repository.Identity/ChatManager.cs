using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChatLe.Models
{
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

        public Task AddMessage(TKey from, TKey to, string message)
        {
            throw new NotImplementedException();
        }

        public async Task<Conversation<TKey>> GetConversationAsync(string from, string to)
        {
            if (from == null)
            {
                throw new ArgumentNullException("from");
            }
            if(to == null)
            {
                throw new ArgumentNullException("to");
            }

            var attendee1 = await Store.FindUserByNameAsync(from);
            var attendee2 = await Store.FindUserByNameAsync(to);
            if (attendee1 != null && attendee2 != null)
            {
                return await Store.GetConversationAsync(attendee1, attendee2);
            }
            return null;
        }

        public async Task<bool> SetConnectionStatusAsync(string userName, string connectionId, bool isConnected, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (userName == null)
            {
                throw new ArgumentNullException("userName");
            }
            var user = await Store.FindUserByNameAsync(userName);
            if (isConnected)
            {
                var ret = !user.IsConnected;
                await Store.SetConnectionStatusAsync(user, connectionId, isConnected);
                return ret;
            }
            else if (user.SignalRConnectionId == connectionId)
            {
                await Store.SetConnectionStatusAsync(user, null, false);
                return true;
            }

            return false;
        }
    }
}