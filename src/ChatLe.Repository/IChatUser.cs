using System.Collections.Generic;

namespace ChatLe.Models
{
    public interface IChatUser<TKey>
    {
        TKey Id { get; set; }
        string UserName { get; set; }
        bool IsConnected { get; }
        ICollection<string> SignalRConnectionIds { get; }
    }
}