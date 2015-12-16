using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using ChatLe.Models;
using System;
using Microsoft.Extensions.Configuration;

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
    }
}
