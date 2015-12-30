using System.Threading.Tasks;
using ChatLe.ViewModels;
using Microsoft.AspNet.Authorization;
using System.Security.Claims;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR.Infrastructure;
using ChatLe.Hubs;

namespace ChatLe.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public AccountController(UserManager<ChatLeUser> userManager, SignInManager<ChatLeUser> signInManager, IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection> chatManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            ChatManager = chatManager;
        }

        public UserManager<ChatLeUser> UserManager { get; private set; }
        public SignInManager<ChatLeUser> SignInManager { get; private set; }

        public IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection> ChatManager { get; private set; }

        // GET: /Account/Index
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index(string returnUrl = null, string reason = null)
        {
            ViewBag.ReturnUrl = returnUrl;
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
        // GET: /Account/Manage
        [HttpGet]
        public IActionResult Manage(ManageMessageId? message = null)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(ManageUserViewModel model)
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
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff([FromServices] IConnectionManager signalRConnectionManager, string reason = null)
        {
            var user = await GetCurrentUserAsync();
			if (user != null)
			{
				if (user.PasswordHash == null)
				{
					var hub = signalRConnectionManager.GetHubContext<ChatHub>();
					hub.Clients.All.userDisconnected(user.UserName);
					await ChatManager.RemoveUserAsync(user);
				}
			}
            await SignInManager.SignOutAsync();
			return RedirectToAction("Index", routeValues: new { Reason = reason });
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(error.Code, error.Description);
        }

        private async Task<ChatLeUser> GetCurrentUserAsync()
        {
            return await UserManager.FindByIdAsync(HttpContext.User.GetUserId());
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            Error
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index", "Home");
        }
        #endregion
    }
}