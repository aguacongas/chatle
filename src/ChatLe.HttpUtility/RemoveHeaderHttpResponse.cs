using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.Framework.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;

namespace ChatLe.HttpUtility
{
    /// <summary>
    /// <see cref="HttpResponse"/> decorator to remove unwanted HTTP response header added by other middleware
    /// </summary>
    public class RemoveHeaderHttpResponse : HttpResponse
    {
        readonly HttpResponse _parent;
        readonly HttpContext _context;
        readonly IHeaderDictionary _headers;
        /// <summary>
        /// Create instance of <see cref="RemoveHeaderHttpResponse"/>
        /// </summary>
        /// <param name="context">the <see cref="HttpContext"/> associated to the HTTP respone</param>
        /// <param name="parent">the <see cref="HttpResponse"/> to decorate</param>
        /// <param name="headersToRemove">a list of unwanted header</param>
        /// <param name="loggerFactory">the logger factory to create logger</param>
        public RemoveHeaderHttpResponse(HttpContext context, HttpResponse parent, IEnumerable<string> headersToRemove, ILoggerFactory loggerFactory)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");
            if (headersToRemove == null)
                throw new ArgumentNullException("headersToRemove");
            if (loggerFactory == null)
                throw new ArgumentNullException("loggerFactory");

            _parent = parent;
            _context = context;
            _headers = new RemoveHeaderHeaderDictionary(parent.Headers, headersToRemove, loggerFactory);
        }

        public override IHeaderDictionary Headers
        {
            get
            {
                return _headers;
            }
        }

        public override Stream Body
        {
            get
            {
                return _parent.Body;
            }

            set
            {
                _parent.Body = value;
            }
        }

        public override long? ContentLength
        {
            get
            {
                return _parent.ContentLength;
            }

            set
            {
                _parent.ContentLength = value;
            }
        }

        public override string ContentType
        {
            get
            {
                return _parent.ContentType;
            }

            set
            {
                _parent.ContentType = value;
            }
        }

        public override IResponseCookies Cookies
        {
            get
            {
                return _parent.Cookies;
            }
        }

        public override HttpContext HttpContext
        {
            get
            {
                return _context;
            }
        }

        public override int StatusCode
        {
            get
            {
                return _parent.StatusCode;
            }

            set
            {
                _parent.StatusCode = value;
            }
        }

        public override bool HeadersSent
        {
            get
            {
                return _parent.HeadersSent;
            }
        }

        public override void Challenge(AuthenticationProperties properties, IEnumerable<string> authenticationTypes)
        {
            _parent.Challenge(properties, authenticationTypes);
        }

        public override void OnSendingHeaders(Action<object> callback, object state)
        {
            _parent.OnSendingHeaders(callback, state);
        }

        public override void Redirect(string location, bool permanent)
        {
            _parent.Redirect(location, permanent);
        }

        public override void SignIn(string authenticationScheme, ClaimsPrincipal principal, AuthenticationProperties properties = null)
        {
            _parent.SignIn(authenticationScheme, principal, properties);
        }

        public override void SignOut(string authenticationScheme, AuthenticationProperties properties)
        {
            _parent.SignOut(authenticationScheme, properties);
        }

        public override void SignOut(string authenticationScheme)
        {
            _parent.SignOut(authenticationScheme);
        }

        public override void OnResponseCompleted(Action<object> callback, object state)
        {
            _parent.OnResponseCompleted(callback, state);
        }
    }
}