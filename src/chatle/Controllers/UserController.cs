using ChatLe.Models;
using Microsoft.AspNet.Mvc;
using System.Collections.Generic;
using System.Linq;
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
        /// Gets the conneted users list
        /// </summary>
        /// <returns>a <see cref="Task<IEnumerable<dynamic>>"/> with the connected user list as result</returns>
        // GET: /<controller>/
        [HttpGet()]
        public async Task<IEnumerable<dynamic>> Get()
        {
            var users = await _manager.GetUsersConnectedAsync();
            return users.Select(u => new { Id = u.UserName });
        }
    }
}
