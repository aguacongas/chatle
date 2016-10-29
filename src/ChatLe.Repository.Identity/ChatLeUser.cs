using System;
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
            get { return PasswordHash == null && Logins.Count == 0; }
        }

        public DateTime LastLoginDate { get; set; }
    }
}