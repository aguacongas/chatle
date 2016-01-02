using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using ChatLe.Models;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNet.Builder;

namespace ChatLe.Repository.Test
{
    public class BuilderExtensionTest
    {
        [Fact]
        public void ConfigureChatLeTest()
        {
            var serviceCollectionMock = new Mock<IServiceCollection>();
            var services = serviceCollectionMock.Object;
            services.ConfigureChatLe((options) => { });
        }

        [Fact]
        public void AddChatLe_with_IConfiguration()
        {
            var serviceCollectionMock = new Mock<IServiceCollection>();
            var services = serviceCollectionMock.Object;
            services.AddChatLe(config: new Mock<IConfiguration>().Object);
        }

        [Fact]
        public void AddChatLe_with_configure_action()
        {
            var serviceCollectionMock = new Mock<IServiceCollection>();
            var services = serviceCollectionMock.Object;
            services.AddChatLe(configure: (options) => { });
        }

        class FakeServiceProvider : IServiceProvider
        {
            public object GetService(Type serviceType)
            {
                return new Mock<IChatStore<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>>().Object;
            }
        }

        [Fact]
        public void UseChatLeTest()
        {
            var mockApplicationBuilder = new Mock<IApplicationBuilder>();
            mockApplicationBuilder.SetupGet(a => a.ApplicationServices).Returns(new FakeServiceProvider());
            mockApplicationBuilder.Object.UseChatLe();
        }
    }
}
