using System;

namespace chatle.Models
{
    public class AttendeeViewModel
    {
        public string UserId { get; set; }        
    }
    public class MessageViewModel
    {
        public string From { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
    }
}