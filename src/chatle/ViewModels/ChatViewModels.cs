using ChatLe.Models;
using System;
using System.Collections.Generic;

namespace ChatLe.ViewModels
{
    /// <summary>
    /// View model for conversation
    /// </summary>
    public class ConversationViewModel
    {
        /// <summary>
        /// Gets or sets the conversation id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Gets or sets the attendees list
        /// </summary>
        public IEnumerable<AttendeeViewModel> Attendees { get; set; }
        /// <summary>
        /// Gets or sets the messages list
        /// </summary>
        public IEnumerable<MessageViewModel> Messages { get; set; }
    }
    /// <summary>
    /// View model for attendee
    /// </summary>
    public class AttendeeViewModel
    {
        /// <summary>
        /// Gets or sets the user id
        /// </summary>
        public string UserId { get; set; }        
    }
    /// <summary>
    /// View model for message
    /// </summary>
    public class MessageViewModel
    {
        public string ConversationId { get; set; }
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

    public class UserCollectionViewModel
    {
        private readonly Page<ChatLeUser> _model;
        public UserCollectionViewModel(Page<ChatLeUser> model)
        {
            _model = model;
        }

        public int PageCount
        {
            get
            {
                return _model.PageCount;
            }
        }

        public int PageIndex
        {
            get
            {
                return _model.PageIndex;
            }
        }

        public IEnumerable<UserViewModel> Users
        {
            get
            {
                foreach (var user in _model)
                    yield return new UserViewModel() { Id = user.UserName };
            }
        }
    }

    public class UserViewModel
    {
        public string Id { get; set; }
    }

    public class MessageToSend
    {
        public string To { get; set; }
        public string Text { get; set; }
    }
}