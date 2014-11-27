using System.Threading;
using System.Threading.Tasks;

namespace ChatLe.Models
{
    public interface IChatManager<TKey, TUser> where TUser : IApplicationUser<TKey>
    {
        IChatStore<TKey, TUser> Store { get; }
        Task AddConnectionIdAsync (string userName, string connectionId, CancellationToken cancellationToken = default(CancellationToken));
        Task RemoveConnectionIdAsync(string userName, string connectionId, CancellationToken cancellationToken = default(CancellationToken));
        Task<Conversation<TKey>> GetConversationAsync(string from, string to);
        Task AddMessageAsync(string fromName, TKey toConversationId, Message<TKey> message);
    }
}