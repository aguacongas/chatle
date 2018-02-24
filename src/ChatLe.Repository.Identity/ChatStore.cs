using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

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
        public ChatStore(ChatLeIdentityDbContext context, IHostingEnvironment env) 
            : base(context, env) { }
    }
    
    /// <summary>
    /// Chat store for TUser
    /// </summary>
    /// <typeparam name="TUser">type of user, must a class and implement <see cref="IChatUser{string}"/></typeparam>
    public class ChatStore<TUser> : ChatStore<string, TUser, DbContext, Conversation, Attendee, Message, NotificationConnection, IdentityUserLogin<string>>
        where TUser : IdentityUser<string>, IChatUser<string>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The <see cref="DbContext" to use/></param>
        /// <param name="loggerFactory"></param>
        public ChatStore(DbContext context, IHostingEnvironment env) 
            : base(context, env) { }
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
    /// <typeparam name="TNotificationConnection">type of notifciation connection, must be a <see cref="NotificationConnection{TKey}"/></typeparam> <summary>
    /// <typeparam name="TUserLogin">type of the user login object.</typeparam>
    /// </summary>
    public class ChatStore<TKey, TUser, TContext, TConversation, TAttendee, TMessage, TNotificationConnection, TUserLogin> :IChatStore<TKey,TUser, TConversation, TAttendee, TMessage, TNotificationConnection>
        where TKey : IEquatable<TKey>
        where TUser : IdentityUser<TKey>, IChatUser<TKey>
        where TContext : DbContext
        where TConversation : Conversation<TKey>
        where TAttendee : Attendee<TKey>
        where TMessage : Message<TKey>
        where TNotificationConnection : NotificationConnection<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
    {
        readonly IHostingEnvironment _env;
        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="context">The <see cref="DbContext" to use/></param>
        /// <param name="loggerFactory"></param>
        public ChatStore(TContext context, IHostingEnvironment env)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            Context = context;
            _env = env;
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
        /// Gets the <see cref="DbSet{TUserLogin}"/>
        /// </summary>
        public virtual DbSet<TUserLogin> Logins { get { return Context.Set<TUserLogin>(); } }
        
        /// <summary>
        /// Create a message on the database
        /// </summary>
        /// <param name="message">The message to create</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a <see cref="Task"/></returns>
        public virtual Task CreateMessageAsync(TMessage message, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if(message == null)
                throw new ArgumentNullException("message");

            Messages.Add(message);
            Context.SaveChanges();

            return Task.FromResult(0);
        }

        /// <summary>
        /// Create an attendee on the database
        /// </summary>
        /// <param name="attendee">The attendee to create</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a <see cref="Task"/></returns>
        public virtual Task CreateAttendeeAsync(TAttendee attendee, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if(attendee == null)
            {
                throw new ArgumentNullException("attendee");
            }
            Attendees.Add(attendee);
            Context.SaveChanges();

            return Task.FromResult(0);
        }

        /// <summary>
        /// Create a conversation on the database
        /// </summary>
        /// <param name="conversation">The conversation to create</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a <see cref="Task"/></returns>
        public virtual Task CreateConversationAsync(TConversation conversation, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (conversation == null)
                throw new ArgumentNullException("conversation");

            Conversations.Add(conversation);
            Context.SaveChanges();

            return Task.FromResult(0);
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
            return await Users
                .SingleOrDefaultAsync(u => u.UserName == userName, cancellationToken);
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

            var convs = (from c in Conversations
                       join a1 in Attendees
                            on c.Id equals a1.ConversationId
                       join a2 in Attendees
                            on c.Id equals a2.ConversationId
                       where a1.UserId.Equals(attendee1.Id)
                            && a2.UserId.Equals(attendee2.Id)
                            && c.Attendees.Count.Equals(2)
                       select c)
                       .Include(c => c.Attendees);

            return await convs.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets a conversation by her id
        /// </summary>
        /// <param name="convId">the conversation id</param>
        /// <returns>a <see cref="Task{TConversation}"</returns>
        public virtual async Task<TConversation> GetConversationAsync(TKey convId, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await Conversations
                .Include(c => c.Attendees)
                .FirstOrDefaultAsync(c => c.Id.Equals(convId));
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
        public virtual Task CreateNotificationConnectionAsync(TNotificationConnection connection, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (connection == null)
                throw new ArgumentNullException("connection");

            NotificationConnections.Add(connection);
            try
            {
                Context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                foreach (var entry in ex.Entries)
                {
                    var notification = entry.Entity as NotificationConnection<TKey>;
                    if (entry.Entity != null)
                    {
                        entry.State = EntityState.Added;
                        // Using a NoTracking query means we get the entity but it is not tracked by the context
                        // and will not be merged with existing entities in the context.
                        var connectionId = connection.ConnectionId;
                        var type = connection.NotificationType;

                        var databaseEntity = NotificationConnections
                            .AsNoTracking()
                            .FirstOrDefault(nc => nc.ConnectionId == connectionId && nc.NotificationType == type);

                        if (databaseEntity == null)
                        {
                            var databaseEntry = Context.Entry(connection);
                            ResetDbEntry<TNotificationConnection>(databaseEntry);
                        }
                    }
                    else
                    {
                        throw new NotSupportedException("Don't know how to handle concurrency conflicts for " + entry.Metadata.Name);
                    }
                }

                // Retry the save operation
                Context.SaveChanges();
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Delete a notification connection on the database
        /// </summary>
        /// <param name="connection">the notification connection</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a <see cref="Task"/></returns>
        public virtual Task DeleteNotificationConnectionAsync(TNotificationConnection connection, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (connection == null)
                throw new ArgumentNullException("connection");

            NotificationConnections.Remove(connection);

            try
            {
                Context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                RetryDeleteNotificationConnection(ex);
            }

            return Task.FromResult(0);
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

            return await NotificationConnections
                .FirstOrDefaultAsync(c => c.ConnectionId == connectionId && c.NotificationType == notificationType);
        }
        
        /// <summary>
        /// Initialise the database
        /// </summary>
        public virtual void Init()
        {
            if (_env!= null &&  _env.IsDevelopment())
                Context.Database.EnsureDeletedAsync();

            Context.Database.EnsureCreated();
            NotificationConnections.RemoveRange(NotificationConnections.ToArray());
            Context.SaveChanges();
            Attendees.RemoveRange(Attendees.ToArray());
            Context.SaveChanges();
            Messages.RemoveRange(Messages.ToArray());
            Context.SaveChanges();
            Conversations.RemoveRange(Conversations.ToArray());
            Context.SaveChanges();
            Users.RemoveRange(Users.Where(u => IsGuess(u, default(CancellationToken)).Result).ToArray());
            Context.SaveChanges();
        }
        
        public virtual async Task<bool> IsGuess(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Logins.AnyAsync(l => l.UserId.Equals(user.Id), cancellationToken) == false;
        }
        /// <summary>
        /// Check if a user has connection
        /// </summary>
        /// <param name="user">the user</param>
        /// <returns>true if user has connection</returns>
        public virtual async Task<bool> UserHasConnectionAsync(TUser user)
        {
            return await NotificationConnections.AnyAsync(n => n.UserId.Equals(user.Id));
        }
        
        /// <summary>
        /// Gets conversations for a user id
        /// </summary>
        /// <param name="user">the user</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a <see cref="Task{IEnumerable{TConversation}}"/></returns>
        public virtual async Task<IEnumerable<TConversation>> GetConversationsAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await (from c in Conversations
                          join a in Attendees
                              on c.Id equals a.ConversationId
                          join m in Messages
                              on c.Id equals m.ConversationId
                          where a.UserId.Equals(user.Id)
                          orderby m.Date descending                          
                          select c).Include(c => c.Attendees).Distinct().ToListAsync();
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

            // Remove all conversations the user attends
            var conversations = await GetConversationsAsync(user, cancellationToken);
            foreach (var conversation in conversations)
            {
                Messages.RemoveRange(Messages.Where(m => m.ConversationId.Equals(conversation.Id)));
                Attendees.RemoveRange(Attendees.Where(a => a.ConversationId.Equals(conversation.Id)));
                Conversations.Remove(conversation);
            }
                            
            var userConnections = NotificationConnections.Where(n => n.UserId.Equals(user.Id));
            NotificationConnections.RemoveRange(userConnections);
            
            Users.Remove(user);
            
            try
            {
                Context.SaveChanges();            
            }
            catch (DbUpdateConcurrencyException ex)
            {
                RetryDeleteUser(ex);
            }
        }

		public async Task<TUser> FindUserByIdAsync(TKey id, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
            var user = await Users.SingleOrDefaultAsync(u => u.Id.Equals(id), cancellationToken);
            return user;
		}

        void ResetDbEntry<TEntity>(EntityEntry<TEntity> entry) where TEntity : class
        {
            foreach (var property in entry.Metadata.GetProperties())
            {
                if (property.IsKey())
                    continue;

                entry.Property(property.Name).IsModified = false;
            }
        }

        void RetryDeleteNotificationConnection(DbUpdateConcurrencyException ex)
        {
            foreach (var entry in ex.Entries)
            {
                RetryDeleteNotificationConnection(entry);
            }
        }

        void RetryDeleteNotificationConnection(EntityEntry entry)
        {
            var notification = entry.Entity as NotificationConnection<TKey>;
            if (notification != null)
            {
                // Using a NoTracking query means we get the entity but it is not tracked by the context
                // and will not be merged with existing entities in the context.
                var databaseEntity = NotificationConnections.AsNoTracking().SingleOrDefault(nc => nc.ConnectionId == notification.ConnectionId && nc.NotificationType == notification.NotificationType);
                if (databaseEntity == null)
                    return;

                var databaseEntry = Context.Entry(databaseEntity);

                ResetDbEntry(databaseEntry);
            }
            else
            {
                throw new NotSupportedException("Don't know how to handle concurrency conflicts for " + entry.Metadata.Name);
            }
        }


        void RetryDeleteUser(DbUpdateConcurrencyException ex)
        {
            foreach (var entry in ex.Entries)
            {
                if (entry.Entity is TNotificationConnection)
                {
                    RetryDeleteNotificationConnection(entry);
                }
                if (entry.Entity is TMessage)
                {
                    RetryDeleteEntity<TMessage>(entry, Messages);
                }
                if (entry.Entity is TAttendee)
                {
                    RetryDeleteEntity<TAttendee>(entry,  entity => Attendees.AsNoTracking().SingleOrDefault(a => a.ConversationId.Equals(entity.ConversationId) && a.UserId.Equals(entity.UserId)));
                }
                if (entry.Entity is TUser)
                {
                    RetryDeleteEntity<TUser>(entry, Users);
                }
            }
        }

        void RetryDeleteEntity<TIdentifiable>(EntityEntry entry, DbSet<TIdentifiable> dbSet) where TIdentifiable: class, IIdentifiable<TKey>
        {
            RetryDeleteEntity<TIdentifiable>(entry, entity => dbSet.AsNoTracking().SingleOrDefault(m => m.Id.Equals(entity as TIdentifiable)));
        }

        void RetryDeleteEntity<TEntity>(EntityEntry entry, Func<TEntity, TEntity> getEntity) where TEntity: class
        {
            var entity = entry.Entity as TEntity;
            if (entity != null)
            {
                // Using a NoTracking query means we get the entity but it is not tracked by the context
                // and will not be merged with existing entities in the context.
                var databaseEntity = getEntity(entity);
                if (databaseEntity == null)
                    return;

                var databaseEntry = Context.Entry(databaseEntity);

                ResetDbEntry(databaseEntry);
            }
            else
            {
                throw new NotSupportedException("Don't know how to handle concurrency conflicts for " + entry.Metadata.Name);
            }
        }
	}
}