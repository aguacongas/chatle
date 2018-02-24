using Aguacongas.Firebase;
using ChatLe.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChatLe.Repository.Identity.Firebase
{
    public class FirebaseChatStore<TUser, TConversation, TAttendee, TMessage, TNotificationConnection> : IChatStore<string, TUser, TConversation, TAttendee, TMessage, TNotificationConnection>
        where TUser : IdentityUser<string>, IChatUser<string>
        where TConversation : Conversation<string>
        where TAttendee : Attendee<string>
        where TMessage : Message<string>
        where TNotificationConnection : NotificationConnection<string>
    {
        private const string ConversationTableName = "conversations";
        private const string AttendeeTableName = "attendees";
        private const string MessageSubTableName = "messages";
        private const string NotificationConnectionsTableName = "connections";
        private const string UserCountTableName = "connections-count";
        private const string RulePath = ".settings/rules.json";

        private readonly IFirebaseClient _client;
        private readonly IUserStore<TUser> _userStore;        

        public FirebaseChatStore(IFirebaseClient client, IUserStore<TUser> userStore)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _userStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
        }
        public virtual Task CreateAttendeeAsync(TAttendee attendee, CancellationToken cancellationToken = default(CancellationToken))
        {
            attendee = attendee ?? throw new ArgumentNullException(nameof(attendee));

            return _client.PostAsync(GetFirebasePath(AttendeeTableName), attendee, cancellationToken);
        }
        
        public virtual async Task CreateConversationAsync(TConversation conversation, CancellationToken cancellationToken = default(CancellationToken))
        {
            conversation = conversation ?? throw new ArgumentNullException(nameof(conversation));

            var result = await _client.PostAsync(GetFirebasePath(ConversationTableName), conversation, cancellationToken);
            conversation.Id = result.Data;
        }

        public virtual async Task CreateMessageAsync(TMessage message, CancellationToken cancellationToken = default(CancellationToken))
        {
            message = message ?? throw new ArgumentNullException(nameof(message));

            var result = await _client.PostAsync(GetFirebasePath(ConversationTableName, message.ConversationId, MessageSubTableName), message, cancellationToken);
            message.Id = result.Data;
        }

        public virtual Task CreateNotificationConnectionAsync(TNotificationConnection connection, CancellationToken cancellationToken = default(CancellationToken))
        {
            connection = connection ?? throw new ArgumentNullException(nameof(connection));
            return _client.PutAsync(GetFirebasePath(NotificationConnectionsTableName, connection.ConnectionId), cancellationToken);
        }

        public virtual Task DeleteNotificationConnectionAsync(TNotificationConnection connection, CancellationToken cancellationToken = default(CancellationToken))
        {
            connection = connection ?? throw new ArgumentNullException(nameof(connection));
            return _client.DeleteAsync(GetFirebasePath(NotificationConnectionsTableName, connection.ConnectionId), cancellationToken);
        }

        public virtual Task DeleteUserAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            user = user ?? throw new ArgumentNullException(nameof(user));

            return _userStore.DeleteAsync(user, cancellationToken);
        }

        public virtual Task<TUser> FindUserByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
            => GetLoginStore().FindByIdAsync(id, cancellationToken);

        public virtual Task<TUser> FindUserByNameAsync(string userName, CancellationToken cancellationToken = default(CancellationToken))
            => GetLoginStore().FindByNameAsync(userName, cancellationToken);

        public async virtual Task<TConversation> GetConversationAsync(TUser attendee1, TUser attendee2, CancellationToken cancellationToken = default(CancellationToken))
        {
            attendee1 = attendee1 ?? throw new ArgumentNullException(nameof(attendee1));
            attendee2 = attendee2 ?? throw new ArgumentNullException(nameof(attendee2));

            var results = await Task.WhenAll(_client.GetAsync<Dictionary<string, TAttendee>>(GetFirebasePath(AttendeeTableName), 
                cancellationToken, 
                false, 
                $"orderBy=\"UserId\"&equalTo=\"{attendee1.Id}\""),
                _client.GetAsync<Dictionary<string, TAttendee>>(GetFirebasePath(AttendeeTableName),
                cancellationToken,
                false,
                $"orderBy=\"UserId\"&equalTo=\"{attendee2.Id}\""));

            var match = from id1 in results[0].Data.Values.Select(a => a.ConversationId)
                        join id2 in results[1].Data.Values.Select(a => a.ConversationId)
                            on id1 equals id2
                        select id1;

            foreach(var conversationId in match)
            {
                var result = await _client.GetAsync<Dictionary<string, TAttendee>>(GetFirebasePath(AttendeeTableName),
                    cancellationToken,
                    false,
                    $"orderBy=\"ConverationId\"&equalTo=\"{conversationId}\"&shallow=true");

                if (result.Data.Count == 2)
                {
                    return (await _client.GetAsync<TConversation>(GetFirebasePath(ConversationTableName, conversationId))).Data;
                }
            }

            return null;
        }

        public async virtual Task<TConversation> GetConversationAsync(string toConversationId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(toConversationId))
            {
                throw new ArgumentNullException(nameof(toConversationId));
            }

            return (await _client.GetAsync<TConversation>(GetFirebasePath(ConversationTableName, toConversationId))).Data;
        }

        public async virtual Task<IEnumerable<TConversation>> GetConversationsAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            user = user ?? throw new ArgumentNullException(nameof(user));

            var conversationIds = (await _client.GetAsync<Dictionary<string, TAttendee>>(GetFirebasePath(AttendeeTableName),
                cancellationToken,
                false,
                $"orderBy=\"UserId\"&equalTo=\"{user.Id}\"")).Data.Values.Select(a => a.ConversationId);

            var taskList = new List<Task<FirebaseResponse<TConversation>>>(conversationIds.Count());
            foreach(var id in conversationIds)
            {
                taskList.Add(_client.GetAsync<TConversation>(GetFirebasePath(ConversationTableName, id)));
            }

            var results = await Task.WhenAll(taskList);

            return results.Select(r => r.Data);
        }

        public async virtual Task<IEnumerable<TMessage>> GetMessagesAsync(string convId, int max = 50, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(convId))
            {
                throw new ArgumentNullException(nameof(convId));
            }

            return (await _client.GetAsync<Dictionary<string, TMessage>>(GetFirebasePath(AttendeeTableName),
                cancellationToken,
                false,
                $"orderBy=\"Date\"&limitToLast=50")).Data.Values.OrderByDescending(m => m.Date);
        }

        public async virtual Task<TNotificationConnection> GetNotificationConnectionAsync(string connectionId, string notificationType, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(connectionId))
            {
                throw new ArgumentNullException(nameof(connectionId));
            }
            if (string.IsNullOrEmpty(notificationType))
            {
                throw new ArgumentNullException(nameof(notificationType));
            }

            return (await _client.GetAsync<TNotificationConnection>(GetFirebasePath(NotificationConnectionsTableName, connectionId), cancellationToken)).Data;
        }

        public async virtual Task<Page<TUser>> GetUsersConnectedAsync(int pageIndex = 0, int pageLength = 50, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await _client.GetAsync<int>(GetFirebasePath(UserCountTableName), cancellationToken, false, "orderBy=\"ConnectionDate\"");
            var count = result.Data;

            var limitBottom = count - (pageIndex * pageLength);
            var limitTop = limitBottom - pageIndex;
            var userIds = (await _client.GetAsync<Dictionary<string, TNotificationConnection>>(GetFirebasePath(NotificationConnectionsTableName),
                cancellationToken,
                false,
                $"orderBy=\"ConnectionDate\"&limitToFirst={limitTop}&limitToLast={limitBottom}")).Data.Values.OrderByDescending(n => n.ConnectionDate).Select(n => n.UserId);

            var taskList = new List<Task<TUser>>(userIds.Count());
            foreach(var id in userIds)
            {
                taskList.Add(_userStore.FindByIdAsync(id, cancellationToken));
            }
            var users = await Task.WhenAll(taskList);
            return new Page<TUser>(users, pageIndex, count / pageLength);
        }

        public virtual void Init()
        {
            var response = _client.GetAsync<FirebaseRules>(RulePath).GetAwaiter().GetResult();
            var rules = response.Data ?? new FirebaseRules();
            var indexes = rules.Rules ?? new Dictionary<string, object>();
            indexes[AttendeeTableName] = new FirebaseIndexes
            {
                On = new string[] { "UserId", "ConversationId" }
            };
            indexes[ConversationTableName] = new Dictionary<string, object>{
                {
                    MessageSubTableName,
                    new FirebaseIndex
                    {
                        On = "Date"
                    }
                }
            };
            indexes[NotificationConnectionsTableName] = new FirebaseIndexes
            {
                On = new string[] { "ConnectionDate", "UserId" }
            };

            _client.PutAsync(RulePath, rules).GetAwaiter().GetResult();
        }

        public virtual async Task<bool> IsGuess(TUser user, CancellationToken cancellationToken = default(CancellationToken)) 
            => (await GetLoginStore().GetLoginsAsync(user, cancellationToken)).Any();
       
        public async virtual Task<bool> UserHasConnectionAsync(TUser user)
        {
            var result = await _client.GetAsync<Dictionary<string, TNotificationConnection>>(GetFirebasePath(NotificationConnectionsTableName), default(CancellationToken), false, $"orderBy=\"UserId\"&equalTo=\"{user.Id}\"");
            return result.Data != null && result.Data.Count > 0;
        }

        protected virtual IUserLoginStore<TUser> GetLoginStore()
            => _userStore is IUserLoginStore<TUser> ? _userStore as IUserLoginStore<TUser> : throw new InvalidOperationException("User store doesn't implement IUserLoginStore<TUser>");

        private string GetFirebasePath(params string[] segments)
        {
            return string.Join("/", segments);
        }
    }
}
