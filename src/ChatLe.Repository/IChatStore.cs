using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Data.Entity;
using System.Collections.Generic;

namespace ChatLe.Models
{
    public interface IChatStore<TKey, TUser, TConversation, TAttendee, TMessage>
        where TUser : IChatUser<TKey>
        where TConversation : Conversation<TKey>
        where TAttendee : Attendee<TKey>
        where TMessage : Message<TKey>
    {
        Task<TUser> FindUserByNameAsync(string userName);
        Task CreateMessageAsync(TMessage message);
        Task CreateAttendeeAsync(TAttendee attendee);
        Task CreateConversationAsync(TConversation conversation);
        Task<TConversation> GetConversationAsync(TUser attendee1, TUser attendee2);
        Task UpdateUserAsync(TUser user, CancellationToken cancellationToken);
        Task<TConversation> GetConversationAsync(TKey toConversationId);
        Task<IEnumerable<TMessage>> GetMessagesAsync(TKey convId);
        Task<IEnumerable<TUser>> GetUsersConnectedAsync();
    }
}