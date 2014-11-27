﻿using ChatLe.Controllers;
using ChatLe.Models;
using System;
using Xunit;

namespace chatle.test.Controllers
{
    public class UserControllerTest
    {
        [Fact]
        public void GetUsersTest()
        {
            var provider = TestUtils.GetServiceProvider();
            var dbContext = provider.GetService(typeof(ChatLeIdentityDbContext)) as ChatLeIdentityDbContext;

            using (var controller = new UserController(dbContext))
            {
                var users = controller.Get();
            }                
        }
    }
}