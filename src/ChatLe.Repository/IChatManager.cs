using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChatLe.Models
{
    public interface IChatManager<TKey, TUser, TConversation, TAttendee, TMessage> 
        where TUser : IChatUser<TKey>
        where TConversation : Conversation<TKey>
        where TAttendee : Attendee<TKey>
        where TMessage : Message<TKey>
    {
        IChatStore<TKey, TUser, TConversation, TAttendee, TMessage> Store { get; }
        Task AddConnectionIdAsync (string userName, string connectionId, CancellationToken cancellationToken = default(CancellationToken));
        Task RemoveConnectionIdAsync(string userName, string connectionId, CancellationToken cancellationToken = default(CancellationToken));
        Task<TConversation> GetOrCreateConversationAsync(string from, string to, string inialMessage = null);
        Task AddMessageAsync(string fromName, TKey toConversationId, TMessage message);
        Task<IEnumerable<TUser>> GetUsersConnectedAsync();
        Task<IEnumerable<TMessage>> GetMessagesAsync(TKey id);
    }
}