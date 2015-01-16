using ChatLe.Models;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using System.Linq;
using System.Threading;
using Microsoft.Framework.OptionsModel;

namespace ChatLe.Repository.Text
{
    public class ChatManagerTest
    {
        class OptionsAccessor : IOptions<ChatOptions>
        {
            public ChatOptions Options
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
        public async Task AddConnectionIdAsyncTest()
        {
            var storeMock = new Mock<IChatStore<string, UserTest, Conversation, Attendee, Message, NotificationConnection>>();
            storeMock.Setup(s => s.FindUserByNameAsync("test", default(CancellationToken))).ReturnsAsync(new UserTest());            
            var manager = new ChatManager<string, UserTest, Conversation, Attendee, Message, NotificationConnection>(storeMock.Object, new OptionsAccessor());
            await manager.AddConnectionIdAsync("test", "test", "test");
        }

        [Fact]
        public async Task AddConnectionIdAsyncUserNameNullTest()
        {
            var storeMock = new Mock<IChatStore<string, UserTest, Conversation, Attendee, Message, NotificationConnection>>();
            storeMock.Setup(s => s.FindUserByNameAsync("test", default(CancellationToken))).ReturnsAsync(new UserTest());
            var manager = new ChatManager<string, UserTest, Conversation, Attendee, Message, NotificationConnection>(storeMock.Object, new OptionsAccessor());
            await Assert.ThrowsAsync<ArgumentNullException>(() => manager.AddConnectionIdAsync(null, "test", "test"));
        }

        [Fact]
        public async Task AddConnectionIdAsyncConnectionIdNullTest()
        {
            var storeMock = new Mock<IChatStore<string, UserTest, Conversation, Attendee, Message, NotificationConnection>>();
            storeMock.Setup(s => s.FindUserByNameAsync("test", It.IsAny<CancellationToken>())).ReturnsAsync(new UserTest());
            var manager = new ChatManager<string, UserTest, Conversation, Attendee, Message, NotificationConnection>(storeMock.Object, new OptionsAccessor());
            await Assert.ThrowsAsync<ArgumentNullException>(() => manager.AddConnectionIdAsync("test", null, "test"));
        }

        [Fact]
        public async Task AddConnectionIdAsyncUserNotExistTest()
        {
            var storeMock = new Mock<IChatStore<string, UserTest, Conversation, Attendee, Message, NotificationConnection>>();
            storeMock.Setup(s => s.FindUserByNameAsync("test", default(CancellationToken))).ReturnsAsync(null);
            var manager = new ChatManager<string, UserTest, Conversation, Attendee, Message, NotificationConnection>(storeMock.Object, new OptionsAccessor());
            await manager.AddConnectionIdAsync("test", "test", "test");
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
            Assert.True(conv.Messages.Count > 0);
            Assert.True(conv.Messages.Last().Text == "test");
        }
    }
}