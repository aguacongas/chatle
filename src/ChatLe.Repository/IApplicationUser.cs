using System.Collections.Generic;

namespace ChatLe.Models
{
    public interface IApplicationUser
    {
        ICollection<Conversation> Conversations { get; }
        bool IsConnected { get; set; }
        string SignalRConnectionId { get; set; }
    }
}