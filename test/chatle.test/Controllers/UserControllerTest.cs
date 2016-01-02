using ChatLe.Controllers;
using ChatLe.Models;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Chatle.test.Controllers
{
    public class UserControllerTest
    {
        [Fact]
        public async Task GetUsersTest()
        {
			var mockManager = new Mock<IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>>();
			mockManager.Setup(m => m.GetUsersConnectedAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(new Page<ChatLeUser>(new List<ChatLeUser>()
			{
				new ChatLeUser()
			}, 0, 1));


			using (var controller = new UserController(mockManager.Object))
            {
                var users = await controller.Get();
				Assert.NotNull(users);
				Assert.NotEmpty(users.Users);
				Assert.Equal(1, users.PageCount);
				Assert.Equal(0, users.PageIndex);
            }                
        }
    }
}