using ChatLe.Controllers;
using ChatLe.Models;
using Microsoft.AspNet.Identity;
using System;
using Moq;
using Xunit;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.OptionsModel;
using System.Threading;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc.ModelBinding;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace chatle.test.Controllers
{
    public class TestRole
    {
        public string Id { get; private set; }
        public string Name { get; set; }
    }

    
    public class AccountControllerTest
    {
        private static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var options = new OptionsManager<IdentityOptions>(null);
            var passwordOptions = new OptionsManager<PasswordHasherOptions>(null);
            var userValidators = new List<UserValidator<TUser>>(1);
            userValidators.Add(new UserValidator<TUser>());
            var passwordValidators = new List<PasswordValidator<TUser>>(1);
            passwordValidators.Add(new PasswordValidator<TUser>());

            return new Mock<UserManager<TUser>>(
                store.Object,
                options,
                new PasswordHasher<TUser>(passwordOptions),
                userValidators,
                passwordValidators,
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                new ServiceCollection());
        }

        private static Mock<SignInManager<TUser>> MockSigninManager<TUser>(UserManager<TUser> userManager) where TUser : class
        {            
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(a => a.HttpContext).Returns(context.Object);
            var roleManager = new RoleManager<TestRole>(new Mock<IRoleStore<TestRole>>().Object,new RoleValidator<TestRole>[] { new RoleValidator<TestRole>() });
            var identityOptions = new IdentityOptions();
            var options = new Mock<IOptions<IdentityOptions>>();
            options.Setup(a => a.Value).Returns(identityOptions);
            var claimsFactory = new Mock<ClaimsIdentityFactory<TUser, TestRole>>(userManager, roleManager, options.Object);
            return new Mock<SignInManager<TUser>>(userManager, contextAccessor.Object, claimsFactory.Object, options.Object);
        }

        [Fact]
        public async Task RegisterTest()
        {            
            var userManager = MockUserManager<ChatLeUser>();
            userManager.Setup(m => m.CreateAsync(It.IsAny<ChatLeUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Verifiable();
            var signinManager = MockSigninManager<ChatLeUser>(userManager.Object);
            signinManager.Setup(m => m.SignInAsync(It.IsAny<ChatLeUser>(), It.IsAny<bool>(), null, CancellationToken.None)).Returns(Task.FromResult(0)).Verifiable();
            var chatManager = new Mock<IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>>().Object;
            var viewData = new ViewDataDictionary(new DataAnnotationsModelMetadataProvider(), new ModelStateDictionary());
            using (var controller = new AccountController(userManager.Object, signinManager.Object, chatManager) { ViewData = viewData })
            {
                var result = await controller.Register(new RegisterViewModel()
                {
                    ConfirmPassword = "test123",
                    Password = "test123",
                    UserName = "test"
                });

                Assert.IsType<RedirectToActionResult>(result);
            }
        }

        public async Task RegisterFailedTest()
        {
            var userManager = MockUserManager<ChatLeUser>();
            userManager.Setup(m => m.CreateAsync(It.IsAny<ChatLeUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed()).Verifiable();
            var signinManager = MockSigninManager<ChatLeUser>(userManager.Object);
            signinManager.Setup(m => m.SignInAsync(It.IsAny<ChatLeUser>(), It.IsAny<bool>(), null, CancellationToken.None)).Returns(Task.FromResult(0)).Verifiable();
            var metaDataProvider = new Mock<IModelMetadataProvider>().Object;
            var chatManager = new Mock<IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>>().Object;
            using (var controller = new AccountController(userManager.Object, signinManager.Object, chatManager) { ViewData = new ViewDataDictionary(metaDataProvider, new ModelStateDictionary()) })
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


    }
}