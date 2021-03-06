﻿using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using ChatLe.Models;
using System;
using Microsoft.AspNetCore.Builder;

namespace ChatLe.Repository.Test
{
    public class BuilderExtensionTest
    {
        [Fact]
        public void ConfigureChatLeTest()
        {
            var services = new ServiceCollection();
            services.ConfigureChatLe((options) => { });
        }

        [Fact]
        public void AddChatLe_with_configure_action()
        {
            var services = new ServiceCollection();
            services.AddChatLe(configure: (options) => { });
        }

        class FakeServiceProvider : IServiceProvider
        {
            public object GetService(Type serviceType)
            {
                if (serviceType == typeof(IServiceScopeFactory))
                {
                    var mock = new Mock<IServiceScopeFactory>();
                    var scopeMock = new Mock<IServiceScope>();
                    scopeMock.Setup(m => m.ServiceProvider).Returns(this);
                    mock.Setup(m => m.CreateScope()).Returns(scopeMock.Object);
                    return mock.Object;
                }
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
