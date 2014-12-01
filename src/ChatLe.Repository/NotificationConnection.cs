using System;

namespace ChatLe.Models
{
    public class NotificationConnection :NotificationConnection<string>
    {
    }
    public class NotificationConnection<TKey>
    {
        public TKey UserId { get; set; }
        public string ConnectionId { get; set; }
        public string NotificationType { get; set; }
    }
}