using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Data.Entity;
using System.Collections.Generic;

namespace ChatLe.Models
{
    public interface IChatStore<TKey, TUser>
        where TUser : IApplicationUser<TKey>
    {
        Task SetConnectionStatusAsync(TUser user, string connectionId, bool status, CancellationToken cancellationToken = default(CancellationToken));
        Task<TUser> FindUserByNameAsync(string userName);
        Task AddMessageAsync(Conversation<TKey> conversation, Message<TKey> message);
        Task AddAttendeeAsync(Conversation<TKey> conversation, TUser user);
        Task<Conversation<TKey>> CreateConversationAsync(TUser attendee1, TUser attendee2);
        Task<Conversation<TKey>> GetConversationAsync(TUser attendee1, TUser attendee2);
    }
}