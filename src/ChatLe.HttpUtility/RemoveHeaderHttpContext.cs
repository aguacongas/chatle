using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ChatLe.HttpUtility
{
    /// <summary>
    /// <see cref="HttpContext"/> decorator to remove unwanted HTTP response header added by other middleware
    /// </summary>
    public class RemoveHeaderHttpContext : HttpContext
    {
        readonly HttpContext _parent;
        readonly HttpResponse _response;
        /// <summary>
        /// Create instance of <see cref="RemoveHeaderHttpContext"/>
        /// </summary>
        /// <param name="parent">the <see cref="HttpContext"/> to decorate/></param>
        /// <param name="headersToRemove">a list of unwanted header</param>
        /// <param name="loggerFactory">the logger factory to create logger</param>
        public RemoveHeaderHttpContext(HttpContext parent, IEnumerable<string> headersToRemove, ILoggerFactory loggerFactory)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");
            if (headersToRemove == null)
                throw new ArgumentNullException("headersToRemove");
            if (loggerFactory == null)
                throw new ArgumentNullException("loggerFactory");

            _parent = parent;
            _response = new RemoveHeaderHttpResponse(this, parent.Response, headersToRemove, loggerFactory);
        }

        public override HttpResponse Response
        {
            get
            {
                return _response;
            }
        }

        public override IServiceProvider ApplicationServices
        {
            get
            {
                return _parent.ApplicationServices;
            }

            set
            {
                _parent.ApplicationServices = value;
            }
        }

        public override bool IsWebSocketRequest
        {
            get
            {
                return _parent.IsWebSocketRequest;
            }
        }

        public override IDictionary<object, object> Items
        {
            get
            {
                return _parent.Items;
            }
        }

        public override HttpRequest Request
        {
            get
            {
                return _parent.Request;
            }
        }

        public override CancellationToken RequestAborted
        {
            get
            {
                return _parent.RequestAborted;
            }
        }

        public override IServiceProvider RequestServices
        {
            get
            {
                return _parent.RequestServices;
            }

            set
            {
                _parent.RequestServices = value;
            }
        }

        public override ISessionCollection Session
        {
            get
            {
                return _parent.Session;
            }
        }

        public override ClaimsPrincipal User
        {
            get
            {
                return _parent.User;
            }

            set
            {
                _parent.User = value;
            }
        }

        public override IList<string> WebSocketRequestedProtocols
        {
            get
            {
                return _parent.WebSocketRequestedProtocols;
            }
        }

        public override void Abort()
        {
            _parent.Abort();
        }

        public override Task<WebSocket> AcceptWebSocketAsync(string subProtocol)
        {
            return _parent.AcceptWebSocketAsync(subProtocol);
        }

        public override AuthenticationResult Authenticate(string authenticationScheme)
        {
            return _parent.Authenticate(authenticationScheme);
        }

        public override Task<AuthenticationResult> AuthenticateAsync(string authenticationScheme)
        {
            return _parent.AuthenticateAsync(authenticationScheme);
        }

        public override void Dispose()
        {
            _parent.Dispose();
        }

        public override IEnumerable<AuthenticationDescription> GetAuthenticationSchemes()
        {
            return _parent.GetAuthenticationSchemes();
        }

        public override object GetFeature(Type type)
        {
            return _parent.GetFeature(type);
        }

        public override void SetFeature(Type type, object instance)
        {
            _parent.SetFeature(type, instance);
        }        
    }
}