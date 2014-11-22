using System;

namespace ChatLe.Models
{
    public class Message
    {
        public virtual string ConversationId { get; set; }
        public virtual string From { get; set; }
        public virtual string Text { get; set; }
        public virtual DateTime Date { get; set; }
    }
}