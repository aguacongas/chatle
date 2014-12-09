using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Data.Entity;
using System.Collections.Generic;

namespace ChatLe.Models
{
    public interface IChatStore<TKey, TUser, TConversation, TAttendee, TMessage, TNotificationConnection>
        where TUser : IChatUser<TKey>
        where TConversation : Conversation<TKey>
        where TAttendee : Attendee<TKey>
        where TMessage : Message<TKey>
        where TNotificationConnection : NotificationConnection<TKey>
    {
        Task<TUser> FindUserByNameAsync(string userName, CancellationToken cancellationToken);
        Task CreateMessageAsync(TMessage message, CancellationToken cancellationToken);
        Task CreateAttendeeAsync(TAttendee attendee, CancellationToken cancellationToken);
        Task CreateConversationAsync(TConversation conversation, CancellationToken cancellationToken);
        Task<TConversation> GetConversationAsync(TUser attendee1, TUser attendee2, CancellationToken cancellationToken);
        Task UpdateUserAsync(TUser user, CancellationToken cancellationToken);
        Task<TConversation> GetConversationAsync(TKey toConversationId, CancellationToken cancellationToken);
        Task<IEnumerable<TMessage>> GetMessagesAsync(TKey convId, CancellationToken cancellationToken);
        Task<IEnumerable<TUser>> GetUsersConnectedAsync(CancellationToken cancellationToken);
        Task CreateNotificationConnectionAsync(TNotificationConnection connection, CancellationToken cancellationToken);
        Task DeleteNotificationConnectionAsync(TNotificationConnection connection, CancellationToken cancellationToken);
        Task<TNotificationConnection> GetNotificationConnectionAsync(string connectionId, string notificationType, CancellationToken cancellationToken);
        void Init();
        Task<IEnumerable<TNotificationConnection>> GetNotificationConnectionsAsync(TKey userId, string notificationType, CancellationToken cancellationToken);
        Task<IEnumerable<TAttendee>> GetAttendeesAsync(TConversation conv, CancellationToken cancellationToken);
        Task<IEnumerable<TConversation>> GetConversationsAsync(TKey userId, CancellationToken cancellationToken);
    }
}