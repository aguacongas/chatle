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
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Chatle.test.Controllers;
using System.Threading.Tasks;

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

		public static void ExecuteAction(Action<ChatHub, Mock<IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>>, Mock<HttpContext>> a)
		{
			var mockHttpRequest = new Mock<HttpRequest>();
			var mockHttpContext = new Mock<HttpContext>();
			mockHttpRequest.SetupGet(h => h.HttpContext).Returns(mockHttpContext.Object);
			var mockChatManager = new Mock<IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>>();
			var mockLoggerFactory = new Mock<ILoggerFactory>();
			mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>())).Returns(new Mock<ILogger>().Object);
			
			var userValidators = new List<IUserValidator<ChatLeUser>>();
			var validator = new Mock<IUserValidator<ChatLeUser>>();
			userValidators.Add(validator.Object);

			var hub = new ChatHub(mockChatManager.Object, mockLoggerFactory.Object);
			using (hub)
			{
				hub.Context = new HubCallerContext(mockHttpRequest.Object, "test");
				a.Invoke(hub, mockChatManager, mockHttpContext);
			}
		}

		[Fact]
		public void OnConnectedTest()
		{
			ExecuteAction(async (hub, mockChatManager, mockHttpContext) =>
			{
				var mockClaims = new Mock<ClaimsPrincipal>();
				var identity = new ClaimsIdentity("test");

				mockClaims.SetupGet(c => c.Claims).Returns(identity.Claims);				
				mockClaims.SetupGet(c => c.Identity).Returns(identity);
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
			ExecuteAction(async (hub, mockChatManager, mockHttpContext) =>
			{
				var mockClaims = new Mock<ClaimsPrincipal>();
				var identity = new ClaimsIdentity("test");
				identity.AddClaim(new Claim("guess", "true"));

				mockClaims.SetupGet(c => c.Claims).Returns(identity.Claims);				
				mockClaims.SetupGet(c => c.Identity).Returns(identity);
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
		public void OnReconnectedTest()
		{
			ExecuteAction(async (hub, mockChatManager, mockHttpContext) =>
			{
				var mockClaims = new Mock<ClaimsPrincipal>();
				var identity = new ClaimsIdentity("test");
				
				mockClaims.SetupGet(c => c.Claims).Returns(identity.Claims);				
				mockClaims.SetupGet(c => c.Identity).Returns(identity);
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
			ExecuteAction(async (hub, mockChatManager, mockHttpContext) =>
			{
				mockChatManager.Setup(c => c.RemoveConnectionIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ChatLeUser());
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
