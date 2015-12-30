using System;

namespace ChatLe.ViewModels
{
    public class Attendee:Attendee<string>
    { 
    }

    public class Attendee<TKey> where TKey : IEquatable<TKey>
    {
        public virtual TKey ConversationId { get; set; }
        public virtual TKey UserId { get; set; }

        public virtual Conversation<TKey> Conversation { get; set; }
        
    }   
}