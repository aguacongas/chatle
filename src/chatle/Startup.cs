using ChatLe.HttpUtility;
using ChatLe.Logging;
using ChatLe.Models;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Mvc.Routing;
using Microsoft.AspNet.Routing;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Redis.Extensions;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

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

        readonly IHostingEnvironment _environment;
        public ILoggerFactory LoggerFactory { get; private set; }
        public Startup(IHostingEnvironment environment, ILoggerFactory factory)
        {
            LoggerFactory = factory;
            LoggerFactory.AddProvider(new TraceLoggerProvider());
            /* 
            * Below code demonstrates usage of multiple configuration sources. For instance a setting say 'setting1' is found in both the registered sources, 
            * then the later source will win. By this way a Local config can be overridden by a different setting while deployed remotely.
            */
            Configuration = new Configuration()
                .AddJsonFile("config.json")
                .AddEnvironmentVariables(); //All environment variables in the process's context flow in as configuration values.

            _environment = environment;

            int io, worker;
            ThreadPool.GetMinThreads(out worker, out io);
            Console.WriteLine("Startup min worker thread {0}, min io thread {1}", worker, io);
            ThreadPool.GetMaxThreads(out worker, out io);
            Console.WriteLine("Startup max worker thread {0}, max io thread {1}", worker, io);
            ThreadPool.SetMaxThreads(32767, 1000);
            ThreadPool.SetMinThreads(50, 50);
            ThreadPool.GetMinThreads(out worker, out io);
            Console.WriteLine("Startup min worker thread {0}, min io thread {1}", worker, io);
            ThreadPool.GetMaxThreads(out worker, out io);
            Console.WriteLine("Startup max worker thread {0}, max io thread {1}", worker, io);
        }

        public IConfiguration Configuration { get; private set; }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            ConfigureEntity(services);

            services.AddMvc();

            services.AddSignalR(options => options.Hubs.EnableDetailedErrors = _environment.EnvironmentName == "Development");

            services.AddChatLe(Configuration.GetSubKey("ChatCongig"));

            services.AddRemoveResponseHeaders(Configuration.GetSubKey("RemoveResponseHeader"));

        }

        private void ConfigureEntity(IServiceCollection services)
        {
            var builder = services.AddEntityFramework(Configuration);

            var dbEngine = (DBEngine)Enum.Parse(typeof(DBEngine), Configuration.Get("DatabaseEngine"));
            switch (dbEngine)
            {
                case DBEngine.InMemory:
                    builder.AddInMemoryStore();
                    break;
                //case DBEngine.SQLite:
                //    builder.AddSQLite();
                //    break;
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
                    //case DBEngine.SQLite:
                    //    options.UseSQLite(Configuration.Get("Data:DefaultConnection:ConnectionString"));
                    //    break;
                    case DBEngine.Redis:
                        int port;
                        int database;
                        if (!int.TryParse(Configuration.Get("Data:Redis:Port"), out port))
                            port = 6379;
                        int.TryParse(Configuration.Get("Data:Redis:Database"), out database);

                        options.UseRedis(Configuration.Get("Data:Redis:Hostname"), port, database);
                        break;
                }
            });

            services.AddIdentity<ChatLeUser, IdentityRole>(Configuration.GetSubKey("Identity"), options =>
            {
                options.SecurityStampValidationInterval = TimeSpan.FromMinutes(20);
            })
            .AddEntityFrameworkStores<ChatLeIdentityDbContext>();
        }

        public virtual void Configure(IApplicationBuilder app)
        {
            //app.UseRemoveResponseHeaders();
            ConfigureErrors(app);

            app.UseRemoveResponseHeaders()
                .UseStaticFiles()             
                .UseWebSockets()
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

        protected virtual void ConfigureErrors(IApplicationBuilder app)
        {
            if (string.Equals(_environment.EnvironmentName, "Development", StringComparison.OrdinalIgnoreCase))
            {
                app.UseBrowserLink();
                app.UseErrorPage(ErrorPageOptions.ShowAll);
            }
            else
            {
                // Add Error handling middleware which catches all application specific errors and
                // send the request to the following path or controller action.
                app.UseErrorHandler("/Home/Error");
            }            
        }
    }
}
