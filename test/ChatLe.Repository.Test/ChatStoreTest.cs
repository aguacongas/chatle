using ChatLe.Models;
using Microsoft.Data.Entity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ChatLe.Repository.Text
{
    public class ChatStoreTest
    {
        [Fact]
        public void Construtor1Test()
        {
            ServiceCollection services = GetServicesCollection<DbContext>();

            using (var context = new DbContext(services.BuildServiceProvider()))
            {
                var store = new ChatStore<UserTest>(context);
            }
        }

        private ServiceCollection GetServicesCollection<T>() where T : DbContext
        {
            return TestHelpers.GetServicesCollection<T>();
        }

        [Fact]
        public void Construtor2Test()
        {
            ServiceCollection services = GetServicesCollection<DbContext>();

            using (var context = new DbContext(services.BuildServiceProvider()))
            {
                var store = new ChatStore<UserTest>(context);
            }
        }

        [Fact]
        public void ConstrutorFailTest()
        {
            Assert.Throws(typeof(ArgumentNullException), () => new ChatStore<string, UserTest, FakeContextTest, Conversation, Attendee, Message, NotificationConnection>(null));
        }

        class FakeContextTest : DbContext
        {
            public FakeContextTest(IServiceProvider provider) : base(provider) { }
        }

        [Fact]
        public void ConversationsPropertyTest()
        {
            ServiceCollection services = GetServicesCollection<FakeContextTest>();

            using (var context = new FakeContextTest(services.BuildServiceProvider()))
            {
                var store = new ChatStore<string, UserTest, FakeContextTest, Conversation, Attendee, Message, NotificationConnection>(context);
                var conversations = store.Conversations;
                Assert.NotNull(conversations);
                Assert.IsAssignableFrom<DbSet<Conversation>>(conversations);
            }
        }

        [Fact]
        public void MessagesPropertyTest()
        {
            ServiceCollection services = GetServicesCollection<FakeContextTest>();

            using (var context = new FakeContextTest(services.BuildServiceProvider()))
            {
                var store = new ChatStore<string, UserTest, FakeContextTest, Conversation, Attendee, Message, NotificationConnection>(context);
                var messages = store.Messages;
                Assert.NotNull(messages);
                Assert.IsAssignableFrom<DbSet<Message>>(messages);
            }
        }

        [Fact]
        public void UsersPropertyTest()
        {
            ServiceCollection services = GetServicesCollection<FakeContextTest>();

            using (var context = new FakeContextTest(services.BuildServiceProvider()))
            {
                var store = new ChatStore<string, UserTest, FakeContextTest, Conversation, Attendee, Message, NotificationConnection>(context);
                var users = store.Users;
                Assert.NotNull(users);
                Assert.IsAssignableFrom<DbSet<UserTest>>(users);
            }
        }

        [Fact]
        public async Task CreateMessageAsyncTest()
        {
            ServiceCollection services = GetServicesCollection<ChatDbContext>();

            using (var context = new ChatDbContext(services.BuildServiceProvider()))
            {
                var store = new ChatStore<string, UserTest, ChatDbContext, Conversation, Attendee, Message, NotificationConnection>(context);
                var message = new Message()
                {
                    ConversationId = "test",
                    Date = DateTime.UtcNow,
                    Text = "test",
                    UserId = "test",
                };
                await store.CreateMessageAsync(message);
            }
        }

        [Fact]
        public async Task UpdateUserAsyncTest()
        {
            ServiceCollection services = GetServicesCollection<ChatDbContext>();

            using (var context = new ChatDbContext(services.BuildServiceProvider()))
            {
                await context.Database.EnsureCreatedAsync();
                var store = new ChatStore<string, UserTest, ChatDbContext, Conversation, Attendee, Message, NotificationConnection>(context);
                var user = new UserTest()
                {
                    Id = "test",
                    UserName = "test"
                };
                store.Context.Add(user);
                await store.Context.SaveChangesAsync();

                await store.UpdateUserAsync(user);
            }
        }

        [Fact]
        public async Task GetUsersConnectedAsyncTest()
        {
            ServiceCollection services = GetServicesCollection<ChatDbContext>();

            using (var context = new ChatDbContext(services.BuildServiceProvider()))
            {
                var connected = new UserTest()
                {
                    Id = "connected",
                    UserName = "connected"
                };
                context.Users.Add(connected);

                var nc = new NotificationConnection()
                {
                    ConnectionId = "test",
                    UserId = "connected",
                    NotificationType = "test"
                };
                context.NotificationConnections.Add(nc);

                var notConnected = new UserTest()
                {
                    Id = "notConnected",
                    UserName = "notConnected",
                };                
                context.Users.Add(notConnected);

                context.SaveChanges();
                var store = new ChatStore<string, UserTest, ChatDbContext, Conversation, Attendee, Message, NotificationConnection>(context);

                var users = await store.GetUsersConnectedAsync();
                Assert.True(users.Count() == 1);
                Assert.True(users.FirstOrDefault() == connected);
            }
        }

        [Fact]
        public async Task NotificationConnectionAsyncTest()
        {
            ServiceCollection services = GetServicesCollection<ChatDbContext>();

            using (var context = new ChatDbContext(services.BuildServiceProvider()))
            {
                var user = new UserTest()
                {
                    Id = "test",
                    UserName = "test",
                };
                context.Users.Add(user);
                context.SaveChanges();

                var nc = new NotificationConnection()
                {
                    ConnectionId = "test",
                    UserId = "test",
                    NotificationType = "test"
                };

                var store = new ChatStore<string, UserTest, ChatDbContext, Conversation, Attendee, Message, NotificationConnection>(context);
                await store.CreateNotificationConnectionAsync(nc);
                nc = await store.GetNotificationConnectionAsync("test", "test");
                await store.DeleteNotificationConnectionAsync(nc);
            }
        }

        [Fact]
        public async Task GetConversationsAsyncTest()
        {
            ServiceCollection services = GetServicesCollection<ChatDbContext>();

            using (var context = new ChatDbContext(services.BuildServiceProvider()))
            {
                var conv = new Conversation();
                context.Conversations.Add(conv);
                var attendee = new Attendee()
                {
                    ConversationId = conv.Id,
                    UserId = "test"
                };
                context.Attendee.Add(attendee);

                var message = new Message()
                {
                    ConversationId = conv.Id,
                    UserId = "test",
                    Text = "test",
                    Date = DateTime.UtcNow
                };

                context.Messages.Add(message);
                context.SaveChanges();

                var store = new ChatStore<string, UserTest, ChatDbContext, Conversation, Attendee, Message, NotificationConnection>(context);
                var convs = await store.GetConversationsAsync("test");
                Assert.NotNull(convs);
                Assert.True(convs.Count() == 1);
                Assert.NotNull(convs.FirstOrDefault());
                Assert.Equal(conv, convs.First());
            }
        }
    }
}
