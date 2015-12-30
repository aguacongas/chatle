using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;

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

        public virtual ICollection<NotificationConnection<string>> NotificationConnections { get; set; } = new List<NotificationConnection<string>>();
    }
}