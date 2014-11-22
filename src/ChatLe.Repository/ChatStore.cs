using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ChatLe.Models
{
    public class ChatStore : ChatStore<ApplicationUser>
    {
        public ChatStore(DbContext context) : base(context) { }
    }
    public class ChatStore<TUser> : ChatStore<string, TUser, DbContext>
        where TUser : IdentityUser, IApplicationUser
    {
        public ChatStore(DbContext context) : base(context) { }
    }
    public class ChatStore<TKey, TUser, TContext>
        where TKey : IEquatable<TKey>
        where TUser : IdentityUser<TKey>, IApplicationUser
        where TContext : DbContext
    {
        public ChatStore(TContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            Context = context;
        }

        public TContext Context { get; private set; }

        public async Task<bool> SetConnectionStatusAsync(TUser user, string connectionId, bool isConnected, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (isConnected)
            {
                var ret = !user.IsConnected;
                await SetConnectionStatus(true, connectionId, user);
                return ret;
            }
            else if (user.SignalRConnectionId == connectionId)
            {
                await SetConnectionStatus(false, null, user);
                return true;
            }

            return false;
        }

        private async Task SetConnectionStatus(bool status, string connectionId, TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            Trace.TraceInformation("[ChatStore] SetConnectionStatus {0} {1} {2}", user.UserName, connectionId, status);
            user.IsConnected = status;
            user.SignalRConnectionId = connectionId;
            await Context.UpdateAsync(user, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
        }

    }
}