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

    public class ChatStore<TUser> : ChatStore<string, TUser, DbContext, Conversation, Attendee, Message, NotificationConnection>
        where TUser : class, IChatUser<string>
        {
        public ChatStore(DbContext context) : base(context) { }
    }
    public class ChatStore<TKey, TUser, TContext, TConversation, TAttendee, TMessage, TNotificationConnection> :IChatStore<TKey,TUser, TConversation, TAttendee, TMessage, TNotificationConnection>
        where TKey : IEquatable<TKey>
        where TUser : class, IChatUser<TKey>
        where TContext : DbContext
        where TConversation : Conversation<TKey>
        where TAttendee : Attendee<TKey>
        where TMessage : Message<TKey>
        where TNotificationConnection : NotificationConnection<TKey>
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
        public DbSet<TNotificationConnection> NotificationConnections { get { return Context.Set<TNotificationConnection>(); } }

        public async Task CreateMessageAsync(TMessage message, CancellationToken cancellationToken = default(CancellationToken))
        {            
            if(message == null)
            {
                throw new ArgumentNullException("message");
            }
            await Context.AddAsync(message, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateAttendeeAsync(TAttendee attendee, CancellationToken cancellationToken = default(CancellationToken))
        {
            if(attendee == null)
            {
                throw new ArgumentNullException("attendee");
            }
            await Context.AddAsync(attendee, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateConversationAsync(TConversation conversation, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (conversation == null)
            {
                throw new ArgumentNullException("conversation");
            }
            await Context.AddAsync(conversation, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
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
            var query = (from u in Users
                         join nc in NotificationConnections
                         on u.Id equals nc.UserId
                         select u).Distinct();

            return await query.ToListAsync();
        }

        public async Task CreateNotificationConnectionAsync(TNotificationConnection connection, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            await Context.AddAsync(connection, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteNotificationConnectionAsync(TNotificationConnection connection, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            Context.Delete(connection);
            await Context.SaveChangesAsync(cancellationToken);
        }
        public async Task<TNotificationConnection> GetNotificationConnectionAsync(string connectionId, string notificationType)
        {
            if (notificationType == null)
            {
                throw new ArgumentNullException("connectionId");
            }
            if (notificationType == null)
            {
                throw new ArgumentNullException("connectionId");
            }
            return await NotificationConnections.FirstOrDefaultAsync(c => c.ConnectionId.Equals(connectionId) && c.NotificationType.Equals(notificationType));
        }

        public void Init()
        {
            NotificationConnections.RemoveRange(NotificationConnections);
            Attendees.RemoveRange(Attendees);
            Conversations.RemoveRange(Conversations);
            Context.SaveChanges();
        }

        public async Task<IEnumerable<TNotificationConnection>> GetNotificationConnectionsAsync(TKey userId, string notificationType)
        {
            return await NotificationConnections.Where(n => n.UserId.Equals(userId) && (notificationType == null || n.NotificationType == notificationType)).ToListAsync();
        }

        public async Task<IEnumerable<TAttendee>> GetAttendeesAsync(TConversation conv)
        {
            var attendees = await Attendees.Where(a => a.ConversationId.Equals(conv.Id)).ToListAsync();
            var atts = conv.Attendees;
            foreach(var attendee in attendees)
            {
                if (!atts.Any(a => a.UserId.Equals(attendee.UserId)))
                {
                    atts.Add(attendee);
                }
            }

            return attendees;
        }
    }
}