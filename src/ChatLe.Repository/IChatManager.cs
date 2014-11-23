﻿using System.Threading;
using System.Threading.Tasks;

namespace ChatLe.Models
{
    public interface IChatManager<TUser> where TUser : IApplicationUser
    {
        IChatStore<TUser> Store { get; }

        Task<bool> SetConnectionStatusAsync(string userName, string connectionId, bool isConnected, CancellationToken cancellationToken = default(CancellationToken));
    }
}