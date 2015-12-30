using Chatle.test.Controllers;
using ChatLe.Controllers;
using ChatLe.Hubs;
using ChatLe.ViewModels;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.ViewFeatures;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using Moq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace chatle.test.Controllers
{
	public class ChatControllerTest
	{
		public static void ExecuteAction(Action<ChatController
			, Mock<IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>>
			, Mock<IHubContext>
			, Mock<UserManager<ChatLeUser>>> a)
		{
			var mockChatManager = new Mock<IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>>();
			var mockConnectioManager = new Mock<IConnectionManager>();
			var mockHubContext = new Mock<IHubContext>();
			mockConnectioManager.Setup(c => c.GetHubContext<ChatHub>()).Returns(mockHubContext.Object);
			var userValidators = new List<IUserValidator<ChatLeUser>>();
			var validator = new Mock<IUserValidator<ChatLeUser>>();
			userValidators.Add(validator.Object);
			var mockUserManager = MockUserManager(userValidators);
			var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
			var controller = new ChatController(mockChatManager.Object, mockConnectioManager.Object, mockUserManager.Object) { ViewData = viewData };
			using (controller)
			{
				a.Invoke(controller, mockChatManager, mockHubContext, mockUserManager);
			}
		}

		private static Mock<UserManager<TUser>> MockUserManager<TUser>(List<IUserValidator<TUser>> userValidators) where TUser : class
		{
			var store = new Mock<ITestUserStore<TUser>>();
			var options = new Mock<IOptions<IdentityOptions>>();
			var idOptions = new IdentityOptions();
			idOptions.Lockout.AllowedForNewUsers = false;
			options.Setup(o => o.Value).Returns(idOptions);
			var pwdValidators = new List<PasswordValidator<TUser>>();
			pwdValidators.Add(new PasswordValidator<TUser>());
			var userManager = new Mock<UserManager<TUser>>(store.Object, options.Object, new PasswordHasher<TUser>(),
				userValidators, pwdValidators, new UpperInvariantLookupNormalizer(),
				new IdentityErrorDescriber(), null,
				new Mock<ILogger<UserManager<TUser>>>().Object,
				null);

			return userManager;
		}


		[Fact]
		public void GetConversationTest()
		{
			ExecuteAction(async (controller, mockChatManager, mockHubContext, mockUserManager) =>
			{
				mockChatManager.Setup(c => c.GetMessagesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
					.ReturnsAsync(new List<Message>()
						{
							new Message(),
							new Message()
						});

				mockUserManager.Setup(u => u.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new ChatLeUser());
				var result = await controller.Get("test");
				Assert.NotNull(result);
				Assert.NotEmpty(result);
				Assert.Equal(2, result.Count());
				var message = result.First();
				Assert.Null(message.From);
				Assert.Null(message.Text);
				Assert.Equal(DateTime.MinValue, message.Date);
			});
		}

		[Fact]
		public void GetConversationTest_WithNoUser()
		{
			ExecuteAction(async (controller, mockChatManager, mockHubContext, mockUserManager) =>
			{
				mockChatManager.Setup(c => c.GetMessagesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
					.ReturnsAsync(new List<Message>()
						{
							new Message(),
							new Message()
						});

				mockUserManager.Setup(u => u.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(null);
				var result = await controller.Get("test");
				Assert.NotNull(result);
				Assert.Empty(result);
			});
		}

		[Fact]
		public void GetConversationsTest()
		{
			ExecuteAction(async (controller, mockChatManager, mockHubContext, mockUserManager) =>
			{
				var mockHttpContext = new Mock<HttpContext>();
				var mockClaims = new Mock<ClaimsPrincipal>();
				var mockIndentity = new Mock<IIdentity>();
				mockIndentity.SetupGet(i => i.Name).Returns("test");
				mockClaims.SetupGet(c => c.Identity).Returns(mockIndentity.Object);
				mockHttpContext.SetupGet(h => h.User).Returns(mockClaims.Object);
				controller.ActionContext.HttpContext = mockHttpContext.Object;

				mockChatManager.Setup(c => c.GetConversationsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
					.ReturnsAsync(new List<Conversation>()
						{
							new Conversation()
							{
								Messages = new List<Message<string>>()
								{
									new Message() { UserId = "test" }
								},
								Attendees = new List<Attendee<string>>()
								{
									new Attendee() { UserId = "test1" },
									new Attendee() { UserId = "test" }
								}
							}
						});

				mockUserManager.Setup(u => u.FindByIdAsync("test")).ReturnsAsync(new ChatLeUser("test"));
				mockUserManager.Setup(u => u.FindByIdAsync("test1")).ReturnsAsync(new ChatLeUser("test1"));
				var result = await controller.Get();
				Assert.NotNull(result);
				Assert.NotEmpty(result);
				Assert.Equal(1, result.Count());
				var conversation = result.First();
				Assert.NotNull(conversation.Messages);
				Assert.NotEmpty(conversation.Messages);
			});
		}

		[Fact]
		public void GetConversationsTest_WithNoUser()
		{
			ExecuteAction(async (controller, mockChatManager, mockHubContext, mockUserManager) =>
			{
				var mockHttpContext = new Mock<HttpContext>();
				var mockClaims = new Mock<ClaimsPrincipal>();
				var mockIndentity = new Mock<IIdentity>();
				mockIndentity.SetupGet(i => i.Name).Returns("test");
				mockClaims.SetupGet(c => c.Identity).Returns(mockIndentity.Object);
				mockHttpContext.SetupGet(h => h.User).Returns(mockClaims.Object);
				controller.ActionContext.HttpContext = mockHttpContext.Object;

				mockChatManager.Setup(c => c.GetConversationsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
					.ReturnsAsync(new List<Conversation>()
						{
							new Conversation()
							{
								Messages = new List<Message<string>>()
								{
									new Message()
								},
								Attendees = new List<Attendee<string>>()
								{
									new Attendee() { UserId = "test1" },
									new Attendee() { UserId = "test2" }
								}
							},
						});

				mockUserManager.Setup(u => u.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(null);
				var result = await controller.Get();
				Assert.NotNull(result);
				Assert.Empty(result);
			});
		}

		[Fact]
		public void GetConversationsTest_WithDisconnectedUser()
		{
			ExecuteAction(async (controller, mockChatManager, mockHubContext, mockUserManager) =>
			{
				var mockHttpContext = new Mock<HttpContext>();
				var mockClaims = new Mock<ClaimsPrincipal>();
				var mockIndentity = new Mock<IIdentity>();
				mockIndentity.SetupGet(i => i.Name).Returns("test");
				mockClaims.SetupGet(c => c.Identity).Returns(mockIndentity.Object);
				mockHttpContext.SetupGet(h => h.User).Returns(mockClaims.Object);
				controller.ActionContext.HttpContext = mockHttpContext.Object;

				mockChatManager.Setup(c => c.GetConversationsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
					.ReturnsAsync(new List<Conversation>()
						{
							new Conversation()
							{
								Messages = new List<Message<string>>()
								{
									new Message() { UserId = "test2" }
								},
								Attendees = new List<Attendee<string>>()
								{
									new Attendee() { UserId = "test1" },
									new Attendee() { UserId = "test" }
								}
							}
						});

				mockUserManager.Setup(u => u.FindByIdAsync("test1")).ReturnsAsync(new ChatLeUser());
				mockUserManager.Setup(u => u.FindByIdAsync("test")).ReturnsAsync(new ChatLeUser());
				var result = await controller.Get();
				Assert.NotNull(result);
				Assert.NotEmpty(result);
				Assert.Equal(1, result.Count());
				var conversation = result.First();
				Assert.NotNull(conversation.Messages);
				Assert.Empty(conversation.Messages);
			});
		}

		[Fact]
		public void SendMessageTest()
		{
			ExecuteAction(async (controller, mockChatManager, mockHubContext, mockUserManager) =>
			{
				var mockHttpContext = new Mock<HttpContext>();
				var mockClaims = new Mock<ClaimsPrincipal>();
				var mockIndentity = new Mock<IIdentity>();
				mockIndentity.SetupGet(i => i.Name).Returns("test");
				mockClaims.SetupGet(c => c.Identity).Returns(mockIndentity.Object);
				mockHttpContext.SetupGet(h => h.User).Returns(mockClaims.Object);
				controller.ActionContext.HttpContext = mockHttpContext.Object;

				mockChatManager.Setup(c => c.AddMessageAsync(It.IsAny<string>()
						, It.IsAny<string>()
						, It.IsAny<Message>()
						, It.IsAny<CancellationToken>()))
					.ReturnsAsync(new Conversation()
					{
						Attendees = new List<Attendee<string>>()
						{
							new Attendee()
						}
					});

				mockUserManager.Setup(u => u.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new ChatLeUser());
				var mockHubConnectionContext = new Mock<IHubConnectionContext<dynamic>>();
				dynamic all = new ExpandoObject();
				all.messageReceived = new Action<MessageViewModel>(m => { });
				mockHubConnectionContext.Setup(h => h.Group(It.IsAny<string>())).Returns((ExpandoObject)all);
				mockHubContext.SetupGet(h => h.Clients).Returns(mockHubConnectionContext.Object);

				await controller.SendMessage("test1", "test");
			});
		}

		[Fact]
		public void CreateConversationTest()
		{
			ExecuteAction(async (controller, mockChatManager, mockHubContext, mockUserManager) =>
			{
				var mockHttpContext = new Mock<HttpContext>();
				var mockClaims = new Mock<ClaimsPrincipal>();
				var mockIndentity = new Mock<IIdentity>();
				mockIndentity.SetupGet(i => i.Name).Returns("test");
				mockClaims.SetupGet(c => c.Identity).Returns(mockIndentity.Object);
				mockHttpContext.SetupGet(h => h.User).Returns(mockClaims.Object);
				controller.ActionContext.HttpContext = mockHttpContext.Object;

				mockChatManager.Setup(c => c.GetOrCreateConversationAsync(It.IsAny<string>()
						, It.IsAny<string>()
						, It.IsAny<string>()
						, It.IsAny<CancellationToken>()))
					.ReturnsAsync(new Conversation()
					{
						Messages = new List<Message<string>>()
								{
									new Message() { UserId = "test2" }
								},
						Attendees = new List<Attendee<string>>()
								{
									new Attendee() { UserId = "test1" },
									new Attendee() { UserId = "test" }
								}
					});

				mockUserManager.Setup(u => u.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new ChatLeUser());
				var mockHubConnectionContext = new Mock<IHubConnectionContext<dynamic>>();
				dynamic all = new ExpandoObject();
				all.joinConversation = new Action<ConversationViewModel>(m => { });
				mockHubConnectionContext.Setup(h => h.Group(It.IsAny<string>())).Returns((ExpandoObject)all);
				mockHubContext.SetupGet(h => h.Clients).Returns(mockHubConnectionContext.Object);

				var result = await controller.CreateConversation("test1", "test");
				Assert.NotNull(result);

			});
		}

		[Fact]
		public void CreateConversationTest_WithNoUser()
		{
			ExecuteAction(async (controller, mockChatManager, mockHubContext, mockUserManager) =>
			{
				var mockHttpContext = new Mock<HttpContext>();
				var mockClaims = new Mock<ClaimsPrincipal>();
				var mockIndentity = new Mock<IIdentity>();
				mockIndentity.SetupGet(i => i.Name).Returns("test");
				mockClaims.SetupGet(c => c.Identity).Returns(mockIndentity.Object);
				mockHttpContext.SetupGet(h => h.User).Returns(mockClaims.Object);
				controller.ActionContext.HttpContext = mockHttpContext.Object;

				mockChatManager.Setup(c => c.GetOrCreateConversationAsync(It.IsAny<string>()
						, It.IsAny<string>()
						, It.IsAny<string>()
						, It.IsAny<CancellationToken>()))
					.ReturnsAsync(new Conversation()
					{
						Messages = new List<Message<string>>()
								{
									new Message() { UserId = "test2" }
								},
						Attendees = new List<Attendee<string>>()
								{
									new Attendee() { UserId = "test1" },
									new Attendee() { UserId = "test" }
								}
					});

				mockUserManager.Setup(u => u.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(null);

				var result = await controller.CreateConversation("test1", "test");
				Assert.Null(result);
			});

		}

		[Fact]
		public void CreateConversationTest_DisconnectedUser()
		{
			ExecuteAction(async (controller, mockChatManager, mockHubContext, mockUserManager) =>
			{
				var mockHttpContext = new Mock<HttpContext>();
				var mockClaims = new Mock<ClaimsPrincipal>();
				var mockIndentity = new Mock<IIdentity>();
				mockIndentity.SetupGet(i => i.Name).Returns("test");
				mockClaims.SetupGet(c => c.Identity).Returns(mockIndentity.Object);
				mockHttpContext.SetupGet(h => h.User).Returns(mockClaims.Object);
				controller.ActionContext.HttpContext = mockHttpContext.Object;

				mockChatManager.Setup(c => c.GetOrCreateConversationAsync(It.IsAny<string>()
						, It.IsAny<string>()
						, It.IsAny<string>()
						, It.IsAny<CancellationToken>()))
					.ReturnsAsync(new Conversation()
					{
						Messages = new List<Message<string>>()
								{
									new Message() { UserId = "test2" }
								},
						Attendees = new List<Attendee<string>>()
								{
									new Attendee() { UserId = "test1" },
									new Attendee() { UserId = "test" }
								}
					});

				mockUserManager.Setup(u => u.FindByIdAsync("test")).ReturnsAsync(new ChatLeUser("test"));
				mockUserManager.Setup(u => u.FindByIdAsync("test1")).ReturnsAsync(new ChatLeUser("test1"));
				mockUserManager.Setup(u => u.FindByIdAsync("test2")).ReturnsAsync(null);

				var mockHubConnectionContext = new Mock<IHubConnectionContext<dynamic>>();
				dynamic all = new ExpandoObject();
				all.joinConversation = new Action<ConversationViewModel>(m => { });
				mockHubConnectionContext.Setup(h => h.Group(It.IsAny<string>())).Returns((ExpandoObject)all);
				mockHubContext.SetupGet(h => h.Clients).Returns(mockHubConnectionContext.Object);

				var result = await controller.CreateConversation("test1", "test");
				Assert.NotNull(result);
			});

		}

	}
}
