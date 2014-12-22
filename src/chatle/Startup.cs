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
using Microsoft.AspNet.Hosting;
using System.Diagnostics;
using Microsoft.Framework.Runtime;
using Microsoft.AspNet.Mvc.Razor;
using System.IO;

namespace ChatLe
{
    public class StartupDevelopment : Startup
    {
        public StartupDevelopment(IHostingEnvironment environment, ILoggerFactory factory) : base(environment, factory)
        { }

        protected override void ConfigureErrors(IApplicationBuilder app)
        {
            LoggerFactory.AddConsole((name, type) => type < TraceType.Information);
            app.UseBrowserLink()
                .UseErrorPage();
        }

        public override void Configure(IApplicationBuilder app)
        {
            base.Configure(app);
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
        }
    }
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

            /* 
            * Below code demonstrates usage of multiple configuration sources. For instance a setting say 'setting1' is found in both the registered sources, 
            * then the later source will win. By this way a Local config can be overridden by a different setting while deployed remotely.
            */
            Configuration = new Configuration()
                .AddJsonFile("config.json")
                .AddEnvironmentVariables(); //All environment variables in the process's context flow in as configuration values.

            _environment = environment;
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

        public virtual void Configure(IApplicationBuilder app)
        {
            app.UseRemoveResponseHeaders();
            ConfigureErrors(app);

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

        protected virtual void ConfigureErrors(IApplicationBuilder app)
        {
            app.UseErrorHandler(errorApp =>
            {
                errorApp.UseServices(services =>
                {
                    services.AddMvc();
                    services.AddScoped<Controllers.HomeController>();
                });
                errorApp.Run(async context =>
                {
                    try
                    {
                        var errorController = errorApp.ApplicationServices.GetRequiredService<Controllers.HomeController>();
                        await errorController.Error().ExecuteResultAsync(errorController.ActionContext);
                    }
                    catch(Exception e)
                    {
                        Trace.TraceError(e.ToString());
                    }
                });
            });
        }

        class FileInfo : Microsoft.AspNet.FileSystems.IFileInfo
        {
            public FileInfo(string path)
            {
                PhysicalPath = path;
            }

            public bool IsDirectory
            {
                get
                {
                    return Directory.Exists(PhysicalPath);
                }
            }

            public DateTime LastModified
            {
                get
                {
                    return File.GetLastWriteTime(PhysicalPath);
                }
            }

            public long Length
            {
                get
                {
                    return new System.IO.FileInfo(PhysicalPath).Length;
                }
            }

            public string Name
            {
                get
                {
                    return Path.GetFileName(PhysicalPath);
                }
            }

            public string PhysicalPath
            {
                get;
                private set;
            }

            public Stream CreateReadStream()
            {
                return File.OpenRead(PhysicalPath);
            }
        }
    }
}
