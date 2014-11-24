using ChatLe.Models;
using Microsoft.Data.Entity;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.DependencyInjection.Fallback;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ChatLe.Repository.Text
{
    public class ChatStoreTest
    {
        public object HostingServices { get; private set; }

        [Fact]
        public void Construtor1Test()
        {
            ServiceCollection services = GetServicesCollection();

            using (var context = new DbContext(services.BuildServiceProvider()))
            {
                var store = new ChatStore(context);
            }
        }

        private static ServiceCollection GetServicesCollection()
        {
            var services = new ServiceCollection();
            services.AddEntityFramework()
                .AddInMemoryStore();
            services.AddInstance<ILoggerFactory>(new LoggerFactory());
            return services;
        }

        class UserTest : IApplicationUser<string>
        {
            public string Id
            {
                get
                {
                    throw new NotImplementedException();
                }

                set
                {
                    throw new NotImplementedException();
                }
            }

            public bool IsConnected
            {
                get
                {
                    throw new NotImplementedException();
                }

                set
                {
                    throw new NotImplementedException();
                }
            }

            public string SignalRConnectionId
            {
                get
                {
                    throw new NotImplementedException();
                }

                set
                {
                    throw new NotImplementedException();
                }
            }

            public string UserName
            {
                get
                {
                    throw new NotImplementedException();
                }

                set
                {
                    throw new NotImplementedException();
                }
            }
        }
        [Fact]
        public void Construtor2Test()
        {
            ServiceCollection services = GetServicesCollection();

            using (var context = new DbContext(services.BuildServiceProvider()))
            {
                var store = new ChatStore<UserTest>(context);
            }
        }        

        [Fact]        
        public void ConstrutorFailTest()
        {
            Assert.Throws(typeof(ArgumentNullException), () => new ChatStore<string, UserTest, FakeContextTest>(null));            
        }

        class FakeContextTest : DbContext
        {
            public FakeContextTest(IServiceProvider provider) :base(provider) { }
        }

        class ChatDbContext : DbContext
        {
            public ChatDbContext(IServiceProvider provider) :base(provider) { }

            public DbSet<Message> Messages { get; set; }

            public DbSet<Conversation> Conversations { get; set; }
        }

        [Fact]
        public void ConversationsPropertyTest()
        {
            ServiceCollection services = GetServicesCollection();

            using (var context = new FakeContextTest(services.BuildServiceProvider()))
            {
                var store = new ChatStore<string, UserTest, FakeContextTest>(context);
                var conversations = store.Conversations;
                Assert.NotNull(conversations);
                Assert.IsType<DbSet<Conversation<string>>>(conversations);
            }
        }
        [Fact]
        public void MessagesPropertyTest()
        {
            ServiceCollection services = GetServicesCollection();

            using (var context = new FakeContextTest(services.BuildServiceProvider()))
            {
                var store = new ChatStore<string, UserTest, FakeContextTest>(context);
                var messages = store.Messages;
                Assert.NotNull(messages);
                Assert.IsType<DbSet<Message<string>>>(messages);
            }
        }
        [Fact]
        public void UsersPropertyTest()
        {
            ServiceCollection services = GetServicesCollection();

            using (var context = new FakeContextTest(services.BuildServiceProvider()))
            {
                var store = new ChatStore<string, UserTest, FakeContextTest>(context);
                var users = store.Users;
                Assert.NotNull(users);
                Assert.IsType<DbSet<UserTest>>(users);
            }
        }
        [Fact]
        public async Task AddMessageAsyncTest()
        {
            ServiceCollection services = GetServicesCollection();

            using (var context = new ChatDbContext(services.BuildServiceProvider()))
            {
                var store = new ChatStore<string, UserTest, ChatDbContext>(context);
                var message = new Message()
                {
                    Date = DateTime.UtcNow,
                    Text = "test",
                    UserId = "test",
                };
                var conversation = new Conversation();
                await store.AddMessageAsync(conversation, message);
                Assert.Equal(message.ConversationId, conversation.Id);
                Assert.Contains(message, conversation.Messages);
            }
        }
    }
}
