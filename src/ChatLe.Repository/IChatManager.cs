using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChatLe.Models
{
    /// <summary>
    /// Interface defining chat manager
    /// </summary>
    /// <typeparam name="TKey">type of primary key</typeparam>
    /// <typeparam name="TUser">type of user, must implement <see cref="IChatUser{TKey}"/></typeparam>
    /// <typeparam name="TConversation">type of Conversation, must be <see cref="Conversation{TKey}"/></typeparam>
    /// <typeparam name="TAttendee">type of attendee, must be <see cref="Attendee{TKey}"/></typeparam>
    /// <typeparam name="TMessage">type of message, must be <see cref="Message{TKey}"/></typeparam>
    /// <typeparam name="TNotificationConnection">type of notification connecction, must be <see cref="NotificationConnection{TKey}"/></typeparam>
    public interface IChatManager<TKey, TUser, TConversation, TAttendee, TMessage, TNotificationConnection>
        where TKey : IEquatable<TKey>
        where TUser : IChatUser<TKey>
        where TConversation : Conversation<TKey>
        where TAttendee : Attendee<TKey>
        where TMessage : Message<TKey>
        where TNotificationConnection : NotificationConnection<TKey>
    {
        /// <summary>
        /// Gets the store
        /// </summary>
        IChatStore<TKey, TUser, TConversation, TAttendee, TMessage, TNotificationConnection> Store { get; }
        /// <summary>
        /// Gets the options
        /// </summary>
        ChatOptions Options { get; }
        /// <summary>
        /// Adds a notification connection assotiate to a user
        /// </summary>
        /// <param name="userName">The user name</param>
        /// <param name="connectionId">The connection id</param>
        /// <param name="notificationType">The notification type</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a Task</returns>
        Task AddConnectionIdAsync (string userName, string connectionId, string notificationType, CancellationToken cancellationToken = default(CancellationToken));
		/// <summary>
		/// Removes a notification connection assotiate to a user
		/// </summary>
		/// <param name="connectionId">The connection id</param>
		/// <param name="notificationType">the type of notification</param>
		/// <param name="cancellationToken">an optional cancellation token</param>
		/// <returns>A <see cref="Task{TUser} with the disconnected user or null if the user has an other connection or no user found</returns>
		Task<TUser> RemoveConnectionIdAsync(string connectionId, string notificationType, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Gets or creates a conversation
        /// </summary>
        /// <param name="from">The name of the 1st attendee</param>
        /// <param name="to">The name of the second attendee</param>
        /// <param name="inialMessage">The initial message if any</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a Task</returns>
        Task<TConversation> GetOrCreateConversationAsync(string from, string to, string inialMessage = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Adds a message to a conversation
        /// </summary>
        /// <param name="fromName">The sender name</param>
        /// <param name="toConversationId">The conversation id</param>
        /// <param name="message">The message</param>
        /// <param name="cancellationToken">an  optional cancellation token</param>
        /// <returns>a Task</returns>
        Task<TConversation> AddMessageAsync(string fromName, TKey toConversationId, TMessage message, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Gets a page of connected users
        /// </summary>
        /// <param name="pageIndex">the page index</param>
        /// <param name="cancellationToken">an optional concellation token</param>
        /// <returns>a Task</returns>
        Task<Page<TUser>> GetUsersConnectedAsync(int pageIndex = 0, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Gets the messages list for a conversation
        /// </summary>
        /// <param name="id">the conversation id</param>
        /// <param name="cancellationToken">an optional concellation token</param>
        /// <returns>a Task</returns>
        Task<IEnumerable<TMessage>> GetMessagesAsync(TKey id, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Gets the list of conversation for a user
        /// </summary>
        /// <param name="userName">the user name</param>
        /// <param name="cancellationToken">an optional concellation token</param>
        /// <returns>a Task</returns>
        Task<IEnumerable<TConversation>> GetConversationsAsync(string userName, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Removes a user
        /// </summary>
        /// <param name="user">the user to remove</param>
        /// <param name="cancellationToken">an optional concellation token</param>
        /// <returns>a Task</returns>
        Task RemoveUserAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken));
    }
}