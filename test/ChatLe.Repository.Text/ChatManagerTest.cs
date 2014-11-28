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
            var storeMock = new Mock<IChatStore<string, UserTest, Conversation, Attendee, Message>>();
            storeMock.Setup(s => s.FindUserByNameAsync("test")).ReturnsAsync(new UserTest());
            var manager = new ChatManager<string, UserTest, Conversation, Attendee, Message>(storeMock.Object);
            await manager.AddConnectionIdAsync("test", "test");
        }

        [Fact]
        public async Task AddConnectionIdAsyncUserNameNullTest()
        {
            var storeMock = new Mock<IChatStore<string, UserTest, Conversation, Attendee, Message>>();
            storeMock.Setup(s => s.FindUserByNameAsync("test")).ReturnsAsync(new UserTest());
            var manager = new ChatManager<string, UserTest, Conversation, Attendee, Message>(storeMock.Object);
            await Assert.ThrowsAsync<ArgumentNullException>(() => manager.AddConnectionIdAsync(null, "test"));
        }

        [Fact]
        public async Task AddConnectionIdAsyncConnectionIdNullTest()
        {
            var storeMock = new Mock<IChatStore<string, UserTest, Conversation, Attendee, Message>>();
            storeMock.Setup(s => s.FindUserByNameAsync("test")).ReturnsAsync(new UserTest());
            var manager = new ChatManager<string, UserTest, Conversation, Attendee, Message>(storeMock.Object);
            await Assert.ThrowsAsync<ArgumentNullException>(() => manager.AddConnectionIdAsync("test", null));
        }

        [Fact]
        public async Task AddConnectionIdAsyncUserNotExistTest()
        {
            var storeMock = new Mock<IChatStore<string, UserTest, Conversation, Attendee, Message>>();
            storeMock.Setup(s => s.FindUserByNameAsync("test")).ReturnsAsync(null);
            var manager = new ChatManager<string, UserTest, Conversation, Attendee, Message>(storeMock.Object);
            await manager.AddConnectionIdAsync("test", "test");
        }

        [Fact]
        public async Task GetConnectedUsersAsyncTest()
        {
            var storeMock = new Mock<IChatStore<string, UserTest, Conversation, Attendee, Message>>();
            var connected = new UserTest() { Id = "connected", IsConnected=true };
            storeMock.Setup(s => s.GetUsersConnectedAsync()).ReturnsAsync(new UserTest[] { connected });
            var manager = new ChatManager<string, UserTest, Conversation, Attendee, Message>(storeMock.Object);
            var users = await manager.GetUsersConnectedAsync();
            Assert.True(users.Count() == 1);
            Assert.True(users.First() == connected);
        }
    }
}