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
    public class Conversation<TKey>
    {
        public virtual TKey Id { get; set; }
        public virtual ICollection<IApplicationUser<TKey>> Users { get; } = new List<IApplicationUser<TKey>>();
        public virtual ICollection<Message<TKey>> Messages { get; } = new List<Message<TKey>>();
    }
}