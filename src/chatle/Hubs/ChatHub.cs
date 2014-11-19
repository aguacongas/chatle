using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Collections.Generic;
using System.Linq;
using System;
using Chat.CMACGM.Models;
using System.Diagnostics;

namespace Chat.CMACGM.Hubs
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
            var dbContext = _provider.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;
            Trace.TraceInformation("[ChatHub] SetConnectionStatus {0} {1} {2}", name, Context.ConnectionId, isConnected);
            return dbContext.SetConnectionStatus(name, Context.ConnectionId, isConnected);
        }

        public bool PostMessage(Message message)
        {
            Trace.TraceInformation("[ChatHub] SendMsg {0} {1}", message.To, message.Text);
            message.From = Context.User.Identity.Name;
            message.Date = DateTime.UtcNow;
            Clients.Group(message.To).messageReceived(message);
            var dbContext = _provider.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;
            dbContext.Messages.Add(message);
            return dbContext.SaveChanges() > 0;
        }
    }
}