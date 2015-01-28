using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

namespace Bench
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                var payload = "Bench!";
                context.Response.ContentLength = payload.Length;
                await context.Response.WriteAsync(payload);
            });
        }
    }
}
