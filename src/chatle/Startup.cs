using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Routing;
using Microsoft.Data.Entity;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using ChatLe.Models;
using Microsoft.Data.Entity.Redis.Extensions;
using ChatLe.HttpUtility;
using Microsoft.Framework.Logging;
using Microsoft.Framework.Logging.Console;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Diagnostics;
using System.Net;

namespace ChatLe
{
    public class Startup
    {
        enum DBEngine
        {
            SqlServer,
            InMemory,
            Redis,
            SQLite
        }

        public Startup(ILoggerFactory factory)
        {
            //factory.AddConsole();
            /* 
            * Below code demonstrates usage of multiple configuration sources. For instance a setting say 'setting1' is found in both the registered sources, 
            * then the later source will win. By this way a Local config can be overridden by a different setting while deployed remotely.
            */
            Configuration = new Configuration()
                .AddJsonFile("config.json")
                .AddEnvironmentVariables(); //All environment variables in the process's context flow in as configuration values.
        }

        public IConfiguration Configuration { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureEntity(services);

            services.AddMvc();

            services.AddSignalR(options => options.Hubs.EnableDetailedErrors = true);

            services.AddChatLe(Configuration.GetSubKey("ChatCongig"));

            services.AddRemoveResponseHeaders(Configuration.GetSubKey("RemoveResponseHeader"));
        }

        private void ConfigureEntity(IServiceCollection services)
        {
            var builder = services.AddEntityFramework();

            var dbEngine = (DBEngine)Enum.Parse(typeof(DBEngine), Configuration.Get("DatabaseEngine"));
            switch (dbEngine)
            {
                case DBEngine.InMemory:
                    builder.AddInMemoryStore();
                    break;
                case DBEngine.SQLite:
                    builder.AddSQLite();
                    break;
                case DBEngine.SqlServer:
                    builder.AddSqlServer();
                    break;
                case DBEngine.Redis:
                    builder.AddRedis();
                    break;
                default:
                    throw new InvalidOperationException("Database engine unsupported");
            }

            builder.AddDbContext<ChatLeIdentityDbContext>(options =>
            {
                switch (dbEngine)
                {
                    case DBEngine.InMemory:
                        options.UseInMemoryStore(true);
                        break;
                    case DBEngine.SqlServer:
                        options.UseSqlServer(Configuration.Get("Data:DefaultConnection:ConnectionString"));
                        break;
                    case DBEngine.SQLite:
                        options.UseSQLite(Configuration.Get("Data:DefaultConnection:ConnectionString"));
                        break;
                    case DBEngine.Redis:
                        int port;
                        int database;
                        if (!int.TryParse(Configuration.Get("Data:Redis:Port"), out port))
                            port = 6379;
                        int.TryParse(Configuration.Get("Data: Redis:Database"), out database);

                        options.UseRedis(Configuration.Get("Data:Redis:Hostname"), port, database);
                        break;
                }
            });

            services.AddDefaultIdentity<ChatLeIdentityDbContext, ChatLeUser, IdentityRole>(Configuration.GetSubKey("Identity"), options =>
            {
                options.SecurityStampValidationInterval = TimeSpan.FromMinutes(20);
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRemoveResponseHeaders();
            if (Configuration.Get("KRE_ENV") == "Production")
            {
                app.UseBrowserLink()
                    .UseErrorPage();
            }
            else
            {
                app.UseErrorHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "text/html";
                        await context.Response.WriteAsync("<html><body>");
                        await context.Response.WriteAsync("We're sorry, we encountered an un-expected issue with your application.<br>");

                        var error = context.GetFeature<IErrorHandlerFeature>();
                        if (error != null)
                        {
                            // This error would not normally be exposed to the client
                            await context.Response.WriteAsync("<br>Error: " + WebUtility.HtmlEncode(error.Error.Message) + "<br>");
                        }
                        await context.Response.WriteAsync("<br><a href=\"/\">Home</a><br>");
                        await context.Response.WriteAsync("</body></html>");
                        await context.Response.WriteAsync(new string(' ', 512)); // Padding for IE
                    });
                });
            }
                app.UseStaticFiles()
                .UseIdentity()
                .UseMvc(routes =>
                {
                    routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });
                })
                .UseSignalR()
                .UseChatLe();
        }
    }
}
