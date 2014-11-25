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
        Task CreateMessageAsync(Message<TKey> message);
        Task CreateAttendeeAsync(Attendee<TKey> attendee);
        Task CreateConversationAsync(Conversation<TKey> conversation);
        Task<Conversation<TKey>> GetConversationAsync(TUser attendee1, TUser attendee2);
    }
}