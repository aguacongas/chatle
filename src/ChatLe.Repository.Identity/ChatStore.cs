using Microsoft.Data.Entity;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        public DbSet<Attendee<TKey>> Attendees { get { return Context.Set<Attendee<TKey>>(); } }

        public async Task CreateMessageAsync(Message<TKey> message)
        {            
            await Context.AddAsync(message);
            await Context.SaveChangesAsync();
        }

        public async Task CreateAttendeeAsync(Attendee<TKey> attendee)
        {
            await Context.AddAsync(attendee);
            await Context.SaveChangesAsync();
        }

        public async Task CreateConversationAsync(Conversation<TKey> conversation)
        {
            await Context.AddAsync(conversation);
            await Context.SaveChangesAsync();
        }

        public async Task<TUser> FindUserByNameAsync(string userName)
        {
            return await Users.FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task UpdateUserAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            await Context.UpdateAsync(user, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Conversation<TKey>> GetConversationAsync(TUser attendee1, TUser attendee2)
        {
            if (attendee1 == null)
            {
                throw new ArgumentNullException("attendee1");
            }
            if (attendee2 == null)
            {
                throw new ArgumentNullException("attendee2");
            }
            return await Conversations.FirstOrDefaultAsync(x => x.Attendees.Count == 2 && x.Attendees.Any(a => a.UserId.Equals(attendee1.Id)) && x.Attendees.Any(b => b.UserId.Equals(attendee2.UserName)));
        }

        public Task<Conversation<TKey>> GetConversationAsync(TKey toConversationId)
        {
            throw new NotImplementedException();
        }

        public Task AddMessageAsync(Message<TKey> message)
        {
            throw new NotImplementedException();
        }
    }
}