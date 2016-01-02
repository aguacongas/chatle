using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace ChatLe.Models
{
    /// <summary>
    /// Chat store interface
    /// </summary>
    /// <typeparam name="TKey">type of primary key</typeparam>
    /// <typeparam name="TUser">type of user, must be a class and implement <see cref="IChatUser{TKey}"/></typeparam>
    /// <typeparam name="TConversation">type of conversation, must be a <see cref="Conversation{TKey}"/></typeparam>
    /// <typeparam name="TAttendee">type of attendee, must be a <see cref="Attendee{TKey}"/></typeparam>
    /// <typeparam name="TMessage">type of message, must be a <see cref="Message{TKey}"/></typeparam>
    /// <typeparam name="TNotificationConnection">type of notifciation connection, must be a <see cref="NotificationConnection{TKey}"/></typeparam>
    public interface IChatStore<TKey, TUser, TConversation, TAttendee, TMessage, TNotificationConnection>
        where TKey : IEquatable<TKey>
        where TUser : IChatUser<TKey>
        where TConversation : Conversation<TKey>
        where TAttendee : Attendee<TKey>
        where TMessage : Message<TKey>
        where TNotificationConnection : NotificationConnection<TKey>
    {
        /// <summary>
        /// Find a user by her name
        /// </summary>
        /// <param name="userName">the user name</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a <see cref="Task{TUser}"/></returns>
        Task<TUser> FindUserByNameAsync(string userName, CancellationToken cancellationToken = default(CancellationToken));
		/// <summary>
		/// Find a user by her Id
		/// </summary>
		/// <param name="id">the user id</param>
		/// <param name="cancellationToken">an optional cancellation token</param>
		/// <returns>a <see cref="Task{TUser}"/></returns>
		Task<TUser> FindUserByIdAsync(TKey id, CancellationToken cancellationToken = default(CancellationToken));
		/// <summary>
		/// Create a message on the database
		/// </summary>
		/// <param name="message">The message to create</param>
		/// <param name="cancellationToken">an optional cancellation token</param>
		/// <returns>a <see cref="Task"/></returns>
		Task CreateMessageAsync(TMessage message, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Create an attendee on the database
        /// </summary>
        /// <param name="attendee">The attendee to create</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a <see cref="Task"/></returns>
        Task CreateAttendeeAsync(TAttendee attendee, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Create a conversation on the database
        /// </summary>
        /// <param name="conversation">The conversation to create</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a <see cref="Task"/></returns>
        Task CreateConversationAsync(TConversation conversation, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Update the user on the database
        /// </summary>
        /// <param name="user">The user to update</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a <see cref="Task"/></returns>
        Task<TConversation> GetConversationAsync(TUser attendee1, TUser attendee2, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Gets a conversation for 2 attendees
        /// </summary>
        /// <param name="attendee1">the 1st attendee</param>
        /// <param name="attendee2">the 2dn attendee</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a <see cref="Task{TConversation}"/></returns>
        Task UpdateUserAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Gets a conversation by her id
        /// </summary>
        /// <param name="convId">the conversation id</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a <see cref="Task{TConversation}"</returns>
        Task<TConversation> GetConversationAsync(TKey toConversationId, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Gets messages in a conversation
        /// </summary>
        /// <param name="convId">the conversation id</param>
        /// <param name="max">max number of messages to get, default is 50</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a <see cref="Task{IEnumerable{TMessage}}"/></returns>
        Task<IEnumerable<TMessage>> GetMessagesAsync(TKey convId, int max = 50, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Gets connected users
        /// </summary>
        /// <param name="pageIndex">the 1 based page index, default is 1</param>
        /// <param name="pageLength">number of user per page, default is 50</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a <see cref="Task{IEnumerable{TUser}}"/></returns>
        Task<Page<TUser>> GetUsersConnectedAsync(int pageIndex = 0, int pageLength = 50, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Deletes a user 
        /// </summary>
        /// <param name="user">the user to delete</param>
        /// <param name="cancellationToken"an optional cancellation token></param>
        /// <returns>a <see cref="Task"/></returns>
        Task DeleteUserAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Check if a user has connection
        /// </summary>
        /// <param name="userId">the <see cref="TKey"/> user id</param>
        /// <returns>true if user has connection</returns>
        Task<bool> UserHasConnectionAsync(TKey userId);

        /// <summary>
        /// Create a notification connection on the database
        /// </summary>
        /// <param name="connection">the notification connection</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a <see cref="Task"/></returns>
        Task CreateNotificationConnectionAsync(TNotificationConnection connection, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Delete a notification connection on the database
        /// </summary>
        /// <param name="connection">the notification connection</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a <see cref="Task"/></returns>
        Task DeleteNotificationConnectionAsync(TNotificationConnection connection, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Gets a notification connection by her id and her type
        /// </summary>
        /// <param name="connectionId">the notification connection id</param>
        /// <param name="notificationType">the type of notification</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a <see cref="Task{TNotificationConnection}"/></returns>
        Task<TNotificationConnection> GetNotificationConnectionAsync(string connectionId, string notificationType, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Initialise the database
        /// </summary>
        void Init();
        /// <summary>
        /// Gets notification connections for a user id and notification type
        /// </summary>
        /// <param name="userId">the user id</param>
        /// <param name="notificationType">the notification type</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a <see cref="Task{IEnumerable{TNotificationConnection}}"/></returns>
        Task<IEnumerable<TNotificationConnection>> GetNotificationConnectionsAsync(TKey userId, string notificationType, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Gets attendees in a conversation
        /// </summary>
        /// <param name="conv">the conversation</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a <see cref="Task{IEnumerable{TAttendee}}"/></returns>
        Task<IEnumerable<TAttendee>> GetAttendeesAsync(TConversation conv, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Gets conversations for a user id
        /// </summary>
        /// <param name="userId">the user id</param>
        /// <param name="cancellationToken">an optional cancellation token</param>
        /// <returns>a <see cref="Task{IEnumerable{TConversation}}"/></returns>
        Task<IEnumerable<TConversation>> GetConversationsAsync(TKey userId, CancellationToken cancellationToken = default(CancellationToken));
    }
}