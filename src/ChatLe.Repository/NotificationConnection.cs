using System;

namespace ChatLe.Models
{
    public class NotificationConnection :NotificationConnection<string>
    {
    }
    public class NotificationConnection<TKey>
    {
        public virtual TKey UserId { get; set; }
        public virtual string ConnectionId { get; set; }
        public virtual string NotificationType { get; set; }
        public virtual DateTime ConnectionDate { get; set; }
    }
}