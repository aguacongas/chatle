using ChatLe.Hubs;
using ChatLe.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Hubs;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using Xunit;

namespace chatle.test.Hubs
{
    public class ChatHubTest
    {
		public static void ExecuteAction(Action<ChatHub, Mock<IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>>, Mock<HttpContext>, Mock<UserManager<ChatLeUser>>> a)
		{
			var mockHttpRequest = new Mock<HttpRequest>();
			var mockHttpContext = new Mock<HttpContext>();
			mockHttpRequest.SetupGet(h => h.HttpContext).Returns(mockHttpContext.Object);
			var mockChatManager = new Mock<IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>>();
			var mockLoggerFactory = new Mock<ILoggerFactory>();
			mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>())).Returns(new Mock<ILogger>().Object);
			var mockUserManager = new Mock<UserManager<ChatLeUser>>();
			var hub = new ChatHub(mockChatManager.Object, mockUserManager.Object, mockLoggerFactory.Object);
			using (hub)
			{
				hub.Context = new HubCallerContext(mockHttpRequest.Object, "test");
				a.Invoke(hub, mockChatManager, mockHttpContext, mockUserManager);
			}
		}

		[Fact]
		public void OnConnectedTest()
		{
			ExecuteAction(async (hub, mockChatManager, mockHttpContext, mockUserManager) =>
			{
				var mockClaims = new Mock<ClaimsPrincipal>();
				var mockIndentity = new Mock<IIdentity>();
				mockClaims.SetupGet(c => c.Claims).Returns(new List<Claim>());
				mockIndentity.SetupGet(i => i.IsAuthenticated).Returns(true);				
				mockClaims.SetupGet(c => c.Identity).Returns(mockIndentity.Object);
				mockHttpContext.SetupGet(h => h.User).Returns(mockClaims.Object);

				var mockGroups = new Mock<IGroupManager>();
				hub.Groups = mockGroups.Object;
				
				var mockClients = new Mock<IHubCallerConnectionContext<dynamic>>();
				dynamic others = new ExpandoObject();
				others.userConnected = new Action<object>(o => { });
				mockClients.SetupGet(c => c.Others).Returns((ExpandoObject)others);
				hub.Clients = mockClients.Object;

				await hub.OnConnected();
			});
		}

		[Fact]
		public void OnGuessConnectedTest()
		{
			ExecuteAction(async (hub, mockChatManager, mockHttpContext, mockUserManager) =>
			{
				var mockClaims = new Mock<ClaimsPrincipal>();
				var mockIndentity = new Mock<IIdentity>();
				mockClaims.SetupGet(c => c.Claims).Returns(new List<Claim>{ new Claim("guess", "true")});
				mockIndentity.SetupGet(i => i.IsAuthenticated).Returns(true);				
				mockClaims.SetupGet(c => c.Identity).Returns(mockIndentity.Object);
				mockHttpContext.SetupGet(h => h.User).Returns(mockClaims.Object);
				mockUserManager.Setup(u => u.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new ChatLeUser());
				mockUserManager.Setup(u => u.CreateAsync(It.IsAny<ChatLeUser>())).ReturnsAsync(new IdentityResult());
				
				var mockGroups = new Mock<IGroupManager>();
				hub.Groups = mockGroups.Object;
				
				var mockClients = new Mock<IHubCallerConnectionContext<dynamic>>();
				dynamic others = new ExpandoObject();
				others.userConnected = new Action<object>(o => { });
				mockClients.SetupGet(c => c.Others).Returns((ExpandoObject)others);
				hub.Clients = mockClients.Object;

				await hub.OnConnected();
			});
		}

		[Fact]
		public void OnReconnectedTest()
		{
			ExecuteAction(async (hub, mockChatManager, mockHttpContext, mockUserManager) =>
			{
				var mockClaims = new Mock<ClaimsPrincipal>();
				var mockIndentity = new Mock<IIdentity>();
				mockIndentity.SetupGet(i => i.IsAuthenticated).Returns(true);
				mockClaims.SetupGet(c => c.Identity).Returns(mockIndentity.Object);
				mockHttpContext.SetupGet(h => h.User).Returns(mockClaims.Object);

				var mockGroups = new Mock<IGroupManager>();
				hub.Groups = mockGroups.Object;

				var mockClients = new Mock<IHubCallerConnectionContext<dynamic>>();
				dynamic others = new ExpandoObject();
				others.userConnected = new Action<object>(o => { });
				mockClients.SetupGet(c => c.Others).Returns((ExpandoObject)others);
				hub.Clients = mockClients.Object;

				await hub.OnReconnected();
			});
		}

		[Fact]
		public void OnDisconnectedTest()
		{
			ExecuteAction(async (hub, mockChatManager, mockHttpContext, mockUserManager) =>
			{
				mockChatManager.Setup(c => c.RemoveConnectionIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ChatLeUser());
				var mockClients = new Mock<IHubCallerConnectionContext<dynamic>>();
				dynamic others = new ExpandoObject();
				others.userDisconnected = new Action<object>(o => { });
				mockClients.SetupGet(c => c.Others).Returns((ExpandoObject)others);
				hub.Clients = mockClients.Object;

				await hub.OnDisconnected(false);
			});
		}
	}
}
