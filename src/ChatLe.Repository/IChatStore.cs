using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Data.Entity;

namespace ChatLe.Models
{
    public interface IChatStore<TUser>
        where TUser : IApplicationUser
    {
        Task SetConnectionStatusAsync(TUser user, string connectionId, bool status, CancellationToken cancellationToken = default(CancellationToken));
        Task<TUser> FindUserByNameAsync(string userName);
    }
}