using Microsoft.Data.Entity;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ChatLe.Models
{
    public class ChatStore : ChatStore<ChatLeUser>
    {
        public ChatStore(ChatLeIdentityDbContext context) : base(context) { }
    }

    public class ChatStore<TUser> : ChatStore<string, TUser, DbContext, Conversation, Attendee, Message>
        where TUser : class, IChatUser<string>
        {
        public ChatStore(DbContext context) : base(context) { }
    }
    public class ChatStore<TKey, TUser, TContext, TConversation, TAttendee, TMessage> :IChatStore<TKey,TUser, TConversation, TAttendee, TMessage>
        where TKey : IEquatable<TKey>
        where TUser : class, IChatUser<TKey>
        where TContext : DbContext
        where TConversation : Conversation<TKey>
        where TAttendee : Attendee<TKey>
        where TMessage : Message<TKey>
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
        public DbSet<TConversation> Conversations { get { return Context.Set<TConversation>(); } }
        public DbSet<TMessage> Messages { get { return Context.Set<TMessage>(); } }
        public DbSet<TAttendee> Attendees { get { return Context.Set<TAttendee>(); } }

        public async Task CreateMessageAsync(TMessage message)
        {            
            if(message == null)
            {
                throw new ArgumentNullException("message");
            }
            await Context.AddAsync(message);
            await Context.SaveChangesAsync();
        }

        public async Task CreateAttendeeAsync(TAttendee attendee)
        {
            if(attendee == null)
            {
                throw new ArgumentNullException("attendee");
            }
            await Context.AddAsync(attendee);
            await Context.SaveChangesAsync();
        }

        public async Task CreateConversationAsync(TConversation conversation)
        {
            if (conversation == null)
            {
                throw new ArgumentNullException("conversation");
            }
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

        public async Task<TConversation> GetConversationAsync(TUser attendee1, TUser attendee2)
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

        public async Task<TConversation> GetConversationAsync(TKey convId)
        {
            return await Conversations.FirstOrDefaultAsync(c => c.Id.Equals(convId));
        }

        public async Task<IEnumerable<TMessage>> GetMessagesAsync(TKey convId)
        {
            return await Messages.Where(m => m.ConversationId.Equals(convId)).ToListAsync();
        }

        public async Task<IEnumerable<TUser>> GetUsersConnectedAsync()
        {
            return await Users.Where(u => u.IsConnected == true).ToListAsync();
        }
    }
}