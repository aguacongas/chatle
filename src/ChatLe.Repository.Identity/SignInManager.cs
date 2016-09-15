using ChatLe.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ChatLe.Repository.Identity
{
    public class SignInManager: SignInManager<ChatLeUser>
    {
        public SignInManager(UserManager<ChatLeUser> userManager, 
            IHttpContextAccessor contextAccessor, 
            IUserClaimsPrincipalFactory<ChatLeUser> claimsFactory, 
            IOptions<IdentityOptions> optionsAccessor, 
            ILogger<SignInManager<ChatLeUser>> logger) 
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger)
        {
        }

        public override Task SignInAsync(ChatLeUser user, bool isPersistent, string authenticationMethod = null)
        {
            user.LastLoginDate = DateTime.UtcNow;
            return base.SignInAsync(user, isPersistent, authenticationMethod);
        }

        public override Task SignInAsync(ChatLeUser user, AuthenticationProperties authenticationProperties, string authenticationMethod = null)
        {
            user.LastLoginDate = DateTime.UtcNow;
            return base.SignInAsync(user, authenticationProperties, authenticationMethod);
        }
    }
}
