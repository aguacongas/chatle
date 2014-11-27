using ChatLe.Models;
using Microsoft.Data.Entity;
using Microsoft.Framework.DependencyInjection.Fallback;
using Moq;
using System;
using Xunit;

namespace ChatLe.Repository.Text
{
    public class ChatManagerTest
    {
        [Fact]
        public void Constructor()
        {
            var services = TestHelpers.GetServicesCollection();
            var manager1 = new ChatManager<string, UserTest>(new Mock<IChatStore<string, UserTest>>().Object);
            var manager2 = new ChatManager(new Mock<ChatStore>().Object);
        }
    }
}