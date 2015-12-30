using ChatLe.ViewModels;
using Microsoft.Data.Entity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ChatLe.Repository.Test
{
    public static class TestHelpers
    {
        public static ServiceCollection GetServicesCollection<T>() where T :DbContext
        {
            var services = new ServiceCollection();
            services.AddEntityFramework()
                .AddInMemoryDatabase()
                .AddDbContext<T>(options => options.UseInMemoryDatabase());
            services.AddInstance<ILoggerFactory>(new LoggerFactory());

            return services;
        }
    }

    public class UserTest: UserTest<string>
    {
        public UserTest()
        {
            Id = "test";
        }
    }

    public class UserTest<TKey> : IChatUser<TKey> where TKey: IEquatable<TKey>
    {
        public TKey Id { get; set; }

        public ICollection<NotificationConnection<TKey>> NotificationConnections { get; set; } = new List<NotificationConnection<TKey>>();

        public string UserName { get; set; } = "test";

        public string PasswordHash { get; set; } = null;

    }

    public class ChatDbContext:ChatDbContext<string, UserTest, Message, Attendee, Conversation, NotificationConnection>
    {
        public ChatDbContext(IServiceProvider provider) : base(provider) { }
    }

    public class ChatDbContext<TKey, TUser, TMessage, TAttendee, TConversation, TNotificationConnection> : DbContext 
        where TKey:IEquatable<TKey>
        where TUser : UserTest<TKey>
        where TMessage: Message<TKey>
        where TAttendee : Attendee<TKey>
        where TConversation : Conversation<TKey>
        where TNotificationConnection : NotificationConnection<TKey>
    {
        public ChatDbContext(IServiceProvider provider) : base(provider) { }

        public DbSet<TUser> Users { get; set; }
        public DbSet<TMessage> Messages { get; set; }
        public DbSet<TAttendee> Attendee { get; set; }
        public DbSet<TConversation> Conversations { get; set; }
        public DbSet<TNotificationConnection> NotificationConnections { get; set; }

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