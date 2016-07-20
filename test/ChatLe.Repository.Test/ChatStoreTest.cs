using ChatLe.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ChatLe.Repository.Test
{
    public class ChatStoreTest
    {
        [Fact]
        public void ConstrutorTest()
        {
            using (var context = new DbContext(new DbContextOptionsBuilder().Options))
            {
                var store = new ChatStore<UserTest>(context);
            }
        }


        [Fact]
        public void Construtor_should_throw_argumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ChatStore<string, UserTest, FakeContextTest, Conversation, Attendee, Message, NotificationConnection>(null));
        }

        class FakeContextTest : DbContext
        {
            public FakeContextTest(DbContextOptions options) : base(options) { }
            
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseInMemoryDatabase();
            }

            public DbSet<UserTest> Users { get; set; }
            public DbSet<Message> Messages { get; set; }
            public DbSet<Attendee> Attendee { get; set; }
            public DbSet<Conversation> Conversations { get; set; }
            public DbSet<NotificationConnection> NotificationConnections { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Attendee<string>>()
                    .HasKey(a => new { a.ConversationId, a.UserId });
                
                modelBuilder.Entity<NotificationConnection<string>>()
                    .HasKey(nc => new { nc.ConnectionId, nc.UserId, nc.NotificationType });
            }
        }

        [Fact]
        public void ConversationsPropertyTest()
        {
            using (var context = new FakeContextTest(new DbContextOptionsBuilder().Options))
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
            using (var context = new FakeContextTest(new DbContextOptionsBuilder().Options))
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
            using (var context = new FakeContextTest(new DbContextOptionsBuilder().Options))
            {
                var store = new ChatStore<string, UserTest, FakeContextTest, Conversation, Attendee, Message, NotificationConnection>(context);
                var users = store.Users;
                Assert.NotNull(users);
                Assert.IsAssignableFrom<DbSet<UserTest>>(users);
            }
        }

        [Fact]
        public async Task CreateMessageAsync_should_throw_ArgumentNullException()
        {
            using (var context = new ChatDbContext(new DbContextOptionsBuilder().Options))
            {
                var store = new ChatStore<string, UserTest, ChatDbContext, Conversation, Attendee, Message, NotificationConnection>(context);
                await Assert.ThrowsAsync<ArgumentNullException>(() => store.CreateMessageAsync(null));
            }
        }

        static async Task ExecuteTest(Func<ChatStore<string, ChatLeUser, ChatLeIdentityDbContext<string, Message, Attendee, Conversation, NotificationConnection>, Conversation, Attendee, Message, NotificationConnection>, Task> action)
        {  
            var builder = new DbContextOptionsBuilder();
            builder.UseInMemoryDatabase();            
                      
            using (var context = new ChatLeIdentityDbContext<string, Message, Attendee, Conversation, NotificationConnection>(builder.Options))
            {
                var store = new ChatStore<string, ChatLeUser, ChatLeIdentityDbContext<string, Message, Attendee, Conversation, NotificationConnection>, Conversation, Attendee, Message, NotificationConnection>(context);
                await action(store);
            }
        }

        [Fact]
        public async Task CreateMessageAsyncTest()
        {
            await ExecuteTest(async (store) =>
            {
                var message = new Message()
                {
                    ConversationId = "test",
                    Date = DateTime.UtcNow,
                    Text = "test",
                    UserId = "test",
                };
                await store.CreateMessageAsync(message);
            });
        }

        [Fact]
        public async Task UpdateUserAsync_should_throw_ArgumentNullException()
        {
            await ExecuteTest(async (store) =>
            {
                await Assert.ThrowsAsync<ArgumentNullException>(() => store.UpdateUserAsync(null));
            });
        }

        [Fact]
        public async Task UpdateUserAsyncTest()
        {
            await ExecuteTest(async (store) =>
            {
                var user = new ChatLeUser()
                {
                    Id = "test",
                    UserName = "test"
                };
                store.Context.Add(user);
                await store.Context.SaveChangesAsync();

                await store.UpdateUserAsync(user);
            });
        }

        [Fact]
        public async Task GetUsersConnectedAsyncTest()
        {
            await ExecuteTest(async (store) =>
            {
                var context = store.Context;
                var connected = new ChatLeUser()
                {
                    Id = "connected",
                    UserName = "connected"
                };
                context.Add(connected);

                var nc = new NotificationConnection()
                {
                    ConnectionId = "test",
                    UserId = "connected",
                    NotificationType = "test"
                };
                context.NotificationConnections.Add(nc);

                var notConnected = new ChatLeUser()
                {
                    Id = "notConnected",
                    UserName = "notConnected",
                };
                context.Add(notConnected);

                context.SaveChanges();

                var users = await store.GetUsersConnectedAsync();
                Assert.True(users.Count() == 1);
                Assert.True(users.FirstOrDefault() == connected);
            });
        }

        [Fact]
        public async Task NotificationConnectionAsyncTest()
        {
            await ExecuteTest(async (store) =>
            {
                var context = store.Context;
                var user = new ChatLeUser()
                {
                    Id = "test",
                    UserName = "test",
                };
                context.Add(user);
                context.SaveChanges();

                var nc = new NotificationConnection()
                {
                    ConnectionId = "test",
                    UserId = "test",
                    NotificationType = "test"
                };

                await store.CreateNotificationConnectionAsync(nc);

                Assert.Equal(1, context.NotificationConnections.Count());

                nc = await store.GetNotificationConnectionAsync("test", "test");                
                await store.DeleteNotificationConnectionAsync(nc);

                Assert.Empty(context.NotificationConnections);

                var nc1 = new NotificationConnection()
                {
                    ConnectionId = "test",
                    UserId = "test",
                    NotificationType = "test"
                };

                await store.CreateNotificationConnectionAsync(nc1);
                var nc2 = new NotificationConnection()
                {
                    ConnectionId = "test2",
                    UserId = "test2",
                    NotificationType = "test"
                };

                await store.CreateNotificationConnectionAsync(nc2);

                Assert.Equal(2, context.NotificationConnections.Count());
            });

        }

        [Fact]
        public async Task CreateNotificationConnectionAsync_should_throw_ArgumentNullException()
        {
            await ExecuteTest(async (store) =>
            {
                await Assert.ThrowsAsync<ArgumentNullException>(() => store.CreateNotificationConnectionAsync(null));
            });
        }

        [Fact]
        public async Task GetConversationsAsyncTest()
        {
            await ExecuteTest(async (store) =>
            {
                var context = store.Context;
                var conv = new Conversation();
                context.Conversations.Add(conv);
                var attendee = new Attendee()
                {
                    ConversationId = conv.Id,
                    UserId = "test"
                };
                context.Add(attendee);

                var message = new Message()
                {
                    ConversationId = conv.Id,
                    UserId = "test",
                    Text = "test",
                    Date = DateTime.UtcNow
                };

                context.Messages.Add(message);
                context.SaveChanges();

                var convs = await store.GetConversationsAsync("test");
                Assert.NotNull(convs);
                Assert.True(convs.Count() == 1);
                Assert.NotNull(convs.FirstOrDefault());
                Assert.Equal(conv, convs.First());
            });
        }

        [Fact]
        public async Task CreateAttendeeAsync_should_throw_ArgumentNullException()
        {
            await ExecuteTest(async store =>
            {
                await Assert.ThrowsAsync<ArgumentNullException>(() => store.CreateAttendeeAsync(null));
            });
        }

        [Fact]
        public async Task CreateAttendeeAsyncTest()
        {
            await ExecuteTest(async store =>
            {
                await store.CreateAttendeeAsync(new Attendee() { ConversationId = "test", UserId = "test" });
            });
        }

        [Fact]
        public async Task InitTest()
        {
            await ExecuteTest(async store =>
            {
                await store.Context.Database.EnsureCreatedAsync();
                store.Init();
            });
        }

        [Fact]
        public async Task CreateConversationAsync_should_throw_ArgumentNullException()
        {
            await ExecuteTest(async store =>
            {
                await Assert.ThrowsAsync<ArgumentNullException>(() => store.CreateConversationAsync(null));
            });
        }

        [Fact]
        public async Task CreateConversationAsyncTest()
        {
            await ExecuteTest(async store =>
            {
                await store.CreateConversationAsync(new Conversation());
            });
        }

        [Fact]
        public async Task FindUserByNameTest()
        {
            await ExecuteTest(async store =>
            {
                await store.FindUserByNameAsync("test");
            });
        }

		[Fact]
		public async Task FindUserByIdTest()
		{
			await ExecuteTest(async store =>
			{
				await store.FindUserByIdAsync("test");
			});
		}

		[Fact]
        public async Task GetConversationAsync_should_throw_ArgumentNullException()
        {
            await ExecuteTest(async store =>
            {
                await Assert.ThrowsAsync<ArgumentNullException>(() => store.GetConversationAsync(null, null));
                await Assert.ThrowsAsync<ArgumentNullException>(() => store.GetConversationAsync(new ChatLeUser(), null));
            });
        }

        [Fact]
        public async Task GetConversationAsync_with_attendees()
        {
            await ExecuteTest(async store =>
            {
                AddConv(store);
                Assert.NotNull(await store.GetConversationAsync(new ChatLeUser() { Id = "test1" }, new ChatLeUser() { Id = "test2" }));
            });
        }

        private static Conversation AddConv(ChatStore<string, ChatLeUser, ChatLeIdentityDbContext<string, Message, Attendee, Conversation, NotificationConnection>, Conversation, Attendee, Message, NotificationConnection> store)
        {
            var context = store.Context;
            var conv = new Conversation();
            context.Add(conv);
            var attendee1 = new Attendee()
            {
                ConversationId = conv.Id,
                UserId = "test1"
            };
            context.Add(attendee1);
            var attendee2 = new Attendee()
            {
                ConversationId = conv.Id,
                UserId = "test2"
            };
            context.Add(attendee2);

            var message = new Message()
            {
                ConversationId = conv.Id,
                UserId = "test1",
                Text = "test",
                Date = DateTime.UtcNow
            };

            context.Messages.Add(message);
            context.SaveChanges();
            return conv;
        }

        [Fact]
        public async Task GetConversationAsync_with_convId()
        {
            await ExecuteTest(async store =>
            {
                var conv = AddConv(store);
                Assert.NotNull(await store.GetConversationAsync(conv.Id));
            });
        }

        [Fact]
        public async Task GetMessagesAsyncTest()
        {
            await ExecuteTest(async store =>
            {
                var conv = AddConv(store);
                var messages = await store.GetMessagesAsync(conv.Id);
                Assert.NotNull(messages);
                Assert.NotNull(messages.FirstOrDefault());
            });
        }

        [Fact]
        public async Task DeleteNotificationConnectionAsync_should_throw_ArgumentNullException()
        {
            await ExecuteTest(async store =>
            {
                await Assert.ThrowsAsync<ArgumentNullException>(() => store.DeleteNotificationConnectionAsync(null));
            });
        }

        [Theory]
        [InlineData("test", null)]
        [InlineData(null, "test")]
        public async Task GetNotificationConnectionAsync_should_throw_ArgumentNullException(string conversationId, string notificationId)
        {
            await ExecuteTest(async store =>
            {
                await Assert.ThrowsAsync<ArgumentNullException>(() => store.GetNotificationConnectionAsync(conversationId, notificationId));
            });
        }

        [Fact]
        public async Task GetNotificationConnectionsAsyncTest()
        {
            await ExecuteTest(async store =>
            {
                var context = store.Context;

                context.NotificationConnections.Add(new NotificationConnection()
                {
                    ConnectionDate = DateTime.Now,
                    ConnectionId = "test",
                    NotificationType = "test",
                    UserId = "test"
                });

                Assert.NotNull(await store.GetNotificationConnectionsAsync("test", "test"));
            });
        }

        [Fact]
        public async Task UserHasConnectionAsyncTest()
        {
            await ExecuteTest(async store =>
            {
                var context = store.Context;

                context.NotificationConnections.Add(new NotificationConnection()
                {
                    ConnectionDate = DateTime.Now,
                    ConnectionId = "test",
                    NotificationType = "test",
                    UserId = "test1"
                });

                Assert.False(await store.UserHasConnectionAsync("test"));
            });
        }

        [Fact]
        public async Task GetAttendeesAsyncTest()
        {
            await ExecuteTest(async store =>
            {
                var conv = AddConv(store);
                var attendees = await store.GetAttendeesAsync(conv);
                Assert.NotNull(attendees);
                Assert.NotNull(attendees.FirstOrDefault());
            });
        }

        [Fact]
        public async Task DeleteUserAsyncTest()
        {
            await ExecuteTest(async store =>
            {
                var conv = AddConv(store);
                var user = new ChatLeUser() { Id = "test" };

                var context = store.Context;

                context.Add(new Attendee()
                {
                    ConversationId = conv.Id,
                    UserId = "test"
                });

                context.Add(user);
                context.NotificationConnections.Add(new NotificationConnection()
                {
                    ConnectionDate = DateTime.Now,
                    ConnectionId = "test",
                    NotificationType = "test",
                    UserId = "test"
                });

                context.SaveChanges();

                await store.DeleteUserAsync(user);

                Assert.Empty(context.NotificationConnections);
                Assert.Empty(context.Users);

                Assert.NotEmpty(context.Conversations);
            });
        }
    }
}
