﻿using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Framework.DependencyInjection;

namespace SignalRTest
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddSignalR(options =>
                {
                    options.Hubs.EnableDetailedErrors = true;
                });
        }
        public void Configure(IApplicationBuilder app)
        {
            app.UseSignalR()
                .UseStaticFiles()
                .UseMvc();
                
        }
    }
}
