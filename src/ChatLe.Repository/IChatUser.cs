using System.Collections.Generic;

namespace ChatLe.Models
{
    public interface IChatUser<TKey>
    {
        TKey Id { get; set; }
        string UserName { get; set; }
        ICollection<NotificationConnection<TKey>> NotificationConnections { get; }
    }
}