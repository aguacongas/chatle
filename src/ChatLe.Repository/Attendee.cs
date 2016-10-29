﻿using System;

namespace ChatLe.Models
{
    public class Attendee:Attendee<string>
    { 
    }

    public class Attendee<TKey> where TKey : IEquatable<TKey>
    {
        public virtual TKey ConversationId { get; set; }
        public virtual TKey UserId { get; set; }
        public virtual bool IsConnected { get; set; } = true;

        public virtual Conversation<TKey> Conversation { get; set; }
        
    }   
}