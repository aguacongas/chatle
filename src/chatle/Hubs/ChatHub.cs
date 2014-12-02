using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Diagnostics;
using ChatLe.Models;

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
        /// Constructor
        /// </summary>
        /// <param name="manager">The chat repository manager</param>
        public ChatHub(IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection> manager) :base()
        {
            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }
            Trace.TraceInformation("[ChatHub] constructor");
            Manager = manager;
        }
        /// <summary>
        /// Called when the connection connects to this hub instance.
        /// <para>Create a signalR group for the connected user with is name</para>
        /// </summary>
        /// <returns>an async task</returns>
        public override async Task OnConnected()
        {
            string name = Context.User.Identity.Name;
            Trace.TraceInformation("[ChatHub] OnConnected " + name);
            await Manager.AddConnectionIdAsync(name, Context.ConnectionId, "signalR");            
            await Groups.Add(this.Context.ConnectionId, name);
            Clients.Others.userConnected(new { Id = name });
            await base.OnConnected();
        }
        /// <summary>
        /// Called when the connection reconnects to this hub instance.
        /// <para>Create a signalR group for the connected user with is name</para>
        /// </summary>
        /// <returns>an async task</returns>
        public override async Task OnReconnected()
        {
            string name = Context.User.Identity.Name;
            Trace.TraceInformation("[ChatHub] OnReconnected " + name);
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
        /// <returns></returns>
        public override async Task OnDisconnected(bool stopCalled)
        {
            string name = Context.User.Identity.Name;
            Trace.TraceInformation("[ChatHub] OnDisconnected " + name);
            await Manager.RemoveConnectionIdAsync(name, Context.ConnectionId, "signalR");
            await Groups.Remove(Context.ConnectionId, name);
            Clients.Others.userDisconnected(new { Id = name });
            await base.OnDisconnected(stopCalled);
        }
    }
}