using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Diagnostics;
using ChatLe.Models;
using Microsoft.Framework.Logging;

namespace ChatLe.Hubs
{
    /// <summary>
    /// Chat Hub
    /// </summary>
    [HubName("chat")]
    public class ChatHub : Hub
    {
        /// <summary>
        /// The chat repository manager
        /// </summary>
        public IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection> Manager
        {
            get;
            private set;
        }
        /// <summary>
        /// Logger
        /// </summary>
        public ILogger Logger { get; private set; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="manager">The chat repository manager</param>
        public ChatHub(IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection> manager, ILoggerFactory loggerFactory) :base()
        {
            if (manager == null)
                throw new ArgumentNullException("manager");
            if (loggerFactory == null)
                throw new ArgumentNullException("loggerFactory");

            Logger = loggerFactory.Create<ChatHub>();
            Logger.WriteInformation("constructor");
            Manager = manager;
        }
        /// <summary>
        /// Called when the connection connects to this hub instance.
        /// <para>Create a signalR group for the connected user with is name</para>
        /// </summary>
        /// <returns>a <see cref="Task"/></returns>
        public override async Task OnConnected()
        {
            string name = Context.User.Identity.Name;
            Logger.WriteInformation("OnConnected " + name);
            await Manager.AddConnectionIdAsync(name, Context.ConnectionId, "signalR");            
            await Groups.Add(this.Context.ConnectionId, name);
            Clients.Others.userConnected(new { Id = name });
            await base.OnConnected();
        }
        /// <summary>
        /// Called when the connection reconnects to this hub instance.
        /// <para>Create a signalR group for the connected user with is name</para>
        /// </summary>
        /// <returns>a <see cref="Task"/></returns>
        public override async Task OnReconnected()
        {
            string name = Context.User.Identity.Name;
            Logger.WriteInformation("OnReconnected " + name);
            await Manager.AddConnectionIdAsync(name, Context.ConnectionId, "signalR");
            await Groups.Add(this.Context.ConnectionId, name);
            Clients.Others.userConnected(new { Id = name });
            await base.OnReconnected();
        }
        /// <summary>
        /// Called when a connection disconnects from this hub gracefully or due to a timeout.
        /// <para>Remove the signalR group for the user</para>
        /// </summary>
        /// <param name="stopCalled">true, if stop was called on the client closing the connection gracefully; false,
        /// <para>if the connection has been lost for longer than the Configuration.IConfigurationManager.DisconnectTimeout.</para>
        /// <para>Timeouts can be caused by clients reconnecting to another SignalR server in scaleout.</para>
        /// </param>
        /// <returns>a <see cref="Task"/></returns>
        public override async Task OnDisconnected(bool stopCalled)
        {
            string name = Context.User.Identity.Name;
            Logger.WriteInformation("OnDisconnected " + name);
            if (!await Manager.RemoveConnectionIdAsync(name, Context.ConnectionId, "signalR"))
                Clients.Others.userDisconnected(name);
            await base.OnDisconnected(stopCalled);
        }
    }
}