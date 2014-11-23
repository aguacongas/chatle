using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Data.Entity;
using System.Collections.Generic;

namespace ChatLe.Models
{
    public interface IChatStore<TUser>
        where TUser : IApplicationUser
    {
        Task SetConnectionStatusAsync(TUser user, string connectionId, bool status, CancellationToken cancellationToken = default(CancellationToken));
        Task<TUser> FindUserByNameAsync(string userName);
        Task AddMessageAsync(Conversation conversation, Message message);
        Task CreateConversationAsync(TUser attendee1, TUser attendee2);
        Task<Conversation> GetConversationAsync(TUser attendee1, TUser attendee2);
    }
}