using System;

namespace ChatLe.Models
{
    public class Message : Message<string>
    {
        public Message()
        {
            Id = Guid.NewGuid().ToString("N");
        }
    }
    public class Message<TKey>
    {
        public virtual TKey Id { get; set; }
        public virtual TKey ConversationId { get; set; }
        public virtual TKey UserId { get; set; }
        public virtual string Text { get; set; }
        public virtual DateTime Date { get; set; }
    }
}