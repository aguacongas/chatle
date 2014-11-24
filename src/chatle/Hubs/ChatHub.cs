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
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            Trace.TraceInformation("[ChatHub] constructor");
            _provider = provider;
        }

        public override async Task OnConnected()
        {
            string name = Context.User.Identity.Name;
            Trace.TraceInformation("[ChatHub] OnConneect " + name);
            if (await SetConnectionStatus(true, name))
            {
                await Groups.Add(this.Context.ConnectionId, name);
                Clients.Others.userConnected(name);
            }
            await base.OnConnected();
        }

        public override async Task OnReconnected()
        {
            string name = Context.User.Identity.Name;
            Trace.TraceInformation("[ChatHub] OnReconnected " + name);
            if (await SetConnectionStatus(true, name))
            {
                await Groups.Add(this.Context.ConnectionId, name);
                Clients.Others.userConnected(name);
            }
            await base.OnReconnected();
        }

        public override async Task OnDisconnected(bool stopCalled)
        {
            string name = Context.User.Identity.Name;
            Trace.TraceInformation("[ChatHub] OnDisconnected " + name);
            if (await SetConnectionStatus(false, name))
            {
                await Groups.Remove(this.Context.ConnectionId, name);
                Clients.Others.userDisconnected(name);
            }
            await base.OnDisconnected(stopCalled);
        }

        private async Task<bool> SetConnectionStatus(bool isConnected, string name)
        {            
            var manager = _provider.GetService(typeof(IChatManager<string, ApplicationUser>)) as IChatManager<string, ApplicationUser>;
            Trace.TraceInformation("[ChatHub] SetConnectionStatus {0} {1} {2}", name, Context.ConnectionId, isConnected);
            return await manager.SetConnectionStatusAsync(name, Context.ConnectionId, isConnected);
        }
    }
}