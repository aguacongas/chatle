using System;
using System.Collections.Generic;

namespace ChatLe.Models
{
    public class Conversation : Conversation<string>
    {
        public Conversation()
        {
            Id = Guid.NewGuid().ToString("N");
        }
    }
    public class Conversation<TKey> where TKey : IEquatable<TKey>
    {
        public virtual TKey Id { get; set; }
        public virtual ICollection<Attendee<TKey>> Attendees { get; set; } = new List<Attendee<TKey>>();
        public virtual ICollection<Message<TKey>> Messages { get; set;  } = new List<Message<TKey>>();
    }
}