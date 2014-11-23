using System.Collections.Generic;

namespace ChatLe.Models
{
    public interface IApplicationUser
    {
        string UserName { get; set; }
        ICollection<Conversation> Conversations { get; }
        bool IsConnected { get; set; }
        string SignalRConnectionId { get; set; }
    }
}