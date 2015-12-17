using System;
using System.Threading;
using ChatLe.Models;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

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
        public Startup(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .AddJsonFile($"config.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
                loggerFactory.AddDebug();
                loggerFactory.MinimumLevel = LogLevel.Debug;
            }

            builder.AddEnvironmentVariables();

            Configuration = builder.Build();

            _environment = env;
            LoggerFactory = loggerFactory;

            loggerFactory.AddConsole();
            
#if DNX451
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

            var sourceSwitch = new SourceSwitch("chatle");
            loggerFactory.AddTraceSource(sourceSwitch, new ConsoleTraceListener());
#endif
        }

        public IConfiguration Configuration { get; private set; }

        public virtual void ConfigureServices(IServiceCollection services)
        {

            ConfigureEntity(services);

            services.AddMvc();

            services.AddSignalR(options => options.Hubs.EnableDetailedErrors = _environment.EnvironmentName == "Development");

            services.AddChatLe(Configuration);

        }

        private void ConfigureEntity(IServiceCollection services)
        {
            var builder = services.AddEntityFramework();

            var dbEngine = (DBEngine)Enum.Parse(typeof(DBEngine), Configuration["DatabaseEngine"]);
            switch (dbEngine)
            {
                case DBEngine.InMemory:
                    builder.AddInMemoryDatabase();
                    break;
                //case DBEngine.SQLite:
                //    builder.AddSQLite();
                //    break;
                case DBEngine.SqlServer:
                    builder.AddSqlServer();
                    break;
                //case DBEngine.Redis:
                //    //builder.AddRedis();
                //    break;
                default:
                    throw new InvalidOperationException("Database engine unsupported");
            }

            builder.AddDbContext<ChatLeIdentityDbContext>(options =>
            {
                switch (dbEngine)
                {
                    case DBEngine.InMemory:
                        options.UseInMemoryDatabase();
                        break;
                    case DBEngine.SqlServer:
                        options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]);
                        break;
                    //case DBEngine.SQLite:
                    //    options.UseSQLite(Configuration.Get("Data:DefaultConnection:ConnectionString"));
                    //    break;
                    //case DBEngine.Redis:
                    //    int port;
                    //    int database;
                    //    if (!int.TryParse(Configuration.Get("Data:Redis:Port"), out port))
                    //        port = 6379;
                    //    int.TryParse(Configuration.Get("Data:Redis:Database"), out database);
                        
                    //    //options.UseRedis(Configuration.Get("Data:Redis:Hostname"), port, database);
                    //    break;
                }
            });

            services.AddIdentity<ChatLeUser, IdentityRole>(options =>
            {
                options.SecurityStampValidationInterval = TimeSpan.FromMinutes(20);
            }).AddEntityFrameworkStores<ChatLeIdentityDbContext>();
        }

        public virtual void Configure(IApplicationBuilder app)
        {
            ConfigureErrors(app);

            app.UseStaticFiles()             
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
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Add Error handling middleware which catches all application specific errors and
                // send the request to the following path or controller action.
                app.UseExceptionHandler("/Home/Error");
            }            
        }
    }
}
