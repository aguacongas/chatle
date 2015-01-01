using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;

namespace SignalRTest.Hubs
{
    [HubName("test")]
    public class TestHub : Hub
    {

    }
}