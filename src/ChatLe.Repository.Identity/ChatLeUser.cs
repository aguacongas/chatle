using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

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

        public bool IsGuess
        {
            get { return PasswordHash == null; }
        }

        public DateTime LastLoginDate { get; set; }
    }
}