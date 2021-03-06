using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ChatLe.Models;
using ChatLe.Repository.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Authentication;

namespace ChatLe.Repository.Test
{
    public class TestRole
	{
		public string Id { get; private set; }
		public string Name { get; set; }
	}

	public interface ITestUserStore<T> : IUserStore<T>, IUserPasswordStore<T> where T : class
	{ }

    public class SignInManagerTest
    {
        private static Mock<UserManager<ChatLeUser>> MockUserManager()
		{
            var userValidators = new List<IUserValidator<ChatLeUser>>();
			var validator = new Mock<IUserValidator<ChatLeUser>>();
			userValidators.Add(validator.Object);

			var store = new Mock<ITestUserStore<ChatLeUser>>();
			var options = new Mock<IOptions<IdentityOptions>>();
			var idOptions = new IdentityOptions();
			idOptions.Lockout.AllowedForNewUsers = false;
			options.Setup(o => o.Value).Returns(idOptions);
			var pwdValidators = new List<PasswordValidator<ChatLeUser>>();
			pwdValidators.Add(new PasswordValidator<ChatLeUser>());

			var services = new ServiceCollection();
			services.AddEntityFrameworkInMemoryDatabase();

			var userManager = new Mock<UserManager<ChatLeUser>>(store.Object, options.Object, new PasswordHasher<ChatLeUser>(),
				userValidators, pwdValidators, new UpperInvariantLookupNormalizer(),
				new IdentityErrorDescriber(), services.BuildServiceProvider(),
				new Mock<ILogger<UserManager<ChatLeUser>>>().Object);

			return userManager;		
        }
        
        [Fact]
        public async Task SignInAsync_should_update_the_last_login_date()
        {
            var authServiceMock = new Mock<IAuthenticationService>();
            var serviceProviderMock = new Mock<IServiceProvider>();

            serviceProviderMock.Setup(s => s.GetService(It.IsAny<Type>())).Returns(authServiceMock.Object);
            var context = new Mock<HttpContext>();
            context.SetupGet(c => c.RequestServices)
                .Returns(serviceProviderMock.Object);
			var contextAccessor = new Mock<IHttpContextAccessor>();
			contextAccessor.Setup(a => a.HttpContext).Returns(context.Object);

			var roleManager = new RoleManager<TestRole>(new Mock<IRoleStore<TestRole>>().Object, new RoleValidator<TestRole>[] { new RoleValidator<TestRole>() }, null, null, null);
			var identityOptions = new IdentityOptions();
			var options = new Mock<IOptions<IdentityOptions>>();
			options.Setup(a => a.Value).Returns(identityOptions);
            var userManagerMock = MockUserManager();
            userManagerMock.Setup(m => m.UpdateAsync(It.IsAny<ChatLeUser>()))
                .ReturnsAsync(IdentityResult.Success);

            var claimsFactoryMock = new Mock<IUserClaimsPrincipalFactory<ChatLeUser>>();
            claimsFactoryMock.Setup(f => f.CreateAsync(It.IsAny<ChatLeUser>()))
                .ReturnsAsync(new ClaimsPrincipal());

            var loggerMock = new Mock<ILogger<SignInManager<ChatLeUser>>>();
            var authSchemeProvider = new Mock<IAuthenticationSchemeProvider>();
            var confirmationMock = new Mock<IUserConfirmation<ChatLeUser>>();
            var manager = new SignInManager(userManagerMock.Object,
                contextAccessor.Object, 
                claimsFactoryMock.Object, 
                options.Object, 
                loggerMock.Object, 
                authSchemeProvider.Object,
                confirmationMock.Object);

            var user = new ChatLeUser { Id = "test", UserName = "test" };
            await manager.SignInAsync(user, isPersistent: false);

            Assert.NotEqual(DateTime.MinValue, user.LastLoginDate);
            userManagerMock.Verify(m => m.UpdateAsync(It.IsAny<ChatLeUser>()), Times.Once);
        }
    }
}