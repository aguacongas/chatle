using System.Collections.Generic;

namespace ChatLe.Models
{
    public interface IApplicationUser<TKey>
    {
        TKey Id { get; set; }
        string UserName { get; set; }
        bool IsConnected { get; set; }
        string SignalRConnectionId { get; set; }
    }
}