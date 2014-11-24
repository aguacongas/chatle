using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ChatLe.Models
{
    public class ChatStore : ChatStore<ApplicationUser>
    {
        public ChatStore(DbContext context) : base(context) { }
    }
    public class ChatStore<TUser> : ChatStore<string, TUser, DbContext>
        where TUser : class, IApplicationUser<string>
        {
        public ChatStore(DbContext context) : base(context) { }
    }
    public class ChatStore<TKey, TUser, TContext> :IChatStore<TKey,TUser>
        where TKey : IEquatable<TKey>
        where TUser : class, IApplicationUser<TKey>
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

        public DbSet<TUser> Users { get { return Context.Set<TUser>(); } }
        public DbSet<Conversation<TKey>> Conversations { get { return Context.Set<Conversation<TKey>>(); } }
        public DbSet<Message<TKey>> Messages { get { return Context.Set<Message<TKey>>(); } }

        public async Task AddMessageAsync(Conversation<TKey> conversation, Message<TKey> message)
        {            
            message.ConversationId = conversation.Id;
            await Messages.AddAsync(message);
            await Context.SaveChangesAsync();
            conversation.Messages.Add(message);
        }

        public async Task AddAttendeeAsync(Conversation<TKey> conversation, Message<TKey> message)
        {
            throw new NotImplementedException();
        }

        public async Task<Conversation<TKey>> CreateConversationAsync(TUser attendee1, TUser attendee2)
        {
            var conversation = new Conversation<TKey>();
            await Conversations.AddAsync(conversation);
            await Context.SaveChangesAsync();
            conversation.Users.Add(attendee1);
            conversation.Users.Add(attendee2);
            return conversation;
        }

        public async Task<TUser> FindUserByNameAsync(string userName)
        {
            return await Users.FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task SetConnectionStatusAsync(TUser user, string connectionId, bool status, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            Trace.TraceInformation("[ChatStore] SetConnectionStatus {0} {1} {2}", user.UserName, connectionId, status);
            user.IsConnected = status;
            user.SignalRConnectionId = connectionId;
            await Context.UpdateAsync(user, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Conversation<TKey>> GetConversationAsync(TUser attendee1, TUser attendee2)
        {
            return await Conversations.FirstOrDefaultAsync(x => x.Users.Count == 2 && x.Users.Any(a => a.Id.Equals(attendee1.Id)) && x.Users.Any(b => b.Id.Equals(attendee2.UserName)));
        }
    }
}