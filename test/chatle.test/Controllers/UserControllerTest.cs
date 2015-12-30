using ChatLe.Controllers;
using ChatLe.ViewModels;
using System.Threading.Tasks;
using Xunit;

namespace Chatle.test.Controllers
{
    public class UserControllerTest
    {
        [Fact]
        public async Task GetUsersTest()
        {
            var provider = TestUtils.GetServiceProvider();
            var manager = provider.GetService(typeof(IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>)) as IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>;
            
            using (var controller = new UserController(manager))
            {
                var users = await controller.Get();
				Assert.NotNull(users);
				Assert.Empty(users.Users);
				Assert.Equal(1, users.PageCount);
				Assert.Equal(0, users.PageIndex);
            }                
        }
    }
}