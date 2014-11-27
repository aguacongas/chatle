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
    [HubName("chat")]
    public class ChatHub : Hub
    {
        public ChatManager Manager
        {
            get;
            private set;
        }

        public ChatHub(ChatManager manager) :base()
        {
            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }
            Trace.TraceInformation("[ChatHub] constructor");
            Manager = manager;
        }

        public override async Task OnConnected()
        {
            string name = Context.User.Identity.Name;
            Trace.TraceInformation("[ChatHub] OnConneect " + name);
            await Manager.AddConnectionIdAsync(name, Context.ConnectionId);            
            await Groups.Add(this.Context.ConnectionId, name);
            Clients.Others.userConnected(name);
            await base.OnConnected();
        }

        public override async Task OnReconnected()
        {
            string name = Context.User.Identity.Name;
            Trace.TraceInformation("[ChatHub] OnReconnected " + name);
            await Manager.AddConnectionIdAsync(name, Context.ConnectionId);
            await Groups.Add(this.Context.ConnectionId, name);
            Clients.Others.userConnected(name);
            await base.OnReconnected();
        }

        public override async Task OnDisconnected(bool stopCalled)
        {
            string name = Context.User.Identity.Name;
            Trace.TraceInformation("[ChatHub] OnDisconnected " + name);
            await Manager.RemoveConnectionIdAsync(name, Context.ConnectionId);
            await Groups.Remove(Context.ConnectionId, name);
            Clients.Others.userDisconnected(name);
            await base.OnDisconnected(stopCalled);
        }
    }
}