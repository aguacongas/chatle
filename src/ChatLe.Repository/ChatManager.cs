using System;
using System.Threading;
using System.Threading.Tasks;

namespace ChatLe.Models
{
    public class ChatManager<TUser> : IChatManager<TUser> where TUser : IApplicationUser
    {
        public ChatManager(IChatStore<TUser> store)
        {
            if (store == null)
            {
                throw new ArgumentNullException("store");
            }
            Store = store;
        }

        public IChatStore<TUser> Store { get; private set; }

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