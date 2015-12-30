using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;

namespace ChatLe.Models
{
    /// <summary>
    /// Default <see cref="ChatManager<TKey, TUser, TConversation, TAttendee, TMessage, TNotificationConnection>"/>
    /// </summary>
    public class ChatManager : ChatManager<ChatLeUser>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="store">a chat store</param>
        public ChatManager(IChatStore<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection> store, IOptions<ChatOptions> options, ILoggerFactory loggerFactory = null) 
            : base(store, options, loggerFactory)
        { }
    }

    public class ChatManager<TUser> : ChatManager<string, TUser, Conversation, Attendee, Message, NotificationConnection>
        where TUser : IChatUser<string>
    {
        public ChatManager(IChatStore<string, TUser, Conversation, Attendee, Message, NotificationConnection> store, IOptions<ChatOptions> options, ILoggerFactory loggerFactory = null) 
            : base(store, options, loggerFactory)
        { }
    }
}