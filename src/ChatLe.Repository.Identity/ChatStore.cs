using Microsoft.Data.Entity;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ChatLe.Models
{
    /// <summary>
    /// Chat store for <see cref="ChatLeUser"/>
    /// </summary>
    public class ChatStore : ChatStore<ChatLeUser>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The <see cref="ChatLeIdentityDbContext"/> to use</param>
        /// <param name="loggerFactory"></param>
        public ChatStore(ChatLeIdentityDbContext context) : base(context) { }
    }
    /// <summary>
    /// Chat store for TUser
    /// </summary>
    /// <typeparam name="TUser">type of user, must a class and implement <see cref="IChatUser{string}"/></typeparam>
    public class ChatStore<TUser> : ChatStore<string, TUser, DbContext, Conversation, Attendee, Message, NotificationConnection>
        where TUser : class, IChatUser<string>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The <see cref="DbContext" to use/></param>
        /// <param name="loggerFactory"></param>
        public ChatStore(DbContext context) : base(context) { }
    }
    /// <summary>
    /// Chat store, implement <see cref="IChatStore{TKey, TUser, TConversation, TAttendee, TMessage, TNotificationConnection}"/>
    /// </summary>
    /// <typeparam name="TKey">type of primary key</typeparam>
    /// <typeparam name="TUser">type of user, must be a class and implement <see cref="IChatUser{TKey}"/></typeparam>
    /// <typeparam name="TContext">type of context, must be a <see cref="DbContext"/></typeparam>
    /// <typeparam name="TConversation">type of conversation, must be a <see cref="Conversation{TKey}"/></typeparam>
    /// <typeparam name="TAttendee">type of attendee, must be a <see cref="Attendee{TKey}"/></typeparam>
    /// <typeparam name="TMessage">type of message, must be a <see cref="Message{TKey}"/></typeparam>
    /// <typeparam name="TNotificationConnection">type of notifciation connection, must be a <see cref="NotificationConnection{TKey}"/></typeparam>
    public class ChatStore<TKey, TUser, TContext, TConversation, TAttendee, TMessage, TNotificationConnection> :IChatStore<TKey,TUser, TConversation, TAttendee, TMessage, TNotificationConnection>
        where TKey : IEquatable<TKey>
        where TUser : class, IChatUser<TKey>
        where TContext : DbContext
        where TConversation : Conversation<TKey>
        where TAttendee : Attendee<TKey>
        where TMessage : Message<TKey>
        where TNotificationConnection : NotificationConnection<TKey>
    {
        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="context">The <see cref="DbContext" to use/></param>
        /// <param name="loggerFactory"></param>
        public ChatStore(TContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            Context = context;
        }
        /// <summary>
        /// Gets the <see cref="DbContext"/>
        /// </summary>
        public virtual TContext Context { get; private set; }
        /// <summary>
        /// Gets the <see cref="DbSet{TUser}"/>
        /// </summary>
        public virtual DbSet<TUser> Users { get { return Context.Set<TUser>(); } }
        /// <summary>
        /// Gets the <see cref="DbSet{TConversation}"/>
        /// </summary>
        public DbSet<TConversation> Conversations { get { return Context.Set<TConversation>(); } }
        /// <summary>
        /// Gets the <see cref="DbSet{TMessage}"/>
        /// </summary>
        public DbSet<TMessage> Messages { get { return Context.Set<TMessage>(); } }
        /// <summary>
        /// Gets the <see cref="DbSet{TAttendee}"/>
        /// </summary>
        public virtual DbSet<TAttendee> Attendees { get { return Context.Set<TAttendee>(); } }
        /// <summary>
        /// Gets the <see cref="DbSet{TNotificationConnection}"/>
        /// </summary>
        public virtual DbSet<TNotificationConnection> NotificationConnections { get { return Context.Set<TNotificationConnection>(); } }
        /// <summary>
        /// Create a message on the database
        /// </summary>
        /// <param name="message">The message to create</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a <see cref="Task"/></returns>
        public virtual async Task CreateMessageAsync(TMessage message, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if(message == null)
                throw new ArgumentNullException("message");

            Context.Add(message);
            await Context.SaveChangesAsync(cancellationToken);
        }
        /// <summary>
        /// Create an attendee on the database
        /// </summary>
        /// <param name="attendee">The attendee to create</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a <see cref="Task"/></returns>
        public virtual async Task CreateAttendeeAsync(TAttendee attendee, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if(attendee == null)
            {
                throw new ArgumentNullException("attendee");
            }
            Context.Add(attendee);
            await Context.SaveChangesAsync(cancellationToken);
        }
        /// <summary>
        /// Create a conversation on the database
        /// </summary>
        /// <param name="conversation">The conversation to create</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a <see cref="Task"/></returns>
        public virtual async Task CreateConversationAsync(TConversation conversation, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (conversation == null)
                throw new ArgumentNullException("conversation");

            Context.Add(conversation);
            await Context.SaveChangesAsync(cancellationToken);
        }
        /// <summary>
        /// Find a user by her name
        /// </summary>
        /// <param name="userName">the user name</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a <see cref="Task{TUser}"/></returns>
        public virtual async Task<TUser> FindUserByNameAsync(string userName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await Users.Include(u => u.NotificationConnections).FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
        }
        /// <summary>
        /// Update the user on the database
        /// </summary>
        /// <param name="user">The user to update</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a <see cref="Task"/></returns>
        public virtual async Task UpdateUserAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException("user");
            
            Context.Update(user);
            await Context.SaveChangesAsync(cancellationToken);
        }
        /// <summary>
        /// Gets a conversation for 2 attendees
        /// </summary>
        /// <param name="attendee1">the 1st attendee</param>
        /// <param name="attendee2">the 2dn attendee</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a <see cref="Task{TConversation}"/></returns>
        public virtual async Task<TConversation> GetConversationAsync(TUser attendee1, TUser attendee2, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (attendee1 == null)
                throw new ArgumentNullException("attendee1");
            if (attendee2 == null)
                throw new ArgumentNullException("attendee2");

            var convs = from c in Conversations
                       join a1 in Attendees
                            on c.Id equals a1.ConversationId
                       join a2 in Attendees
                            on c.Id equals a2.ConversationId
                       where a1.UserId.Equals(attendee1.Id)
                            && a2.UserId.Equals(attendee2.Id)
                       select c;

            return await convs.FirstOrDefaultAsync(c => c.Attendees.Count == 2);
        }
        /// <summary>
        /// Gets a conversation by her id
        /// </summary>
        /// <param name="convId">the conversation id</param>
        /// <returns>a <see cref="Task{TConversation}"</returns>
        public virtual async Task<TConversation> GetConversationAsync(TKey convId, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await Conversations.FirstOrDefaultAsync(c => c.Id.Equals(convId));
        }
        /// <summary>
        /// Gets messages in a conversation
        /// </summary>
        /// <param name="convId">the conversation id</param>
        /// <param name="max">max number of messages to get, default is 50</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a <see cref="Task{IEnumerable{TMessage}}"/></returns>
        public virtual async Task<IEnumerable<TMessage>> GetMessagesAsync(TKey convId, int max = 50, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await Messages.Where(m => m.ConversationId.Equals(convId)).OrderByDescending(m=>m.Date).Take(max).ToListAsync();
        }
        /// <summary>
        /// Gets connected users
        /// </summary>
        /// <param name="pageIndex">the 0 based page index</param>
        /// <param name="pageLength">number of user per page</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a <see cref="Task{IEnumerable{TUser}}"/></returns>
        public virtual async Task<Page<TUser>> GetUsersConnectedAsync(int pageIndex = 0, int pageLength = 50, CancellationToken cancellationToken = default(CancellationToken))
        {
            var skip = pageIndex * pageLength;
            cancellationToken.ThrowIfCancellationRequested();
            var ids = new List<TKey>();
            var q1 = (from nc in NotificationConnections
                      group new { nc.UserId, nc.ConnectionDate } by nc.UserId into g
                      select new { Id = g.Key, Date = g.Max(x => x.ConnectionDate) })
                      .OrderByDescending(x => x.Date);

            var count = q1.Count();

            var q2 = await q1.Skip(skip)
                     .Take(pageLength)
                     .ToListAsync();

            var query = from r in q2
                        join u in Users
                         on r.Id equals u.Id
                        select u;

            var pageCount = (int)Math.Floor(((double)count) / pageLength) + 1;

            return await Task.FromResult(new Page<TUser>(query.ToList(), pageIndex, pageCount));
        }
        /// <summary>
        /// Create a notification connection on the database
        /// </summary>
        /// <param name="connection">the notification connection</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a <see cref="Task"/></returns>
        public virtual async Task CreateNotificationConnectionAsync(TNotificationConnection connection, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (connection == null)
                throw new ArgumentNullException("connection");

            Context.Add(connection);
            await Context.SaveChangesAsync(cancellationToken);
        }
        /// <summary>
        /// Delete a notification connection on the database
        /// </summary>
        /// <param name="connection">the notification connection</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a <see cref="Task"/></returns>
        public virtual async Task DeleteNotificationConnectionAsync(TNotificationConnection connection, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (connection == null)
                throw new ArgumentNullException("connection");

            Context.Remove(connection);
            await Context.SaveChangesAsync(cancellationToken);
        }
        /// <summary>
        /// Gets a notification connection by her id and her type
        /// </summary>
        /// <param name="connectionId">the notification connection id</param>
        /// <param name="notificationType">the type of notification</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a <see cref="Task{TNotificationConnection}"/></returns>
        public virtual async Task<TNotificationConnection> GetNotificationConnectionAsync(string connectionId, string notificationType, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (connectionId == null)
                throw new ArgumentNullException("connectionId");
            if (notificationType == null)
                throw new ArgumentNullException("notificationType");

            return await NotificationConnections.FirstOrDefaultAsync(c => c.ConnectionId.Equals(connectionId) && c.NotificationType.Equals(notificationType));
        }
        /// <summary>
        /// Initialise the database
        /// </summary>
        public virtual void Init()
        {
            Context.Database.EnsureCreated();
            NotificationConnections.RemoveRange(NotificationConnections.ToArray());
            Context.SaveChanges();
            Attendees.RemoveRange(Attendees.ToArray());
            Context.SaveChanges();
            Messages.RemoveRange(Messages.ToArray());
            Context.SaveChanges();
            Conversations.RemoveRange(Conversations.ToArray());
            Context.SaveChanges();
            Users.RemoveRange(Users.Where(u => u.PasswordHash == null).ToArray());
            Context.SaveChanges();
        }
        /// <summary>
        /// Gets notification connections for a user id and notification type
        /// </summary>
        /// <param name="userId">the user id</param>
        /// <param name="notificationType">the notification type</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a <see cref="Task{IEnumerable{TNotificationConnection}}"/></returns>
        public virtual async Task<IEnumerable<TNotificationConnection>> GetNotificationConnectionsAsync(TKey userId, string notificationType, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await NotificationConnections.Where(n => n.UserId.Equals(userId) && n.NotificationType == notificationType).ToListAsync();
        }
        /// <summary>
        /// Check if a user has connection
        /// </summary>
        /// <param name="user">the <see cref="TUser"/></param>
        /// <returns>true if user has connection</returns>
        public virtual async Task<bool> UserHasConnectionAsync(TKey userId)
        {
            return await NotificationConnections.AnyAsync(n => n.UserId.Equals(userId));
        }
        /// <summary>
        /// Gets attendees in a conversation
        /// </summary>
        /// <param name="conv">the conversation</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a <see cref="Task{IEnumerable{TAttendee}}"/></returns>
        public virtual async Task<IEnumerable<TAttendee>> GetAttendeesAsync(TConversation conv, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var attendees = await Attendees.Where(a => a.ConversationId.Equals(conv.Id)).ToListAsync();
            var atts = conv.Attendees;
            foreach(var attendee in attendees)
                if (!atts.Any(a => a.UserId.Equals(attendee.UserId)))
                    atts.Add(attendee);

            return attendees;
        }
        /// <summary>
        /// Gets conversations for a user id
        /// </summary>
        /// <param name="userId">the user id</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a <see cref="Task{IEnumerable{TConversation}}"/></returns>
        public virtual async Task<IEnumerable<TConversation>> GetConversationsAsync(TKey userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await (from c in Conversations
                          join a in Attendees
                              on c.Id equals a.ConversationId
                          join m in Messages
                              on c.Id equals m.ConversationId
                          where a.UserId.Equals(userId)
                          orderby m.Date descending                          
                          select c).Distinct().ToListAsync();
        }
        /// <summary>
        /// Deletes a user 
        /// </summary>
        /// <param name="user">the user to delete</param>
        /// <param name="cancellationToken"an optional cancellation token></param>
        /// <returns>a <see cref="Task"/></returns>
        public virtual async Task DeleteUserAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException("user");

            Messages.RemoveRange(await Messages.Where(m => m.UserId.Equals(user.Id)).ToArrayAsync());
            Attendees.RemoveRange(await Attendees.Where(a => a.UserId.Equals(user.Id)).ToArrayAsync());
            Conversations.RemoveRange(await Conversations.Where(c => c.Attendees.Count < 2).ToArrayAsync());
            NotificationConnections.RemoveRange(await NotificationConnections.Where(n => n.UserId.Equals(user.Id)).ToArrayAsync());
            Users.Remove(user);
            await Context.SaveChangesAsync(cancellationToken);
        }

		public async Task<TUser> FindUserByIdAsync(TKey id, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			return await Users.Include(u => u.NotificationConnections).FirstOrDefaultAsync(u => u.Id.Equals(id), cancellationToken);
		}
	}
}