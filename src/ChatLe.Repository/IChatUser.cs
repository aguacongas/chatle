using System;

namespace ChatLe.Models
{
    public interface IChatUser<TKey> : IIdentifiable<TKey>where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Gets or sets the user name for this user.
        /// </summary>
        string UserName { get; set; }

        /// <summary>
        /// Gets or sets the last login date for this user.
        /// </summary>
        DateTime LastLoginDate { get; set; }

        /// <summary>
        /// Gets or sets a flag indecating that user is guess.
        /// </summary>
        bool IsGuess { get; set; }
    }
}