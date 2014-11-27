using ChatLe.Models;
using Microsoft.AspNet.Mvc;
using System.Collections.Generic;
using System.Linq;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ChatLe.Controllers
{
    [Route("api/users")]
    public class UserController : Controller
    {
        ChatLeIdentityDbContext _dbContext;
        public UserController(ChatLeIdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        // GET: /<controller>/
        [HttpGet()]
        public IEnumerable<string> Get()
        {
            return _dbContext.Users.Where(x => x.IsConnected && x.UserName != Context.User.Identity.Name).Select(x => x.UserName);
        }
    }
}
