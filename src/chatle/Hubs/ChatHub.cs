using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Hubs;
using Microsoft.Extensions.Logging;
using ChatLe.Models;

namespace ChatLe.Hubs
{
	/// <summary>
	/// Chat Hub
	/// </summary>
	[HubName("chat")]
	public class ChatHub : Hub
	{
		readonly IServiceProvider _provider;
		/// <summary>
		/// The chat repository manager
		/// </summary>
		public IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection> Manager
		{
			get
			{
				return _provider.GetService(typeof(IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>)) as IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>;
			}
		}

		/// <summary>
		/// Logger
		/// </summary>
		public ILogger Logger { get; private set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="manager">The chat repository manager</param>
		/// <param name="loggerFactory">The logger factory</param>
		public ChatHub(IServiceProvider provider,
			ILoggerFactory loggerFactory) : base()
		{
			if (provider == null)
				throw new ArgumentNullException("provider");
			if (loggerFactory == null)
				throw new ArgumentNullException("loggerFactory");

			Logger = loggerFactory.CreateLogger<ChatHub>();
			Logger.LogInformation("constructor");
			_provider = provider;
		}
		
        /// <summary>
		/// Called when the connection connects to this hub instance.
		/// <para>Create a signalR group for the connected user with is name</para>
		/// </summary>
		/// <returns>a <see cref="Task"/></returns>
		public override async Task OnConnected()
		{
			string name = Context.User.Identity.Name;
			Logger.LogInformation("OnConnected " + name);

			await Manager.AddConnectionIdAsync(name, Context.ConnectionId, "signalR");
			
			await Groups.Add(Context.ConnectionId, name);
			Clients.Others.userConnected(new { id = name });
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
			Logger.LogInformation("OnReconnected " + name);

			await Manager.AddConnectionIdAsync(name, Context.ConnectionId, "signalR");
				
			await Groups.Add(this.Context.ConnectionId, name);
			Clients.Others.userConnected(new { id = name });
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
			Logger.LogInformation("OnDisconnected stopCalled " + stopCalled);
			var user = await Manager.RemoveConnectionIdAsync(Context.ConnectionId, "signalR", stopCalled);
			if (user != null)
				Clients.Others.userDisconnected(new { id = user.UserName, isRemoved = user.IsGuess });
			await base.OnDisconnected(stopCalled);
		}
	}
}