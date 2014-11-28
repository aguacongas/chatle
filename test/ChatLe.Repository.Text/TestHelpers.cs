using ChatLe.Models;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using System;
using System.Collections.Generic;

namespace ChatLe.Repository.Text
{
    public static class TestHelpers
    {
        public static ServiceCollection GetServicesCollection()
        {
            var services = new ServiceCollection();
            services.AddEntityFramework()
                .AddInMemoryStore();
            services.AddInstance<ILoggerFactory>(new LoggerFactory());
            return services;
        }
    }
    public class UserTest : IChatUser<string>
    {
        public string Id { get; set; } = "test";

        public bool IsConnected { get; set; }

        public ICollection<string> SignalRConnectionIds { get; } = new List<string>();

        public string UserName { get; set; } = "test";
    }
    public class ChatDbContext : DbContext
    {
        public ChatDbContext(IServiceProvider provider) : base(provider) { }

        public DbSet<UserTest> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Attendee> Attendee { get; set; }
        public DbSet<Conversation> Conversations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UserTest>(b =>
            {
                b.Key(u => u.Id);
            });

            builder.Entity<Conversation>(b =>
            {
                b.Key(c => c.Id);
            });

            builder.Entity<Message>(b =>
            {
                b.Key(m => m.Id);
                b.ForeignKey<UserTest>(m => m.UserId);
                b.ForeignKey<Conversation>(m => m.ConversationId);
            });

            builder.Entity<Attendee>(b =>
            {
                b.Key(a => new { a.ConversationId, a.UserId });
                b.ForeignKey<Conversation>(a => a.ConversationId);
            });

        }
    }
}