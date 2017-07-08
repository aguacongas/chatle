using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ChatLe.Models;
using ChatLe.Repository.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

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
            var authenticationManagerMock = new Mock<AuthenticationManager>();
            authenticationManagerMock.Setup(a => a.SignInAsync(It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.FromResult(0));
                
            var context = new Mock<HttpContext>();
            context.SetupGet(c => c.Authentication)
                .Returns(authenticationManagerMock.Object);
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

            var manager = new SignInManager(userManagerMock.Object, contextAccessor.Object, claimsFactoryMock.Object, options.Object, null, null);

            var user = new ChatLeUser { Id = "test", UserName = "test" };
            await manager.SignInAsync(user, isPersistent: false);

            Assert.NotEqual(DateTime.MinValue, user.LastLoginDate);
            userManagerMock.Verify(m => m.UpdateAsync(It.IsAny<ChatLeUser>()), Times.Once);
        }
    }
}