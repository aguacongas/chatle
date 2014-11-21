using System;

namespace ChatLe.Models
{
    public class Message
    {
        public string To { get; set; }
        public string From { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
    }
}