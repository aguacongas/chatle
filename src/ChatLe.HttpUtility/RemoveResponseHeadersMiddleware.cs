using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.HttpFeature;
using Microsoft.Framework.OptionsModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections;
using System.Linq;
using System.IO;
using Microsoft.AspNet.Http.Security;
using System.Security.Claims;
using System.Net.WebSockets;
using System.Threading;
using Microsoft.Framework.Logging;

namespace ChatLe.HttpUtility
{
    /// <summary>
    /// Middleware to remove HTTP response headers
    /// </summary>
    public class RemoveResponseHeadersMiddleware
    {
        readonly IEnumerable<string> _headersToRemove;
        readonly RequestDelegate _next;
        readonly ILoggerFactory _loggerFactory;
        /// <summary>
        /// Create an instance of <see cref="RemoveResponseHeadersMiddleware"/>
        /// </summary>
        /// <param name="next">the next <see cref="RequestDelegate"/> to call in the pipeline</param>
        /// <param name="optionsAccessor">the accessor to <see cref="CommaSeparatedListOptions"/> where headers to remove are configured</param>
        /// <param name="loggerFactory">the logger factory to create logger</param>
        public RemoveResponseHeadersMiddleware(RequestDelegate next, IOptions<CommaSeparatedListOptions> optionsAccessor, ILoggerFactory loggerFactory)
        {
            if (next == null)
                throw new ArgumentNullException("next");
            if (optionsAccessor == null || optionsAccessor.Options == null || optionsAccessor.Options.List == null)
                throw new ArgumentNullException("optionsAccessor");
            if (loggerFactory == null)
                throw new ArgumentNullException("loggerFactory");

            _next = next;
            _headersToRemove = optionsAccessor.Options.List;
            _loggerFactory = loggerFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next.Invoke(new RemoveHeaderHttpContext(context, _headersToRemove, _loggerFactory));            
        }
    }
}