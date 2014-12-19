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

namespace ChatLe.HttpUtility
{
    public class RemoveResponseHeadersMiddleware
    {
        readonly IEnumerable<string> _headersToRemove;
        readonly RequestDelegate _next;
        public RemoveResponseHeadersMiddleware(RequestDelegate next, IOptions<RemoveResponseHeardersOptions> optionsAccessor)
        {
            Trace.TraceInformation("[RemoveResponseHeadersMiddleware] constructor");
            if (next == null)
                throw new ArgumentNullException("next");
            if (optionsAccessor == null || optionsAccessor.Options == null)
                throw new ArgumentNullException("optionsAccessor");
            _next = next;
            _headersToRemove = optionsAccessor.Options.Headers;
        }

        public async Task Invoke(HttpContext context)
        {
            Trace.TraceInformation("[RemoveResponseHeadersMiddleware] Invoke " + context.Request.Path);
            await _next.Invoke(new RemoveHeaderHttpContext(context, _headersToRemove));            
        }
    }
}