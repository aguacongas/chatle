using ChatLe.Controllers;
using ChatLe.Models;
using Microsoft.AspNet.Identity;
using System;
using Moq;
using Xunit;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.OptionsModel;
using System.Threading;
using Microsoft.AspNet.Http;
using Microsoft.Framework.DependencyInjection;
using Microsoft.AspNet.Mvc.ModelBinding;
using System.Collections.Generic;

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
            return new Mock<UserManager<TUser>>(
                store.Object,
                options,
                new PasswordHasher<TUser>(),
                new UserValidator<TUser>(),
                new PasswordValidator<TUser>(),
                new UpperInvariantUserNameNormalizer(),
                new List<IUserTokenProvider<TUser>>());
        }

        private static Mock<SignInManager<TUser>> MockSigninManager<TUser>(UserManager<TUser> userManager) where TUser : class
        {            
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IContextAccessor<HttpContext>>();
            contextAccessor.Setup(a => a.Value).Returns(context.Object);
            var roleManager = new RoleManager<TestRole>(new Mock<IRoleStore<TestRole>>().Object, new RoleValidator<TestRole>());
            var identityOptions = new IdentityOptions();
            var options = new Mock<IOptions<IdentityOptions>>();
            options.Setup(a => a.Options).Returns(identityOptions);
            var claimsFactory = new Mock<ClaimsIdentityFactory<TUser, TestRole>>(userManager, roleManager, options.Object);
            return new Mock<SignInManager<TUser>>(userManager, contextAccessor.Object, claimsFactory.Object, options.Object);
        }

        [Fact]
        public async Task RegisterTest()
        {            
            var userManager = MockUserManager<ApplicationUser>();
            userManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), CancellationToken.None)).ReturnsAsync(IdentityResult.Success).Verifiable();
            var signinManager = MockSigninManager<ApplicationUser>(userManager.Object);
            signinManager.Setup(m => m.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), null, CancellationToken.None)).Returns(Task.FromResult(0)).Verifiable();
            var metaDataProvider = new Mock<IModelMetadataProvider>().Object;
            using (var controller = new AccountController(userManager.Object, signinManager.Object) { ViewData = new ViewDataDictionary(metaDataProvider, new ModelStateDictionary()) })
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
            var userManager = MockUserManager<ApplicationUser>();
            userManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), CancellationToken.None)).ReturnsAsync(IdentityResult.Failed()).Verifiable();
            var signinManager = MockSigninManager<ApplicationUser>(userManager.Object);
            signinManager.Setup(m => m.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), null, CancellationToken.None)).Returns(Task.FromResult(0)).Verifiable();
            var metaDataProvider = new Mock<IModelMetadataProvider>().Object;
            using (var controller = new AccountController(userManager.Object, signinManager.Object) { ViewData = new ViewDataDictionary(metaDataProvider, new ModelStateDictionary()) })
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