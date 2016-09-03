using ChatLe.Models;
using Xunit;

namespace ChatLe.Repository.Test
{
    public class ChatleUserTest
    {
        [Fact]
        public void ConstrutorTest()
        {
            var user = new ChatLeUser("test");
            Assert.Equal("test", user.UserName);
        }
    }
}
