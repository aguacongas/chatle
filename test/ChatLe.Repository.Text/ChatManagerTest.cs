using ChatLe.Models;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using System.Linq;

namespace ChatLe.Repository.Text
{
    public class ChatManagerTest
    {
        [Fact]
        public async Task AddConnectionIdAsyncTest()
        {
            var storeMock = new Mock<IChatStore<string, UserTest, Conversation, Attendee, Message, NotificationConnection>>();
            storeMock.Setup(s => s.FindUserByNameAsync("test")).ReturnsAsync(new UserTest());
            var manager = new ChatManager<string, UserTest, Conversation, Attendee, Message, NotificationConnection>(storeMock.Object);
            await manager.AddConnectionIdAsync("test", "test", "test");
        }

        [Fact]
        public async Task AddConnectionIdAsyncUserNameNullTest()
        {
            var storeMock = new Mock<IChatStore<string, UserTest, Conversation, Attendee, Message, NotificationConnection>>();
            storeMock.Setup(s => s.FindUserByNameAsync("test")).ReturnsAsync(new UserTest());
            var manager = new ChatManager<string, UserTest, Conversation, Attendee, Message, NotificationConnection>(storeMock.Object);
            await Assert.ThrowsAsync<ArgumentNullException>(() => manager.AddConnectionIdAsync(null, "test", "test"));
        }

        [Fact]
        public async Task AddConnectionIdAsyncConnectionIdNullTest()
        {
            var storeMock = new Mock<IChatStore<string, UserTest, Conversation, Attendee, Message, NotificationConnection>>();
            storeMock.Setup(s => s.FindUserByNameAsync("test")).ReturnsAsync(new UserTest());
            var manager = new ChatManager<string, UserTest, Conversation, Attendee, Message, NotificationConnection>(storeMock.Object);
            await Assert.ThrowsAsync<ArgumentNullException>(() => manager.AddConnectionIdAsync("test", null, "test"));
        }

        [Fact]
        public async Task AddConnectionIdAsyncUserNotExistTest()
        {
            var storeMock = new Mock<IChatStore<string, UserTest, Conversation, Attendee, Message, NotificationConnection>>();
            storeMock.Setup(s => s.FindUserByNameAsync("test")).ReturnsAsync(null);
            var manager = new ChatManager<string, UserTest, Conversation, Attendee, Message, NotificationConnection>(storeMock.Object);
            await manager.AddConnectionIdAsync("test", "test", "test");
        }

        [Fact]
        public async Task GetConnectedUsersAsyncTest()
        {
            var storeMock = new Mock<IChatStore<string, UserTest, Conversation, Attendee, Message, NotificationConnection>>();
            var connected = new UserTest() { Id = "connected" };
            connected.NotificationConnections.Add(new NotificationConnection<string>() { UserId = "test", ConnectionId = "test", NotificationType = "test" });
            storeMock.Setup(s => s.GetUsersConnectedAsync()).ReturnsAsync(new UserTest[] { connected });
            var manager = new ChatManager<string, UserTest, Conversation, Attendee, Message, NotificationConnection>(storeMock.Object);
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
            storeMock.Setup(s => s.FindUserByNameAsync(userId1)).ReturnsAsync(user1);
            storeMock.Setup(s => s.FindUserByNameAsync(userId2)).ReturnsAsync(user2);
            var manager = new ChatManager<string, UserTest, Conversation, Attendee, Message, NotificationConnection>(storeMock.Object);
            var conv = await manager.GetOrCreateConversationAsync(userId1, userId2, "test");
            Assert.True(conv.Attendees.Count == 2);
            Assert.True(conv.Messages.Count > 0);
            Assert.True(conv.Messages.Last().Text == "test");
        }
    }
}