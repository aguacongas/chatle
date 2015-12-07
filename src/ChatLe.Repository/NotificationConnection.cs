using System;

namespace ChatLe.Models
{
    public class NotificationConnection :NotificationConnection<string>
    {
    }

    public class NotificationConnection<TKey> where TKey : IEquatable<TKey>
    {        
        public virtual string ConnectionId { get; set; }
        public virtual string NotificationType { get; set; }
        public virtual TKey UserId { get; set; }
        public virtual DateTime ConnectionDate { get; set; }
    }
}