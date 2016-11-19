using ChatLe.Models;
using ChatLe.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ChatLe.Controllers
{
    /// <summary>
    /// User api controller
    /// </summary>
    [Route("api/users")]
    public class UserController : Controller
    {
        IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection> _manager;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="manager">the chat repository manager</param>
        public UserController(IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection> manager)
        {
            _manager = manager;
        }
        /// <summary>
        /// Gets the a connected users list page
        /// </summary>
        /// <returns>a <see cref="Task{UserCollectionViewModel}"/> with the connected user list as result</returns>
        // GET: /<controller>/
        [HttpGet()]
        public async Task<UserCollectionViewModel> Get(int pageIndex = 0)
        {
            var users = await _manager.GetUsersConnectedAsync(pageIndex);
            return new UserCollectionViewModel(users);
        }
    }
}
