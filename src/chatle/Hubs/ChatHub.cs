using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Collections.Generic;
using System.Linq;
using System;
using ChatLe.Models;
using System.Diagnostics;

namespace ChatLe.Hubs
{
    [HubName("chat")]
    public class ChatHub : Hub
    {
        IServiceProvider _provider;
        public ChatHub(IServiceProvider provider) :base()
        {
            _provider = provider;
        }

        public override Task OnConnected()
        {
            string name;
            if (SetConnectionStatus(true, out name))
            {
                Groups.Add(this.Context.ConnectionId, name);
                Clients.Others.userConnected(name);
            }
            return base.OnConnected();
        }

        public override Task OnReconnected()
        {
            string name;
            if (SetConnectionStatus(true, out name))
            {
                Groups.Add(this.Context.ConnectionId, name);
                Clients.Others.userConnected(name);
            }
            return base.OnReconnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string name;
            if (SetConnectionStatus(false, out name))
            {
                Groups.Remove(this.Context.ConnectionId, name);
                Clients.Others.userDisconnected(name);
            }
            return base.OnDisconnected(stopCalled);
        }

        private bool SetConnectionStatus(bool isConnected, out string name)
        {
            name = Context.User.Identity.Name;
            var manager = _provider.GetService(typeof(ChatManager<ApplicationUser>)) as ChatManager<ApplicationUser>;
            Trace.TraceInformation("[ChatHub] SetConnectionStatus {0} {1} {2}", name, Context.ConnectionId, isConnected);
            var task= manager.SetConnectionStatusAsync(name, Context.ConnectionId, isConnected);
            task.Wait();
            return task.Result;
        }
    }
}