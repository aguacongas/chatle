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
using Microsoft.AspNet.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;

namespace chatle.test.Controllers
{
    public class TestRole
    {
        public string Id { get; private set; }
        public string Name { get; set; }
    }

    public interface ITestUserStore<T>:IUserStore<T>, IUserPasswordStore<T> where T :class
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
                new Mock<ILogger<UserManager<TUser>>>().Object,
                null);
           
            return userManager;
        }

        private static Mock<SignInManager<TUser>> MockSigninManager<TUser>(UserManager<TUser> userManager) where TUser : class
        {            
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(a => a.HttpContext).Returns(context.Object);
            
            var roleManager = new RoleManager<TestRole>(new Mock<IRoleStore<TestRole>>().Object,new RoleValidator<TestRole>[] { new RoleValidator<TestRole>() }, null, null, null, null);
            var identityOptions = new IdentityOptions();
            var options = new Mock<IOptions<IdentityOptions>>();
            options.Setup(a => a.Value).Returns(identityOptions);
            var claimsFactory = new UserClaimsPrincipalFactory<TUser, TestRole>(userManager, roleManager, options.Object);
            return new Mock<SignInManager<TUser>>(userManager, contextAccessor.Object, claimsFactory, options.Object, null);
        }


        [Fact]
        public async Task RegisterTest()
        {
            var userValidators = new List<IUserValidator<ChatLeUser>>();
            var validator = new Mock<IUserValidator<ChatLeUser>>();
            userValidators.Add(validator.Object);
            var userManager = GetUserManager<ChatLeUser>(userValidators);

            validator.Setup(v => v.ValidateAsync(userManager, It.IsAny<ChatLeUser>()))
               .Returns(Task.FromResult(IdentityResult.Success)).Verifiable();

            var signinManager = MockSigninManager<ChatLeUser>(userManager);
            signinManager.Setup(m => m.SignInAsync(It.IsAny<ChatLeUser>(), It.IsAny<bool>(), null)).Returns(Task.FromResult(0)).Verifiable();
            var chatManager = new Mock<IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>>().Object;
            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
            using (var controller = new AccountController(userManager, signinManager.Object, chatManager) { ViewData = viewData })
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

        public async Task RegisterFailedTest()
        {
            var userValidators = new List<IUserValidator<ChatLeUser>>();
            var validator = new Mock<IUserValidator<ChatLeUser>>();
            userValidators.Add(validator.Object);
            var userManager = GetUserManager<ChatLeUser>(userValidators);

            validator.Setup(v => v.ValidateAsync(userManager, It.IsAny<ChatLeUser>()))
               .Returns(Task.FromResult(IdentityResult.Failed(new IdentityError[] {
                   new IdentityError() { Code = "-1", Description = "test" }
               }))).Verifiable();
            
            var signinManager = MockSigninManager<ChatLeUser>(userManager);
            signinManager.Setup(m => m.SignInAsync(It.IsAny<ChatLeUser>(), It.IsAny<bool>(), null)).Returns(Task.FromResult(0)).Verifiable();
            var metaDataProvider = new Mock<IModelMetadataProvider>().Object;
            var chatManager = new Mock<IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>>().Object;
            using (var controller = new AccountController(userManager, signinManager.Object, chatManager) { ViewData = new ViewDataDictionary(metaDataProvider, new ModelStateDictionary()) })
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