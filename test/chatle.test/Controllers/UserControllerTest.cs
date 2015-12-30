using ChatLe.Controllers;
using ChatLe.ViewModels;
using System;
using Xunit;

namespace Chatle.test.Controllers
{
    public class UserControllerTest
    {
        [Fact]
        public void GetUsersTest()
        {
            var provider = TestUtils.GetServiceProvider();
            var manager = provider.GetService(typeof(IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>)) as IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>;
            
            using (var controller = new UserController(manager))
            {
                var users = controller.Get();
            }                
        }
    }
}