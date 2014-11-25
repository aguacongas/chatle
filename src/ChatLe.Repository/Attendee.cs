using System;

namespace ChatLe.Models
{
    public class Attendee:Attendee<string>
    { 
    }
    public class Attendee<TKey>
    {        
        public TKey ConversationId { get; set; }

        public TKey UserId { get; set; }
    }
}