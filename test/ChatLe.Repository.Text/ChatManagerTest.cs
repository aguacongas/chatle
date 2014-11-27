using ChatLe.Models;
using Microsoft.Data.Entity;
using Microsoft.Framework.DependencyInjection.Fallback;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ChatLe.Repository.Text
{
    public class ChatManagerTest
    {
        [Fact]
        public void Constructor()
        {
            var manager1 = new ChatManager<string, UserTest>(new Mock<IChatStore<string, UserTest>>().Object);
            var manager2 = new ChatManager(new Mock<ChatStore>().Object);
        }

        [Fact]
        public async Task AddConnectionIdAsyncTest()
        {
            var storeMock = new Mock<IChatStore<string, UserTest>>();
            storeMock.Setup(s => s.FindUserByNameAsync("test")).ReturnsAsync(new UserTest());
            var manager = new ChatManager<string, UserTest>(storeMock.Object);
            await manager.AddConnectionIdAsync("test", "test");
        }
        [Fact]
        public async Task AddConnectionIdAsyncUserNameNullTest()
        {
            var storeMock = new Mock<IChatStore<string, UserTest>>();
            storeMock.Setup(s => s.FindUserByNameAsync("test")).ReturnsAsync(new UserTest());
            var manager = new ChatManager<string, UserTest>(storeMock.Object);
            await Assert.ThrowsAsync<ArgumentNullException>(() => manager.AddConnectionIdAsync(null, "test"));
        }
        public async Task AddConnectionIdAsyncConnectionIdNullTest()
        {
            var storeMock = new Mock<IChatStore<string, UserTest>>();
            storeMock.Setup(s => s.FindUserByNameAsync("test")).ReturnsAsync(new UserTest());
            var manager = new ChatManager<string, UserTest>(storeMock.Object);
            await Assert.ThrowsAsync<ArgumentNullException>(() => manager.AddConnectionIdAsync("test", null));
        }
    }
}