using ChatLe.Models;
using Microsoft.Data.Entity;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.DependencyInjection.Fallback;
using Microsoft.Framework.Logging;
using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Data.Entity.Metadata;
using System.Collections.Generic;
using System.Linq;

namespace ChatLe.Repository.Text
{
    public class ChatStoreTest
    {       
        [Fact]
        public void Construtor1Test()
        {
            ServiceCollection services = GetServicesCollection();

            using (var context = new DbContext(services.BuildServiceProvider()))
            {
                var store = new ChatStore<UserTest>(context);
            }
        }

        private ServiceCollection GetServicesCollection()
        {
            return TestHelpers.GetServicesCollection();
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
            Assert.Throws(typeof(ArgumentNullException), () => new ChatStore<string, UserTest, FakeContextTest, Conversation, Attendee, Message>(null));            
        }

        class FakeContextTest : DbContext
        {
            public FakeContextTest(IServiceProvider provider) :base(provider) { }
        }

        [Fact]
        public void ConversationsPropertyTest()
        {
            ServiceCollection services = GetServicesCollection();

            using (var context = new FakeContextTest(services.BuildServiceProvider()))
            {
                var store = new ChatStore<string, UserTest, FakeContextTest, Conversation, Attendee, Message>(context);
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
                var store = new ChatStore<string, UserTest, FakeContextTest, Conversation, Attendee, Message>(context);
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
                var store = new ChatStore<string, UserTest, FakeContextTest, Conversation, Attendee, Message>(context);
                var users = store.Users;
                Assert.NotNull(users);
                Assert.IsType<DbSet<UserTest>>(users);
            }
        }
        [Fact]
        public async Task CreateMessageAsyncTest()
        {
            ServiceCollection services = GetServicesCollection();

            using (var context = new ChatDbContext(services.BuildServiceProvider()))
            {
                var store = new ChatStore<string, UserTest, ChatDbContext, Conversation, Attendee, Message>(context);
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
            ServiceCollection services = GetServicesCollection();

            using (var context = new ChatDbContext(services.BuildServiceProvider()))
            {
                var store = new ChatStore<string, UserTest, ChatDbContext, Conversation, Attendee, Message>(context);
                var user = new UserTest()
                {
                    Id = "test", UserName = "test", IsConnected = false
                };
                await store.UpdateUserAsync(user);
            }
        }

        [Fact]
        public async Task GetUsersConnectedAsyncTest()
        {
            ServiceCollection services = GetServicesCollection();

            using (var context = new ChatDbContext(services.BuildServiceProvider()))
            {
                var connected = new UserTest()
                {
                    Id = "connected",
                    UserName = "connected",
                    IsConnected = true
                };
                var notConnected = new UserTest()
                {
                    Id = "notConnected",
                    UserName = "notConnected",
                    IsConnected = false
                };
                context.Users.Add(connected);
                context.Users.Add(notConnected);
                context.SaveChanges();
                var store = new ChatStore<string, UserTest, ChatDbContext, Conversation, Attendee, Message>(context);

                var users = await store.GetUsersConnectedAsync();
                Assert.True(users.Count() == 1);
                Assert.True(users.FirstOrDefault() == connected);
            }
        }
    }
}
