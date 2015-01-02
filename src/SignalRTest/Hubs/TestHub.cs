using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Threading.Tasks;

namespace SignalRTest.Hubs
{
    [HubName("test")]
    public class TestHub : Hub
    {
        public override async Task OnConnected()
        {
            Clients.Others.userConnected();
            await base.OnConnected();
        }
        public override async Task OnReconnected()
        {
            Clients.Others.userRonnected();
            await base.OnReconnected();
        }
        public override async Task OnDisconnected(bool stopCalled)
        {
            Clients.Others.userDisconnected();
            await base.OnDisconnected(stopCalled);
        }

    }
}