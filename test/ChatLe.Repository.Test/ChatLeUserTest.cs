using ChatLe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ChatLe.Repository.Test
{
    public class ChatLeUserTest
    {
        [Fact]
        public void ConstrutorTest()
        {
            var user = new ChatLeUser("test");
            Assert.Equal("test", user.UserName);
        }
    }
}
