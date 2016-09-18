using System;
using System.Collections.Generic;

namespace ChatLe.Models
{
    public interface IChatUser<TKey> : IIdentifiable<TKey>where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Gets or sets the user name for this user.
        /// </summary>
        string UserName { get; set; }
        /// <summary>
        /// Gets the notication sytem connection collection for this uses.
        /// </summary>
        ICollection<NotificationConnection<TKey>> NotificationConnections { get; }
        /// <summary>
        /// Gets a flag indicating if this user is a guess.
        /// </summary>
        /// <remarks>A user without password is a guess</remarks>
        bool IsGuess { get; }
        /// <summary>
        /// Gets or sets the last login date for this user.
        /// </summary>
        DateTime LastLoginDate { get; set; }
    }
}