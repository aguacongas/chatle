using ChatLe.Models;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace ChatLe.Repository.Test
{
    public class ChatManagerTest
    {
        class OptionsAccessor : IOptions<ChatOptions>
        {
            public ChatOptions Value
            {
                get
                {
                    return new ChatOptions();
                }
            }

            public ChatOptions GetNamedOptions(string name)
            {
                return new ChatOptions();
            }
        }

        [Fact]
        public void Constructor_should_throw_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ChatManager<string, UserTest, Conversation, Attendee, Message, NotificationConnection>(null, new OptionsAccessor()));
            var storeMock = new Mock<IChatStore<string, UserTest, Conversation, Attendee, Message, NotificationConnection>>();
            Assert.Throws<ArgumentNullException>(() => new ChatManager<string, UserTest, Conversation, Attendee, Message, NotificationConnection>(storeMock.Object, null));
        }

        [Fact]
        public async Task AddConnectionIdAsyncTest()
        {
            var storeMock = new Mock<IChatStore<string, UserTest, Conversation, Attendee, Message, NotificationConnection>>();            
            var manager = new ChatManager<string, UserTest, Conversation, Attendee, Message, NotificationConnection>(storeMock.Object, new OptionsAccessor());
            storeMock.Setup(s => s.FindUserByNameAsync("test", default(CancellationToken))).ReturnsAsync(new UserTest());
            await manager.AddConnectionIdAsync("test", "test", "test");
            storeMock.Setup(s => s.GetNotificationConnectionAsync("test", "test", CancellationToken.None)).ReturnsAsync(new NotificationConnection());
            await manager.AddConnectionIdAsync("test", "test", "test");
        }

        [Theory]
        [InlineData(null, "test", "test")]
        [InlineData("test", null, "test")]
        [InlineData("test", "test", null)]
        public async Task AddConnectionIdAsync_should_throw_ArgumentNullException(string userName, string connectionId, string notificationType)
        {
            var manager = CreateManager();
            await Assert.ThrowsAsync<ArgumentNullException>(() => manager.AddConnectionIdAsync(userName, connectionId, notificationType));
        }

        [Fact]
        public async Task AddConnectionIdAsyncUserNotExistTest()
        {
            var storeMock = new Mock<IChatStore<string, UserTest, Conversation, Attendee, Message, NotificationConnection>>();
            var manager = new ChatManager<string, UserTest, Conversation, Attendee, Message, NotificationConnection>(storeMock.Object, new OptionsAccessor());
            await Assert.ThrowsAsync<InvalidOperationException>(() => manager.AddConnectionIdAsync("test", "test", "test"));            
        }

        [Fact]
        public async Task GetConnectedUsersAsyncTest()
        {
            var storeMock = new Mock<IChatStore<string, UserTest, Conversation, Attendee, Message, NotificationConnection>>();
            var connected = new UserTest() { Id = "connected" };
            connected.NotificationConnections.Add(new NotificationConnection<string>() { UserId = "test", ConnectionId = "test", NotificationType = "test" });
            storeMock.Setup(s => s.GetUsersConnectedAsync(0, 50, default(CancellationToken))).ReturnsAsync(new Page<UserTest>(new UserTest[] { connected }, 0, 1));
            var manager = new ChatManager<string, UserTest, Conversation, Attendee, Message, NotificationConnection>(storeMock.Object, new OptionsAccessor());
            var users = await manager.GetUsersConnectedAsync();
            Assert.True(users.Count() == 1);
            Assert.True(users.First() == connected);
        }

        [Theory]
        [InlineData (null, "test")]
        [InlineData("test", null)]
        public async Task RemoveConnectionIdAsync_Should_ThrowArgumentNullException(string connectionId, string notificationType)
        {
            var manager = CreateManager();
            await Assert.ThrowsAsync<ArgumentNullException>(() => manager.RemoveConnectionIdAsync(connectionId, notificationType, false));
        }

        [Fact]
        public async Task RemoveConnectionIdAsyncTest()
        {
            var storeMock = new Mock<IChatStore<string, UserTest, Conversation, Attendee, Message, NotificationConnection>>();
            var manager = new ChatManager<string, UserTest, Conversation, Attendee, Message, NotificationConnection>(storeMock.Object, new OptionsAccessor());
			Assert.Null(await manager.RemoveConnectionIdAsync("test", "test", false));
            storeMock.Setup(s => s.GetNotificationConnectionAsync("test", "test", CancellationToken.None)).ReturnsAsync(new NotificationConnection() { UserId = "test" });
			Assert.Null(await manager.RemoveConnectionIdAsync("test", "test", false));
            storeMock.Setup(s => s.FindUserByIdAsync("test", CancellationToken.None)).ReturnsAsync(new UserTest());
			storeMock.Setup(s => s.UserHasConnectionAsync("test")).ReturnsAsync(true);
			Assert.NotNull(await manager.RemoveConnectionIdAsync("test", "test", true));
			storeMock.Setup(s => s.UserHasConnectionAsync("test")).ReturnsAsync(false);
			Assert.NotNull(await manager.RemoveConnectionIdAsync("test", "test", true));
        }

        [Theory]
        [InlineData(null, "test")]
        [InlineData("test", null)]
        public async Task GetOrCreateConversationAsync_should_throw_ArgumentNullException(string from, string to)
        {
            var manager = CreateManager();
            await Assert.ThrowsAsync<ArgumentNullException>(() => manager.GetOrCreateConversationAsync(from, to, "test"));
        }

        [Theory,
            InlineData("1","2")
            ,InlineData("1", "2")]
        public async Task GetOrCreateConversationAsyncTest(string userId1, string userId2)
        {
            var storeMock = new Mock<IChatStore<string, UserTest, Conversation, Attendee, Message, NotificationConnection>>();
            var user1 = new UserTest() { Id = userId1, UserName = userId1 };
            var user2 = new UserTest() { Id = userId2, UserName = userId2 };
            storeMock.Setup(s => s.FindUserByNameAsync(userId1, default(CancellationToken))).ReturnsAsync(user1);
            storeMock.Setup(s => s.FindUserByNameAsync(userId2, default(CancellationToken))).ReturnsAsync(user2);
            var manager = new ChatManager<string, UserTest, Conversation, Attendee, Message, NotificationConnection>(storeMock.Object, new OptionsAccessor());
            var conv = await manager.GetOrCreateConversationAsync(userId1, userId2, "test");
            Assert.True(conv.Attendees.Count == 2);
            Assert.True(conv.Messages.Count == 1);
            Assert.True(conv.Messages.Last().Text == "test");
        }

        [Theory]
        [InlineData(null, "test")]
        [InlineData("test", null)]
        public async Task AddMessageAsync_should_throw_ArgumentNullException(string fromName, string conversationId)
        {
            var storeMock = new Mock<IChatStore<string, UserTest, Conversation, Attendee, Message, NotificationConnection>>();
            var manager = new ChatManager<string, UserTest, Conversation, Attendee, Message, NotificationConnection>(storeMock.Object, new OptionsAccessor());
            await Assert.ThrowsAsync<ArgumentNullException>(() => manager.AddMessageAsync(fromName, conversationId, null));
        }

        [Fact]
        public async Task AddMessageAsyncTest()
        {
            var storeMock = new Mock<IChatStore<string, UserTest, Conversation, Attendee, Message, NotificationConnection>>();
            var manager = new ChatManager<string, UserTest, Conversation, Attendee, Message, NotificationConnection>(storeMock.Object, new OptionsAccessor());
            await manager.AddMessageAsync("test", "test", new Message() { Text = "Test" });
            storeMock.Setup(s => s.FindUserByNameAsync("test", default(CancellationToken))).ReturnsAsync(new UserTest());
            Assert.Null(await manager.AddMessageAsync("test", "test", new Message() { Text = "Test" }));
            storeMock.Setup(s => s.GetConversationAsync("test", default(CancellationToken))).ReturnsAsync(new Conversation());
            Assert.NotNull(await manager.AddMessageAsync("test", "test", new Message() { Text = "Test" }));
        }

        [Fact]
        public async Task GetMessagesTest()
        {
            var storeMock = new Mock<IChatStore<string, UserTest, Conversation, Attendee, Message, NotificationConnection>>();
            var manager = new ChatManager<string, UserTest, Conversation, Attendee, Message, NotificationConnection>(storeMock.Object, new OptionsAccessor());
            storeMock.Setup(s => s.GetMessagesAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Message>() { new Message() });
            var list = await manager.GetMessagesAsync("test");
            Assert.NotEmpty(list);            
        }

        [Fact]
        public async Task GetConversationsAsync_should_throw_ArgumentNullException()
        {
            var manager = CreateManager();
            await Assert.ThrowsAsync<ArgumentNullException>(() => manager.GetConversationsAsync(null));
        }

        private static ChatManager<string, UserTest, Conversation, Attendee, Message, NotificationConnection> CreateManager()
        {
            var storeMock = new Mock<IChatStore<string, UserTest, Conversation, Attendee, Message, NotificationConnection>>();
            return new ChatManager<string, UserTest, Conversation, Attendee, Message, NotificationConnection>(storeMock.Object, new OptionsAccessor());
        }

        [Fact]
        public async Task GetConversationsAsyncTest()
        {
            var storeMock = new Mock<IChatStore<string, UserTest, Conversation, Attendee, Message, NotificationConnection>>();
            var manager = new ChatManager<string, UserTest, Conversation, Attendee, Message, NotificationConnection>(storeMock.Object, new OptionsAccessor());
            storeMock.Setup(s => s.FindUserByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new UserTest() { Id = "test" });
            storeMock.Setup(s => s.GetConversationsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Conversation>()
            {
                new Conversation()
                {
                    Id = "test",
                    Attendees = new List<Attendee<string>>{ new Attendee() }
                }
            });
            storeMock.Setup(s => s.GetMessagesAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Message>()
            {
                new Message()
                {
                    ConversationId = "test",
                    Id = "test",
                    Date = DateTime.Now,
                    Text = "test",
                    UserId = "test"
                },
                new Message()
                {
                    ConversationId = "test",
                    Id = "test",
                    Date = DateTime.Now,
                    Text = "test",
                    UserId = "test"
                },
            });

            var conversations = await manager.GetConversationsAsync("test");
            Assert.NotEmpty(conversations);
            Assert.NotEmpty(conversations.First().Attendees);
            Assert.NotEmpty(conversations.First().Messages);
        }

        [Fact]
        public async Task RemoveUserAsync_should_throw_ArgumentNullException()
        {
            var manager = CreateManager();
            await Assert.ThrowsAsync<ArgumentNullException>(() => manager.RemoveUserAsync(null, CancellationToken.None));
        }

        [Fact]
        public async Task RemoveUserAsyncTest()
        {
            var manager = CreateManager();
            await manager.RemoveUserAsync(new UserTest());
        }

        [Fact]
        public void IdentityConstructorCoverage()
        {
            var storeMock = new Mock<IChatStore<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>>();
            var manager = new ChatManager(storeMock.Object, new OptionsAccessor());
        }

    }
}
