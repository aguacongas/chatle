﻿using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;

namespace ChatLe.Models
{
    public class ApplicationUser : IdentityUser, IApplicationUser<string>
    {
        public ApplicationUser()
        {
            Id = Guid.NewGuid().ToString("N");
        }
        public ApplicationUser(string userName) :this()
        {
            UserName = userName;
        }

        public bool IsConnected
        {
            get
            {
                return SignalRConnectionIds.Count > 0;
            }
        }

        public virtual ICollection<string> SignalRConnectionIds { get; } = new List<string>();
    }
}