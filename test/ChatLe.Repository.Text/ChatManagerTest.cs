using Microsoft.Data.Entity;
using Microsoft.Framework.DependencyInjection.Fallback;
using System;
using Xunit;

namespace ChatLe.Repository.Text
{
    public class ChatManagerTest
    {
        [Fact]
        public void Constructor()
        {
            var services = TestHelpers.GetServicesCollection();
            using (var context = new DbContext(services.BuildServiceProvider()))
            {
                var store = new ChatManager<UserTest>(context);
            }
        }
    }
}