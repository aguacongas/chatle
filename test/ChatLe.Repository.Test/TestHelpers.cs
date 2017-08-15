using ChatLe.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ChatLe.Repository.Test
{
    public class UserTest: UserTest<string>
    {
        public UserTest()
        {
            Id = "test";
        }
    }

    public class UserTest<TKey> : IdentityUser<TKey>, IChatUser<TKey> where TKey: IEquatable<TKey>
    {
        public override TKey Id { get; set; }

        public ICollection<NotificationConnection<TKey>> NotificationConnections { get; set; } = new List<NotificationConnection<TKey>>();

        public bool IsGuess { get; set; } = true;

        public override string UserName { get; set;} = "test";

        public DateTime LastLoginDate { get; set; } = DateTime.Now;
    }

    public class ChatDbContext:ChatDbContext<string, UserTest, Message, Attendee, Conversation, NotificationConnection>
    {
        public ChatDbContext(DbContextOptions options) : base(options) { }
    }

    public class ChatDbContext<TKey, TUser, TMessage, TAttendee, TConversation, TNotificationConnection> : DbContext 
        where TKey:IEquatable<TKey>
        where TUser : UserTest<TKey>
        where TMessage: Message<TKey>
        where TAttendee : Attendee<TKey>
        where TConversation : Conversation<TKey>
        where TNotificationConnection : NotificationConnection<TKey>
    {
        public ChatDbContext(DbContextOptions options) : base(options) { }

        public DbSet<TUser> Users { get; set; }
        public DbSet<TMessage> Messages { get; set; }
        public DbSet<TAttendee> Attendee { get; set; }
        public DbSet<TConversation> Conversations { get; set; }
        public DbSet<TNotificationConnection> NotificationConnections { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("test");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserTest>(b =>
            {
                b.HasKey(u => u.Id);
                b.ToTable("TestUsers");

                b.HasMany(u => u.NotificationConnections).WithOne().HasForeignKey(nc => nc.UserId);
                
            });

            builder.Entity<Message<TKey>>(b =>
            {
                b.HasKey(m => m.Id);
                b.ToTable("Messages");
            });

            builder.Entity<Conversation<TKey>>(b =>
            {
                b.HasKey(c => c.Id);
                b.ToTable("Conversations");

                b.HasMany(c => c.Attendees)
                    .WithOne()
                    .HasForeignKey(a => a.ConversationId)
                    .IsRequired();

                b.HasMany(c => c.Messages)
                    .WithOne()
                    .HasForeignKey(m => m.ConversationId)
                    .IsRequired();                
            });

            builder.Entity<NotificationConnection<TKey>>(b =>
            {
                b.HasKey(n => new { n.ConnectionId, n.NotificationType });
                b.ToTable("NotificationConnections");
            });

            
            builder.Entity<Attendee<TKey>>(b => 
            {
                b.HasKey(a => new { a.UserId, a.ConversationId });
                b.ToTable("Attendees");
            });
                
        }
    }
}