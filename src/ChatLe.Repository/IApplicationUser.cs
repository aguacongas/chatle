using System.Collections.Generic;

namespace ChatLe.Models
{
    public interface IApplicationUser<TKey>
    {
        TKey Id { get; set; }
        string UserName { get; set; }
        bool IsConnected { get; }
        ICollection<string> SignalRConnectionIds { get; }
    }
}