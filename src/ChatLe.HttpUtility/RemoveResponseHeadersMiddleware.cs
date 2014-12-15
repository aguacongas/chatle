using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using System.Collections.Generic;
using System;
using Microsoft.Framework.OptionsModel;
using System.Threading.Tasks;

namespace ChatLe.HttpUtility
{
    public class RemoveResponseHeadersMiddleware
    {
        IEnumerable<string> _headersToRemove;
        RequestDelegate _next;
        public RemoveResponseHeadersMiddleware(RequestDelegate next, IOptions<RemoveResponseHeardersOptions> optionsAccessor)
        {
            if (next == null)
                throw new ArgumentNullException("next");
            if (optionsAccessor == null || optionsAccessor.Options == null)
                throw new ArgumentNullException("optionsAccessor");
            _next = next;
            _headersToRemove = optionsAccessor.Options.Headers;
        }

        public async Task Invoke(HttpContext context)
        {
            var hearder = context.Response.Headers;
            foreach (var header in _headersToRemove)
                hearder.Remove(header);

            await _next.Invoke(context);
        }
    }
}