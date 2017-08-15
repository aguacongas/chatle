using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ChatLe.Models
{
    public class ChatLeUser : IdentityUser, IChatUser<string>
    {
        public ChatLeUser()
        {
            Id = Guid.NewGuid().ToString("N");
        }

        public ChatLeUser(string userName) :this()
        {
            UserName = userName;
        }

        public DateTime LastLoginDate { get; set; }
    }
}