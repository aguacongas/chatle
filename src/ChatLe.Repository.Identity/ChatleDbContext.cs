using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;

namespace ChatLe.Models
{
    public class ChatLeIdentityDbContextSql : ChatLeIdentityDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
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
            base.OnModelCreating(builder);

            builder.Entity<ChatLeUser>()
                .HasMany(u => u.NotificationConnections).WithOne().HasForeignKey(nc => new { nc.ConnectionId, nc.NotificationType });

            builder.Entity<NotificationConnection>(b =>
            {
                b.HasKey(n => new { n.ConnectionId, n.NotificationType });
                b.ToTable("NotificationConnections");
            });


            builder.Entity<Conversation>(b =>
            {
                b.HasKey(c => c.Id);
                b.ToTable("Conversations");

                b.HasMany(c => c.Attendees).WithOne().HasForeignKey(a => a.ConversationId);
                b.HasMany(c => c.Messages).WithOne().HasForeignKey(m => m.ConversationId);
            });

            builder.Entity<Message>(b =>
            {
                b.HasKey(m => m.Id);
                b.ToTable("Messages");
            });

            builder.Entity<Attendee>(b =>
            {
                b.HasKey(a => new { a.ConversationId, a.UserId });
                b.ToTable("Attendees");                
            });            
        }
    }
}