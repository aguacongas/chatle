using ChatLe.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;

namespace ChatLe.Repository.Identity
{
    public class SignInManager: SignInManager<ChatLeUser>
    {
        public SignInManager(UserManager<ChatLeUser> userManager, 
            IHttpContextAccessor contextAccessor, 
            IUserClaimsPrincipalFactory<ChatLeUser> claimsFactory, 
            IOptions<IdentityOptions> optionsAccessor, 
            ILogger<SignInManager<ChatLeUser>> logger,
            IAuthenticationSchemeProvider schemes) 
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes)
        {
        }

        public override async Task SignInAsync(ChatLeUser user, AuthenticationProperties authenticationProperties, string authenticationMethod = null)
        {
            user.LastLoginDate = DateTime.UtcNow;
            await this.UserManager.UpdateAsync(user);
            await  base.SignInAsync(user, authenticationProperties, authenticationMethod);
        }
    }
}
