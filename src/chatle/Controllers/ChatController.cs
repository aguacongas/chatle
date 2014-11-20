using ChatLe.Models;
using Microsoft.AspNet.Mvc;
using System.Collections.Generic;
using System.Linq;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ChatLe.Controllers
{
    public class ChatController : Controller
    {
        ApplicationDbContext _dbContext;
        public ChatController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        // GET: /<controller>/
        public IEnumerable<Message> Get(string id)
        {
            return _dbContext.Messages.Where(x => (x.From == id && x.To ==Context.User.Identity.Name) || (x.To == id && x.From == Context.User.Identity.Name)).OrderByDescending(x => x.Date);
        }
    }
}
