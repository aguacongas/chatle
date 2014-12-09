using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using System;
using System.Diagnostics;

namespace ChatLe.Models
{
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
        /// <summary>
        /// Constructor
        /// </summary>
        public ChatLeIdentityDbContext()
        {
            Trace.TraceInformation("[ApplicationDbContext] constructor");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<NotificationConnection>(b =>
            {
                b.Key(n => new { n.ConnectionId, n.NotificationType });
                b.ForeignKey<ChatLeUser>(n => n.UserId);
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
                b.ForeignKey<ChatLeUser>(m => m.UserId);
                b.ForeignKey<Conversation>(m => m.ConversationId);
                b.ForRelational().Table("Messages");
            });

            builder.Entity<Attendee>(b =>
            {
                b.Key(a => new { a.ConversationId, a.UserId });
                b.ForeignKey<Conversation>(a => a.ConversationId);
                b.ForRelational().Table("Attendees");
            });            
        }

        protected override void OnConfiguring(DbContextOptions options)
        {
            //options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=aspnet5-chatle-8aed7be5-ad19-4e43-91f6-a67ba01d2830;Trusted_Connection=True;MultipleActiveResultSets=true");
            base.OnConfiguring(options);
        }
    }
}