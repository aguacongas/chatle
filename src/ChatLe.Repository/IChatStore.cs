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
        Task<TUser> FindUserByNameAsync(string userName);
        Task CreateMessageAsync(TMessage message, CancellationToken cancellationToken);
        Task CreateAttendeeAsync(TAttendee attendee, CancellationToken cancellationToken);
        Task CreateConversationAsync(TConversation conversation, CancellationToken cancellationToken);
        Task<TConversation> GetConversationAsync(TUser attendee1, TUser attendee2);
        Task UpdateUserAsync(TUser user, CancellationToken cancellationToken);
        Task<TConversation> GetConversationAsync(TKey toConversationId);
        Task<IEnumerable<TMessage>> GetMessagesAsync(TKey convId);
        Task<IEnumerable<TUser>> GetUsersConnectedAsync();
        Task CreateNotificationConnectionAsync(TNotificationConnection connection, CancellationToken cancellationToken);
        Task DeleteNotificationConnectionAsync(TNotificationConnection connection, CancellationToken cancellationToken);
        Task<TNotificationConnection> GetNotificationConnectionAsync(string connectionId, string notificationType);
        void Init();
        Task<IEnumerable<TNotificationConnection>> GetNotificationConnectionsAsync(TKey userId, string notificationType);
        Task<IEnumerable<TAttendee>> GetAttendeesAsync(TConversation conv);
        Task<IEnumerable<TConversation>> GetConversationsAsync(TKey userId);
    }
}