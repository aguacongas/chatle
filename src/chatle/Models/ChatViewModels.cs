using System;

namespace chatle.Models
{
    /// <summary>
    /// view model for attendee
    /// </summary>
    public class AttendeeViewModel
    {
        /// <summary>
        /// Gets or sets the user id
        /// </summary>
        public string UserId { get; set; }        
    }
    /// <summary>
    /// view model for message
    /// </summary>
    public class MessageViewModel
    {
        /// <summary>
        /// Gets or sets the sender user name
        /// </summary>
        public string From { get; set; }
        /// <summary>
        /// Gets or sets the message content
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// Gets or sets the message date
        /// </summary>
        public DateTime Date { get; set; }
    }
}