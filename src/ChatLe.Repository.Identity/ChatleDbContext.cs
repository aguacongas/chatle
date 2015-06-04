using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;

namespace ChatLe.Models
{
    public class ChatLeIdentityDbContextSql : ChatLeIdentityDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // TODO: uncomment this line to create an EF migration
            options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=chatle;Trusted_Connection=True;MultipleActiveResultSets=true");
            base.OnConfiguring(options);
        }
    }
    /// <summary>
    /// Database context for ChatLe user
    /// </summary>
    public class ChatLeIdentityDbContext : IdentityDbContext<ChatLeUser>
    {
        /// <summary>
        /// Gets or sets the DbSet of messages
        /// </summary>
        public DbSet<Message> Messages { get; set; }
        /// <summary>
        /// Gets or sets the DbSet of conversations
        /// </summary>
        public DbSet<Conversation> Conversations { get; set; }
        /// <summary>
        /// Gets or sets the DbSet of attendees
        /// </summary>
        public DbSet<Attendee> Attendees { get; set; }
        /// <summary>
        /// Gets or sets the DbSet of notification connections
        /// </summary>
        public DbSet<NotificationConnection> NotificationConnections { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<NotificationConnection>(b =>
            {
                b.Key(n => new { n.ConnectionId, n.NotificationType });
                b.Reference<ChatLeUser>().InverseCollection().ForeignKey(n => n.UserId);
                b.ForRelational().Table("NotificationConnections");
            });


            builder.Entity<Conversation>(b =>
            {
                b.Key(c => c.Id);
                b.ForRelational().Table("Conversations");
            });

            builder.Entity<Message>(b =>
            {
                b.Key(m => m.Id);
                b.Reference<ChatLeUser>().InverseCollection().ForeignKey(m => m.UserId);
                b.Reference<Conversation>().InverseCollection().ForeignKey(m => m.ConversationId);
                b.ForRelational().Table("Messages");
            });

            builder.Entity<Attendee>(b =>
            {
                b.Key(a => new { a.ConversationId, a.UserId });
                b.Reference<Conversation>().InverseCollection().ForeignKey(a => a.ConversationId);
                b.Reference<ChatLeUser>().InverseCollection().ForeignKey(a => a.UserId);
                b.ForRelational().Table("Attendees");
            });

            base.OnModelCreating(builder);
        }


    }
}