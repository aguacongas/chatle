using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ChatLe.Hubs;
using ChatLe.Models;
using ChatLe.ViewModels;
using System.Net;
using ChatLe.Repository.Identity;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Authentication;

namespace ChatLe.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public AccountController(UserManager<ChatLeUser> userManager, 
            SignInManager signInManager,
            IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection> chatManager,
            IHubContext<ChatHub> hubContext,
            ILoggerFactory loggerFactory)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            ChatManager = chatManager;
            Logger = loggerFactory.CreateLogger<AccountController>();
            HubContext = hubContext;
        }

        public UserManager<ChatLeUser> UserManager { get; private set; }

        public SignInManager<ChatLeUser> SignInManager { get; private set; }

        public IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection> ChatManager { get; private set; }

        public ILogger Logger { get; private set; }

        public IHubContext<ChatHub> HubContext { get; private set; }

        // GET: /Account/Index
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index(string returnUrl = null, string reason = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            DeleteExternalCookie();
            return View(new LoginPageViewModel());
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var signInStatus = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (signInStatus.Succeeded)
                    return RedirectToLocal(returnUrl);

                ModelState.AddModelError("", "Invalid username or password.");
                return View(model);
            }

            // If we got this far, something failed, redisplay form
            return View("Index", new LoginPageViewModel() { Login = model });
        }

        //
        // POST: /Account/SpaLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SpaLogin([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var signInStatus = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (signInStatus.Succeeded)
                    return new JsonResult(signInStatus.Succeeded);

                ModelState.AddModelError("InvalidUserOrPAssword", "Invalid username or password.");                
            }

            return ReturnSpaError();
        }

        //
        // POST: /Account/Guess
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guess(GuessViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = new ChatLeUser { UserName = model.UserName };

                var result = await UserManager.CreateAsync(user);
                
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                    AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View("Index", new LoginPageViewModel() { Guess = model });
        }

        //
        // POST: /Account/SpaGuess
        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> SpaGuess([FromBody] GuessViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ChatLeUser { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false);
                    return new JsonResult(result.Succeeded);
                }
                else
                {
                    AddErrors(result);
                }
            }

            return ReturnSpaError();
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ChatLeUser { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                    AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff(string reason = null)
        {
            await SignOut();
			return RedirectToAction("Index", routeValues: new { Reason = reason });
        }

        //
        // POST: /Account/SpaLogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task SpaLogOff(string reason = null)
        {
            await SignOut();        
        }

        //
        // GET: /Account/ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<ExternalLoginProvider> GetExternalProviders(){
            return SignInManager.GetExternalAuthenticationSchemesAsync().Result.Select(p => new ExternalLoginProvider
                { 
                    DisplayName = p.DisplayName,
                    AuthenticationScheme = p.Name  
                });
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, UserManager.GetUserId(User));
            return Challenge(properties, provider);
        }

        //
        // GET: /Account/ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (returnUrl != null)
            {
                return await SpaExternalLogin(returnUrl, remoteError);
            }

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View(nameof(Login));
            }

            var info = await SignInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Index));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await SignInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                // Update any authentication tokens if login succeeded
                await SignInManager.UpdateExternalAuthenticationTokensAsync(info);

                return RedirectToLocal(returnUrl);
            }

            // If the user does not have an account, then ask the user to create an account.
            ViewData["LoginProvider"] = info.LoginProvider;
            var name = info.Principal.FindFirstValue(ClaimTypes.Name);

            return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = name });
        }

        private async Task<IActionResult> SpaExternalLogin(string returnUrl, string remoteError)
        {
            if (remoteError != null)
            {
                return new RedirectResult($"{returnUrl}?e={remoteError}");
            }

            var info = await SignInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return new RedirectResult($"{returnUrl}");
            }

            var name = info.Principal.FindFirstValue(ClaimTypes.Name);
            // Sign in the user with this external login provider if the user already has a login.
            var result = await SignInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                // Update any authentication tokens if login succeeded
                await SignInManager.UpdateExternalAuthenticationTokensAsync(info);
                
                var user = await UserManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                
                return new RedirectResult($"{returnUrl}?u={user.UserName}");
            }

            // If the user does not have an account, then ask the user to create an account.
            return new RedirectResult($"{returnUrl}?p={info.LoginProvider}&u={name}");
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await SignInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ChatLeUser { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false);

                        // Update any authentication tokens as well
                        await SignInManager.UpdateExternalAuthenticationTokensAsync(info);

                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }


        //
        // POST: /Account/SpaExternalLoginConfirmation
        [HttpPut]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> SpaExternalLoginConfirmation([FromBody] ExternalLoginConfirmationViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await SignInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    ModelState.AddModelError("NullInfo", "External login info is null");
                }
                else
                {
                    var user = new ChatLeUser { UserName = model.UserName };
                    var result = await UserManager.CreateAsync(user);
                    if (result.Succeeded)
                    {
                        result = await UserManager.AddLoginAsync(user, info);
                        if (result.Succeeded)
                        {
                            await SignInManager.SignInAsync(user, isPersistent: false);

                            // Update any authentication tokens as well
                            await SignInManager.UpdateExternalAuthenticationTokensAsync(info);

                            return new JsonResult("OK");
                        }
                    }

                    AddErrors(result);
                }
            }

            return ReturnSpaError();
        }

        [HttpGet]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<bool> Exists(string userName)
        {
            return await UserManager.FindByNameAsync(userName) != null;
        }

        //GET: /Manage/Manage
        [HttpGet]
        public async Task<IActionResult> Manage(ManageMessageId? message = null)
        {
            ViewData["StatusMessage"] =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.AddLoginSuccess ? "The external login was added."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";

            var user = await GetCurrentUserAsync();
            var vm = await GetLogins(user);
            ViewData["ShowRemoveButton"] = user.PasswordHash != null || vm.CurrentLogins.Count > 1;
            DeleteExternalCookie();
            return View(vm);
        }

        //
        // POST: /Account/UpdatePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordViewModel model)
        {
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync();
                var result = await UserManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                    return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                else
                    AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // PUT: /Account/ChangePassword
        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword([FromBody] UpdatePasswordViewModel model)
        {
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync();
                var result = await UserManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                    return new JsonResult(ManageMessageId.ChangePasswordSuccess);
                else
                    AddErrors(result);
            }

            return ReturnSpaError();
        }

        //
        // POST: /Account/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword([FromBody] CreatePasswordViewModel model)
        {
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync();
                var result = await UserManager.AddPasswordAsync(user, model.NewPassword);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, false);
                    return new JsonResult(ManageMessageId.ChangePasswordSuccess);
                }
                else
                    AddErrors(result);
            }

            return ReturnSpaError();
        }

        [HttpGet]
        public async Task<ManageLoginsViewModel> Logins()
        {
            var user = await GetCurrentUserAsync();
            return await GetLogins(user);
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLogin(RemoveLoginViewModel account)
        {
            if (await InternalRemoveLogin(account))
            {
                return RedirectToAction(nameof(Manage), new { ManageMessageId.RemoveLoginSuccess });
            }
            return RedirectToAction(nameof(Manage));
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task SpaRemoveLogin(RemoveLoginViewModel account)
        {
            await InternalRemoveLogin(account);
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LinkLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Action("LinkLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, UserManager.GetUserId(User));            
            return Challenge(properties, provider);
        }

        //
        // GET: /Manage/LinkLoginCallback
        [HttpGet]
        public async Task<ActionResult> LinkLoginCallback(string returnUrl = null)
        {
            var user = await GetCurrentUserAsync();

            if (returnUrl != null)
            {
                return await SpaLinkLogin(user, returnUrl);
            }

            if (user == null)
            {
                return View("Error");
            }
            var info = await SignInManager.GetExternalLoginInfoAsync(await UserManager.GetUserIdAsync(user));
            if (info == null)
            {
                return RedirectToAction(nameof(Manage), new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(user, info);
            var message = result.Succeeded ? ManageMessageId.AddLoginSuccess : ManageMessageId.Error;
            return RedirectToAction(nameof(Manage), new { Message = message });
        }

        #region Helpers

        private async Task<ActionResult> SpaLinkLogin(ChatLeUser user, string returnUrl)
        {
            if (user == null)
            {
                return new RedirectResult($"{returnUrl}&r=no-user");
            }

            var info = await SignInManager.GetExternalLoginInfoAsync(await UserManager.GetUserIdAsync(user));
            if (info == null)
            {
                return new RedirectResult($"{returnUrl}&r=no-info");
            }

            var result = await UserManager.AddLoginAsync(user, info);
            var message = result.Succeeded ? "succeed" : "error";
            return new RedirectResult($"{returnUrl}&r={message}");
        }

        private void DeleteExternalCookie()
        {
            Response.Cookies.Delete("Identity.External");
        }

        private async Task<ManageLoginsViewModel> GetLogins(ChatLeUser user)
        {
            var userLogins = await UserManager.GetLoginsAsync(user);
            var otherLogins = (await SignInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => userLogins.All(ul => auth.Name != ul.LoginProvider))
                .Select(auth => new AuthenticationDescription { AuthenticationScheme = auth.Name, DisplayName = auth.DisplayName })
                .ToList();

            return new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins,
                Passwords = user.PasswordHash != null ? new UpdatePasswordViewModel() : null                
            };
        }

        private async Task<bool> InternalRemoveLogin(RemoveLoginViewModel account)
        {
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await UserManager.RemoveLoginAsync(user, account.LoginProvider, account.ProviderKey);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false);
                    return true;
                }
            }

            return false;
        }

        private JsonResult ReturnSpaError()
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return new JsonResult(ModelState.Root.Children);            
        }

        private async Task SignOut()
        {
            var user = await GetCurrentUserAsync();
			if (user != null)
			{
                var isGuess = await ChatManager.IsGuess(user);
                await HubContext.Clients.All.InvokeAsync("userDisconnected", new { id = user.UserName, isRemoved = isGuess });
				if (isGuess)
				{
					await ChatManager.RemoveUserAsync(user);
				}
			}
            await SignInManager.SignOutAsync();
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(error.Code, error.Description);
        }

        private async Task<ChatLeUser> GetCurrentUserAsync()
        {
            return await ChatManager.Store.FindUserByNameAsync(HttpContext.User.Identity.Name);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index", "Home");
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            Error,
            AddLoginSuccess,
            RemoveLoginSuccess
        }
        #endregion
    }
}