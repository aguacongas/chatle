using ChatLe.Hubs;
using ChatLe.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Security.Claims;
using System.Threading;
using Xunit;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Chatle.test.Controllers;
using System.Threading.Tasks.Channels;
using Microsoft.AspNetCore.Sockets;

namespace chatle.test.Hubs
{
    public class ChatHubTest
    {
		private static Mock<UserManager<TUser>> MockUserManager<TUser>(List<IUserValidator<TUser>> userValidators) where TUser : class
		{
			var store = new Mock<ITestUserStore<TUser>>();
			var options = new Mock<IOptions<IdentityOptions>>();
			var idOptions = new IdentityOptions();
			idOptions.Lockout.AllowedForNewUsers = false;
			options.Setup(o => o.Value).Returns(idOptions);
			var pwdValidators = new List<PasswordValidator<TUser>>();
			pwdValidators.Add(new PasswordValidator<TUser>());

			var services = new ServiceCollection();
			services.AddEntityFrameworkInMemoryDatabase();

			var userManager = new Mock<UserManager<TUser>>(store.Object, options.Object, new PasswordHasher<TUser>(),
				userValidators, pwdValidators, new UpperInvariantLookupNormalizer(),
				new IdentityErrorDescriber(), services.BuildServiceProvider(),
				new Mock<ILogger<UserManager<TUser>>>().Object);

			return userManager;
		}

        internal static void ExecuteAction(Action<ChatHub, Mock<IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>>, Mock<HubConnectionContext>> a)
		{
			var mockChatManager = new Mock<IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>>();
			var mockLoggerFactory = new Mock<ILoggerFactory>();
			mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>())).Returns(new Mock<ILogger>().Object);
			
			var userValidators = new List<IUserValidator<ChatLeUser>>();
			var validator = new Mock<IUserValidator<ChatLeUser>>();
			userValidators.Add(validator.Object);

			var provideMock = new Mock<IServiceProvider>();
			provideMock.Setup(p => p.GetService(It.IsAny<Type>()))
				.Returns(mockChatManager.Object);
            var mockWritableChannel = new Mock<WritableChannel<byte[]>>();
            var mockConnectionContext = new Mock<ConnectionContext>();
            var hubConnectionContextMock = new Mock<HubConnectionContext>(mockWritableChannel.Object, mockConnectionContext.Object);
            var hub = new ChatHub(provideMock.Object, mockLoggerFactory.Object);

            using (hub)
			{
				hub.Context = new HubCallerContext(hubConnectionContextMock.Object);
				a.Invoke(hub, mockChatManager, hubConnectionContextMock);
			}
		}

		[Fact]
		public void OnConnectedTest()
		{
			ExecuteAction(async (hub, mockChatManager, mockHubConnectionContext) =>
			{
				var mockClaims = new Mock<ClaimsPrincipal>();
				var identity = new ClaimsIdentity("test");

				mockClaims.SetupGet(c => c.Claims).Returns(identity.Claims);				
				mockClaims.SetupGet(c => c.Identity).Returns(identity);
				mockHubConnectionContext.SetupGet(h => h.User).Returns(mockClaims.Object);

				var mockGroups = new Mock<IGroupManager>();
				hub.Groups = mockGroups.Object;

                var mockClients = new Mock<IHubClients>();
                var clientProxyMock = new Mock<IClientProxy>();
                mockClients.SetupGet(c => c.All).Returns(clientProxyMock.Object);
                hub.Clients = mockClients.Object;

                await hub.OnConnectedAsync();
            });
		}

		[Fact]
		public void OnGuessConnectedTest()
		{
			ExecuteAction(async (hub, mockChatManager, mockHubConnectionContext) =>
			{
				var mockClaims = new Mock<ClaimsPrincipal>();
				var identity = new ClaimsIdentity("test");
				identity.AddClaim(new Claim("guess", "true"));

				mockClaims.SetupGet(c => c.Claims).Returns(identity.Claims);				
				mockClaims.SetupGet(c => c.Identity).Returns(identity);
				mockHubConnectionContext.SetupGet(h => h.User).Returns(mockClaims.Object);

				var mockGroups = new Mock<IGroupManager>();
				hub.Groups = mockGroups.Object;
				
				var mockClients = new Mock<IHubClients>();
                var clientProxyMock = new Mock<IClientProxy>();
                mockClients.SetupGet(c => c.All).Returns(clientProxyMock.Object);
                hub.Clients = mockClients.Object;

                await hub.OnConnectedAsync();
			});
		}
		[Fact]
		public void OnDisconnectedTest()
		{
			ExecuteAction(async (hub, mockChatManager, mockHubConnectionContext) =>
			{
				mockChatManager.Setup(c => c.RemoveConnectionIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ChatLeUser());
                var mockClients = new Mock<IHubClients>();
                var clientProxyMock = new Mock<IClientProxy>();
                mockClients.SetupGet(c => c.All).Returns(clientProxyMock.Object);
                hub.Clients = mockClients.Object;

                await hub.OnDisconnectedAsync(null);
			});
		}
	}
}
