using System.Threading;
using System.Threading.Tasks;

namespace ChatLe.Models
{
    public interface IChatManager<TKey, TUser> where TUser : IApplicationUser<TKey>
    {
        IChatStore<TKey, TUser> Store { get; }

        Task<bool> SetConnectionStatusAsync(string userName, string connectionId, bool isConnected, CancellationToken cancellationToken = default(CancellationToken));

        Task<Conversation<TKey>> GetConversationAsync(string from, string to);

        Task AddMessage(TKey from, TKey to, string message);
    }
}