using ChatLe.Controllers;
using ChatLe.Models;
using Microsoft.AspNetCore.Identity;
using System;
using Moq;
using Xunit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using ChatLe.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Hubs;
using System.Dynamic;
using ChatLe.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using ChatLe.Repository.Identity;
using System.Security.Principal;
using Microsoft.AspNetCore.Http.Authentication;

namespace Chatle.test.Controllers
{
    public class TestRole
    {
        public string Id { get; private set; }
        public string Name { get; set; }
    }

    public interface ITestUserStore<T> : IUserStore<T>, IUserPasswordStore<T> where T : class
    { }

    public class AccountControllerTest
    {
        private static UserManager<TUser> GetUserManager<TUser>(List<IUserValidator<TUser>> userValidators) where TUser : class
        {
            var store = new Mock<ITestUserStore<TUser>>();
            store.Setup(s => s.CreateAsync(It.IsAny<TUser>(), It.IsAny<CancellationToken>())).ReturnsAsync(IdentityResult.Success);
            var options = new Mock<IOptions<IdentityOptions>>();
            var idOptions = new IdentityOptions();
            idOptions.Lockout.AllowedForNewUsers = false;
            options.Setup(o => o.Value).Returns(idOptions);
            var pwdValidators = new List<PasswordValidator<TUser>>();
            pwdValidators.Add(new PasswordValidator<TUser>());
            var userManager = new UserManager<TUser>(store.Object, options.Object, new PasswordHasher<TUser>(),
                userValidators, pwdValidators, new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(), null,
                new Mock<ILogger<UserManager<TUser>>>().Object);

            return userManager;
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

            var services = new ServiceCollection();
            services.AddEntityFrameworkInMemoryDatabase();

            var userManager = new Mock<UserManager<TUser>>(store.Object, options.Object, new PasswordHasher<TUser>(),
                userValidators, pwdValidators, new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(), services.BuildServiceProvider(),
                new Mock<ILogger<UserManager<TUser>>>().Object);

            return userManager;
        }


        private static Mock<SignInManager> MockSigninManager<TUser>(UserManager<TUser> userManager) where TUser : class
        {
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(a => a.HttpContext).Returns(context.Object);

            var roleManager = new RoleManager<TestRole>(new Mock<IRoleStore<TestRole>>().Object, new RoleValidator<TestRole>[] { new RoleValidator<TestRole>() }, null, null, null, null);
            var identityOptions = new IdentityOptions();
            var options = new Mock<IOptions<IdentityOptions>>();
            options.Setup(a => a.Value).Returns(identityOptions);
            var claimsFactory = new UserClaimsPrincipalFactory<TUser, TestRole>(userManager, roleManager, options.Object);
            return new Mock<SignInManager>(userManager, contextAccessor.Object, claimsFactory, options.Object, null);
        }


        [Fact]
        public async Task RegisterTest()
        {
            var userValidators = new List<IUserValidator<ChatLeUser>>();
            var validator = new Mock<IUserValidator<ChatLeUser>>();
            userValidators.Add(validator.Object);
            var userManager = GetUserManager(userValidators);

            validator.Setup(v => v.ValidateAsync(userManager, It.IsAny<ChatLeUser>()))
               .Returns(Task.FromResult(IdentityResult.Success)).Verifiable();

            var signinManager = MockSigninManager<ChatLeUser>(userManager);
            signinManager.Setup(m => m.SignInAsync(It.IsAny<ChatLeUser>(), It.IsAny<bool>(), null)).Returns(Task.FromResult(0)).Verifiable();
            var chatManager = new Mock<IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>>().Object;
            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());

            var mockLogger = new Mock<ILogger>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            mockLoggerFactory.Setup(m => m.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);

            using (var controller = new AccountController(userManager, signinManager.Object, chatManager, mockLoggerFactory.Object) { ViewData = viewData })
            {
                var result = await controller.Register(new RegisterViewModel()
                {
                    ConfirmPassword = "test123",
                    Password = "Test123-123",
                    UserName = "test"
                });

                Assert.IsType<RedirectToActionResult>(result);
            }
        }

        [Fact]
        public async Task RegisterFailedTest()
        {
            var userValidators = new List<IUserValidator<ChatLeUser>>();
            var validator = new Mock<IUserValidator<ChatLeUser>>();
            userValidators.Add(validator.Object);
            var userManagerMock = MockUserManager(userValidators);
            userManagerMock.Setup(u => u.CreateAsync(It.IsAny<ChatLeUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError()
                {
                    Code = "test",
                    Description = "test"
                }));
            var userManager = userManagerMock.Object; ;

            var signinManager = MockSigninManager<ChatLeUser>(userManager);
            var metaDataProvider = new Mock<IModelMetadataProvider>().Object;
            var chatManager = new Mock<IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>>().Object;
            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());

            var mockLogger = new Mock<ILogger>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            mockLoggerFactory.Setup(m => m.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);

            using (var controller = new AccountController(userManager, signinManager.Object, chatManager, mockLoggerFactory.Object) { ViewData = viewData })
            {
                var result = await controller.Register(new RegisterViewModel()
                {
                    ConfirmPassword = "test123",
                    Password = "test123",
                    UserName = "test"
                });

                Assert.IsType<ViewResult>(result);
            }
        }

        [Fact]
        public void GetRegisterTest()
        {
            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
            using (var controller = new AccountController(null, null, null, null) { ViewData = viewData })
            {
                var result = controller.Register();
                Assert.IsType<ViewResult>(result);
            }
        }

        [Fact]
        public void IndexTest()
        {
            var mockCookies = new Mock<IResponseCookies>();
            var mockHttpResponse = new Mock<HttpResponse>();
            mockHttpResponse.SetupGet(r => r.Cookies)
                .Returns(mockCookies.Object);
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.SetupGet(c => c.Response)
                .Returns(mockHttpResponse.Object);

            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
            using (var controller = new AccountController(null, null, null, null) { ViewData = viewData })
            {
                controller.ControllerContext.HttpContext = mockHttpContext.Object;

                var result = controller.Index();
                Assert.IsType<ViewResult>(result);
            }
        }

        [Fact]
        public async Task LoginTest()
        {
            var userValidators = new List<IUserValidator<ChatLeUser>>();
            var validator = new Mock<IUserValidator<ChatLeUser>>();
            userValidators.Add(validator.Object);
            var userManager = GetUserManager(userValidators);

            validator.Setup(v => v.ValidateAsync(userManager, It.IsAny<ChatLeUser>()))
               .Returns(Task.FromResult(IdentityResult.Success)).Verifiable();

            var signinManager = MockSigninManager<ChatLeUser>(userManager);
            signinManager.Setup(m => m.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(Task.FromResult(Microsoft.AspNetCore.Identity.SignInResult.Success));
            var chatManager = new Mock<IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>>().Object;
            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());

            var loginViewModel = new LoginViewModel()
            {
                Password = "test",
                RememberMe = true,
                UserName = "test"
            };

            using (var controller = new AccountController(userManager, signinManager.Object, chatManager, null) { ViewData = viewData })
            {
                controller.Url = new Mock<IUrlHelper>().Object;
                var result = await controller.Login(loginViewModel, null);
                Assert.IsType<RedirectToActionResult>(result);

                signinManager.Setup(m => m.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(Task.FromResult(Microsoft.AspNetCore.Identity.SignInResult.Failed));

                result = await controller.Login(loginViewModel);


                Assert.IsType<ViewResult>(result);
            }
        }

        [Fact]
        public async Task GuessTest()
        {
            var userValidators = new List<IUserValidator<ChatLeUser>>();
            var validator = new Mock<IUserValidator<ChatLeUser>>();
            userValidators.Add(validator.Object);
            var userManager = MockUserManager<ChatLeUser>(userValidators);
            userManager.Setup(u => u.CreateAsync(It.IsAny<ChatLeUser>())).ReturnsAsync(IdentityResult.Success);
            validator.Setup(v => v.ValidateAsync(userManager.Object, It.IsAny<ChatLeUser>()))
               .Returns(Task.FromResult(IdentityResult.Success)).Verifiable();

            var signinManager = MockSigninManager<ChatLeUser>(userManager.Object);
            var chatManager = new Mock<IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>>().Object;
            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());

            var guessViewModel = new GuessViewModel()
            {
                UserName = "test"
            };

            using (var controller = new AccountController(userManager.Object, signinManager.Object, chatManager, null) { ViewData = viewData })
            {
                var result = await controller.Guess(guessViewModel);
                Assert.IsType<RedirectToActionResult>(result);

                userManager.Setup(u => u.CreateAsync(It.IsAny<ChatLeUser>())).ReturnsAsync(IdentityResult.Failed());

                result = await controller.Guess(guessViewModel);
                Assert.IsType<ViewResult>(result);
            }
        }

        [Fact]
        public async Task GetManageTest()
        {
            var storeMock = new Mock<IChatStore<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>>();
            storeMock.Setup(s => s.FindUserByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ChatLeUser());

            var chatManagerMock = new Mock<IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>>();
            chatManagerMock.SetupGet(c => c.Store)
                .Returns(storeMock.Object);

            var mockCookies = new Mock<IResponseCookies>();
            var mockHttpResponse = new Mock<HttpResponse>();
            mockHttpResponse.SetupGet(r => r.Cookies)
                .Returns(mockCookies.Object);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.SetupGet(c => c.User)
                .Returns(new ClaimsPrincipal(new Mock<IIdentity>().Object));
            mockHttpContext.SetupGet(c => c.Response)
                .Returns(mockHttpResponse.Object);

            var userValidators = new List<IUserValidator<ChatLeUser>>();
            var validator = new Mock<IUserValidator<ChatLeUser>>();
            userValidators.Add(validator.Object);
            var userManager = MockUserManager<ChatLeUser>(userValidators);
            userManager.Setup(u => u.GetLoginsAsync(It.IsAny<ChatLeUser>()))
                .ReturnsAsync(new List<UserLoginInfo>());

            var signinManager = MockSigninManager<ChatLeUser>(userManager.Object);
            signinManager.Setup(s => s.GetExternalAuthenticationSchemes())
                .Returns(new List<AuthenticationDescription>());

            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
            using (var controller = new AccountController(userManager.Object, signinManager.Object, chatManagerMock.Object, null) { ViewData = viewData })
            {
                controller.ControllerContext.HttpContext = mockHttpContext.Object;

                controller.Url = new Mock<IUrlHelper>().Object;
                var result = await controller.Manage();
                Assert.IsType<ViewResult>(result);
            }
        }

        [Fact]
        public async Task ManageTest()
        {
            var userValidators = new List<IUserValidator<ChatLeUser>>();
            var validator = new Mock<IUserValidator<ChatLeUser>>();
            userValidators.Add(validator.Object);
            var userManager = MockUserManager<ChatLeUser>(userValidators);
            userManager.Setup(u => u.ChangePasswordAsync(It.IsAny<ChatLeUser>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            validator.Setup(v => v.ValidateAsync(userManager.Object, It.IsAny<ChatLeUser>()))
               .Returns(Task.FromResult(IdentityResult.Success)).Verifiable();

            var signinManager = MockSigninManager<ChatLeUser>(userManager.Object);
            var storeMock = new Mock<IChatStore<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>>();
            storeMock.Setup(s => s.FindUserByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(new ChatLeUser());

            var chatManagerMock = new Mock<IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>>();
            chatManagerMock.SetupGet(c => c.Store)
                .Returns(storeMock.Object);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.SetupGet(c => c.User)
                .Returns(new ClaimsPrincipal(new Mock<IIdentity>().Object));

            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());

            var manageViewModel = new UpdatePasswordViewModel()
            {
                ConfirmPassword = "test",
                NewPassword = "test",
                OldPassword = "test"
            };

            using (var controller = new AccountController(userManager.Object, signinManager.Object, chatManagerMock.Object, null) { ViewData = viewData })
            {
                controller.Url = new Mock<IUrlHelper>().Object;
                controller.ControllerContext.HttpContext = mockHttpContext.Object;

                var result = await controller.UpdatePassword(manageViewModel);
                Assert.IsType<RedirectToActionResult>(result);

                userManager.Setup(u => u.ChangePasswordAsync(It.IsAny<ChatLeUser>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());

                result = await controller.UpdatePassword(manageViewModel);
                Assert.IsType<ViewResult>(result);
            }
        }


        [Fact]
        public async Task LogOffTest()
        {
            var userValidators = new List<IUserValidator<ChatLeUser>>();
            var validator = new Mock<IUserValidator<ChatLeUser>>();
            userValidators.Add(validator.Object);
            var userManager = MockUserManager<ChatLeUser>(userValidators);

            var signinManager = MockSigninManager<ChatLeUser>(userManager.Object);

            var storeMock = new Mock<IChatStore<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>>();
            storeMock.Setup(s => s.FindUserByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ChatLeUser());

            var chatManagerMock = new Mock<IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>>();
            chatManagerMock.SetupGet(c => c.Store)
                .Returns(storeMock.Object);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.SetupGet(c => c.User)
                .Returns(new ClaimsPrincipal(new Mock<IIdentity>().Object));

            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
            var mockConnectionManager = new Mock<IConnectionManager>();
            var mockHubContext = new Mock<IHubContext>();
            var mockHubConnectionContext = new Mock<IHubConnectionContext<dynamic>>();
            dynamic all = new ExpandoObject();
            all.userDisconnected = new Action<object>(o => { });
            mockHubConnectionContext.SetupGet(h => h.All).Returns((ExpandoObject)all);
            mockHubContext.SetupGet(h => h.Clients).Returns(mockHubConnectionContext.Object);
            mockConnectionManager.Setup(c => c.GetHubContext<ChatHub>()).Returns(mockHubContext.Object);

            using (var controller = new AccountController(userManager.Object, signinManager.Object, chatManagerMock.Object, null) { ViewData = viewData })
            {
                controller.Url = new Mock<IUrlHelper>().Object;

                controller.ControllerContext.HttpContext = mockHttpContext.Object;

                var result = await controller.LogOff(mockConnectionManager.Object);
                Assert.IsType<RedirectToActionResult>(result);

                storeMock.Setup(s => s.FindUserByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new ChatLeUser { PasswordHash = "test" });

                result = await controller.LogOff(mockConnectionManager.Object);
                Assert.IsType<RedirectToActionResult>(result);

                storeMock.Setup(s => s.FindUserByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(null);

                result = await controller.LogOff(mockConnectionManager.Object);
                Assert.IsType<RedirectToActionResult>(result);
            }
        }

    }
}