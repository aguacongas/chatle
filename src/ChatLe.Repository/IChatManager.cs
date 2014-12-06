using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChatLe.Models
{
    public interface IChatManager<TKey, TUser, TConversation, TAttendee, TMessage, TNotificationConnection> 
        where TUser : IChatUser<TKey>
        where TConversation : Conversation<TKey>
        where TAttendee : Attendee<TKey>
        where TMessage : Message<TKey>
        where TNotificationConnection : NotificationConnection<TKey>
    {
        IChatStore<TKey, TUser, TConversation, TAttendee, TMessage, TNotificationConnection> Store { get; }
        Task AddConnectionIdAsync (string userName, string connectionId, string notificationType, CancellationToken cancellationToken = default(CancellationToken));
        Task RemoveConnectionIdAsync(string userName, string connectionId, string notificationType, CancellationToken cancellationToken = default(CancellationToken));
        Task<TConversation> GetOrCreateConversationAsync(string from, string to, string inialMessage = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<TConversation> AddMessageAsync(string fromName, TKey toConversationId, TMessage message, CancellationToken cancellationToken = default(CancellationToken));
        Task<IEnumerable<TUser>> GetUsersConnectedAsync();
        Task<IEnumerable<TMessage>> GetMessagesAsync(TKey id);
        Task<IEnumerable<TNotificationConnection>> GetNotificationConnectionsAsync(TKey userId, string notificationType);
        Task<IEnumerable<TConversation>> GetConversationsAsync(string userName);
    }
}