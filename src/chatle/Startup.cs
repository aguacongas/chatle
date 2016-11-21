﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using ChatLe.Models;
using Chatle.EntityFrameworkCore.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.OAuth;
using Newtonsoft.Json.Linq;

namespace ChatLe
{
    public class Startup
    {
        enum DBEngine
        {
            SqlServer,
            InMemory,
            Redis,
            SQLite,
            MySql
        }

        readonly IHostingEnvironment _environment;
        public ILoggerFactory LoggerFactory { get; private set; }
        public Startup(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("config.json")
                .AddJsonFile($"config.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
                loggerFactory.AddDebug();
            }

            builder.AddEnvironmentVariables();

            Configuration = builder.Build();

            _environment = env;
            LoggerFactory = loggerFactory;

            loggerFactory.AddConsole();
        }

        public IConfiguration Configuration { get; private set; }

        public virtual void ConfigureServices(IServiceCollection services)
        {

            ConfigureEntity(services);

            services.AddCors();

            services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

            services.AddMvc();

            services.AddSignalR(options => options.Hubs.EnableDetailedErrors = _environment.EnvironmentName == "Development");

            services.AddChatLe(options => options.UserPerPage = int.Parse(Configuration["ChatConfig:UserPerPage"]));

        }

        private void ConfigureEntity(IServiceCollection services)
        {
            var dbEngine = (DBEngine)Enum.Parse(typeof(DBEngine), Configuration["DatabaseEngine"]);

            services.AddDbContext<ChatLeIdentityDbContext>(options =>
            {
                if (_environment.IsDevelopment())
                    options.EnableSensitiveDataLogging();

                switch (dbEngine)
                {
                    case DBEngine.InMemory:
                        options.UseInMemoryDatabase();
                        break;
                    case DBEngine.SqlServer:
                        options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"], o => o.MigrationsAssembly("ChatLe.Repository.Identity.SqlServer"));
                        break;
                    case DBEngine.SQLite:
                        options.UseSqlite(Configuration["Data:DefaultConnection:ConnectionString"], o => o.MigrationsAssembly("ChatLe.Repository.Identity.Sqlite"));
                        break;
                    case DBEngine.MySql:
                        options.UseMySql(Configuration["Data:DefaultConnection:ConnectionString"], o => o.MigrationsAssembly("ChatLe.Repository.Identity.MySql"));
                        break;
                    case DBEngine.Redis:
                        int port;
                        int database;
                        string hostName = Configuration["Data:Redis:HostName"] ?? "127.0.0.1";

                        if (!int.TryParse(Configuration["Data:Redis:Port"], out port))
                            port = 6379;
                        int.TryParse(Configuration["Data:Redis:Database"], out database);

                        options.UseRedisDatabase(hostName: hostName, port: port, database: database);            
                        break;
                }
            });

            services.AddIdentity<ChatLeUser, IdentityRole>(options =>
            {
                options.SecurityStampValidationInterval = TimeSpan.FromMinutes(20);
                var userOptions = options.User;
                userOptions.AllowedUserNameCharacters += " ";
            }).AddEntityFrameworkStores<ChatLeIdentityDbContext>();
        }

        public virtual void Configure(IApplicationBuilder app, IAntiforgery antiforgery)
        {

            ConfigureErrors(app);

            var logger = LoggerFactory.CreateLogger("request");
            app.UseCors(
                builder => builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials())
                .UseStaticFiles()
                .UseWebSockets()
                .UseIdentity()
                .UseFacebookAuthentication(new FacebookOptions()
                {
                    AppId = Configuration["Authentication:Facebook:AppId"],
                    AppSecret = Configuration["Authentication:Facebook:AppSecret"],
                    SaveTokens = true
                })
                .UseTwitterAuthentication(new TwitterOptions
                {
                    ConsumerKey = Configuration["Authentication:Twitter:ConsumerKey"],
                    ConsumerSecret = Configuration["Authentication:Twitter:ConsumerSecret"],
                    SaveTokens = true
                })
                .UseGoogleAuthentication(new GoogleOptions
                {
                    ClientId = Configuration["Authentication:Google:ClientId"],
                    ClientSecret = Configuration["Authentication:Google:ClientSecret"],
                    SaveTokens = true
                })
                .UseChatleMicrosoftAccountAuthentication(new MicrosoftAccountOptions
                {
                    ClientId = Configuration["Authentication:MicrosoftAccount:ClientId"],
                    ClientSecret = Configuration["Authentication:MicrosoftAccount:ClientSecret"],
                    SaveTokens = true
                })
                .UseOAuthAuthentication(new OAuthOptions
                {
                    AuthenticationScheme = "Github",
                    DisplayName = "Github",
                    ClientId = Configuration["Authentication:Github:ClientId"],
                    ClientSecret = Configuration["Authentication:Github:ClientSecret"],
                    CallbackPath = new PathString("/signin-github"),
                    AuthorizationEndpoint = "https://github.com/login/oauth/authorize",
                    TokenEndpoint = "https://github.com/login/oauth/access_token",
                    UserInformationEndpoint = "https://api.github.com/user",
                    ClaimsIssuer = "OAuth2-Github",
                    SaveTokens = true,
                    // Retrieving user information is unique to each provider.
                    Events = new OAuthEvents
                    {
                        OnCreatingTicket = async context =>
                        {
                        // Get the GitHub user
                        var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                            var response = await context.Backchannel.SendAsync(request, context.HttpContext.RequestAborted);
                            response.EnsureSuccessStatusCode();

                            var user = JObject.Parse(await response.Content.ReadAsStringAsync());

                            var identifier = user.Value<string>("id");
                            if (!string.IsNullOrEmpty(identifier))
                            {
                                context.Identity.AddClaim(new Claim(
                                    ClaimTypes.NameIdentifier, identifier,
                                    ClaimValueTypes.String, context.Options.ClaimsIssuer));
                            }

                            var userName = user.Value<string>("login");
                            if (!string.IsNullOrEmpty(userName))
                            {
                                context.Identity.AddClaim(new Claim(
                                    ClaimsIdentity.DefaultNameClaimType, userName,
                                    ClaimValueTypes.String, context.Options.ClaimsIssuer));
                            }

                            var name = user.Value<string>("name");
                            if (!string.IsNullOrEmpty(name))
                            {
                                context.Identity.AddClaim(new Claim(
                                    "urn:github:name", name,
                                    ClaimValueTypes.String, context.Options.ClaimsIssuer));
                            }

                            var email = user.Value<string>("email");
                            if (!string.IsNullOrEmpty(email))
                            {
                                context.Identity.AddClaim(new Claim(
                                    ClaimTypes.Email, email,
                                    ClaimValueTypes.Email, context.Options.ClaimsIssuer));
                            }

                            var link = user.Value<string>("url");
                            if (!string.IsNullOrEmpty(link))
                            {
                                context.Identity.AddClaim(new Claim(
                                    "urn:github:url", link,
                                    ClaimValueTypes.String, context.Options.ClaimsIssuer));
                            }
                        }
                    }
                })
                .Map("/xhrf", a => a.Run(async context =>
                {
                    var tokens = antiforgery.GetAndStoreTokens(context);
                    context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken, new CookieOptions() { HttpOnly = false });
                    await context.Response.WriteAsync(tokens.RequestToken);
                }))
                .Map("/cls", a => a.Run(async context =>
                {
                    var response = context.Response;
                    foreach (var cookie in context.Request.Cookies)
                    {
                        response.Cookies.Delete(cookie.Key);
                    }
                    await context.Response.WriteAsync(string.Empty);
                }))
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
                app.UseDatabaseErrorPage();
            }
            else
            {
                // Add Error handling middleware which catches all application specific errors and
                // send the request to the following path or controller action.
                app.UseExceptionHandler("/Home/Error");
            }
        }

        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();

            string rootPath = Directory.GetCurrentDirectory();
            if (args.Length == 1)
                rootPath += '/' + args[0];

            var host = new WebHostBuilder()
                .UseContentRoot(rootPath)
                .UseConfiguration(config)
                .UseStartup<Startup>()
                .UseIISIntegration()
                .UseKestrel()
                .Build();

            host.Run();
        }
    }
}
