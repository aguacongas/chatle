using System;
using System.Collections.Generic;

namespace ChatLe.Models
{
    public interface IChatUser<TKey> where TKey : IEquatable<TKey>
    {
        TKey Id { get; set; }
        string UserName { get; set; }
        ICollection<NotificationConnection<TKey>> NotificationConnections { get; }

        string PasswordHash { get; }
    }
}