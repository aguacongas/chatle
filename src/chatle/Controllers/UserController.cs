using ChatLe.Models;
using Microsoft.AspNet.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ChatLe.Controllers
{
    [Route("api/users")]
    public class UserController : Controller
    {
        IChatManager<string, ChatLeUser, Conversation, Attendee, Message> _manager;
        public UserController(IChatManager<string, ChatLeUser, Conversation, Attendee, Message> manager)
        {
            _manager = manager;
        }
        // GET: /<controller>/
        [HttpGet()]
        public async Task<IEnumerable<ChatLeUser>> Get()
        {
            return await _manager.GetUsersConnectedAsync();
        }
    }
}
