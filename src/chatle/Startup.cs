using chatle.Hubs;
using ChatLe.Cryptography;
using ChatLe.Hubs;
using ChatLe.Models;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;

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
            MySql,
            Firebase
        }

        readonly IWebHostEnvironment _environment;
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("config.json")
                .AddJsonFile($"config.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();

            _environment = env;
        }

        public IConfiguration Configuration { get; private set; }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(builder =>
                {
                    builder.AddConsole()
                        .AddAzureWebAppDiagnostics()
                        .AddDebug()
                        .SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
                })
                .Configure<HubSettings>(hubSettings => Configuration.GetSection("HubSettings").Bind(hubSettings))
                .AddChatLe(options => options.UserPerPage = int.Parse(Configuration["ChatConfig:UserPerPage"]))
                .AddCors()
                .AddAntiforgery(options =>
                {
                    options.HeaderName = "X-XSRF-TOKEN";
                    var cookie = options.Cookie;
                    cookie.Name = "XSRF-TOKEN";
                    cookie.HttpOnly = false;
                    cookie.Path = "/";
                    cookie.SameSite = SameSiteMode.None;
                });

            ConfigureEntity(services);

            services.AddMvc()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });

            services.AddSignalR();

            var authServices = services.AddAuthentication();
            if (Configuration["Authentication:Facebook:AppId"] != null)
            { 
                authServices.AddFacebook(facebookOptions =>
                {
                    facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                    facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                    facebookOptions.SaveTokens = true;
                });
            }

            if (Configuration["Authentication:Twitter:ConsumerKey"] != null)
            {
                authServices.AddTwitter(twitterOptions =>
                {
                    twitterOptions.ConsumerKey = Configuration["Authentication:Twitter:ConsumerKey"];
                    twitterOptions.ConsumerSecret = Configuration["Authentication:Twitter:ConsumerSecret"];
                    twitterOptions.SaveTokens = true;

                });
            }
            if (Configuration["Authentication:Google:ClientId"] != null)
            {
                authServices.AddGoogle(googleOption =>
                {
                    googleOption.ClientId = Configuration["Authentication:Google:ClientId"];
                    googleOption.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                    googleOption.SaveTokens = true;
                });
            }
            if (Configuration["Authentication:MicrosoftAccount:ClientId"] != null)
            {
                authServices.AddMicrosoftAccount(microsoftAccountOptions =>
                {
                    microsoftAccountOptions.ClientId = Configuration["Authentication:MicrosoftAccount:ClientId"];
                    microsoftAccountOptions.ClientSecret = Configuration["Authentication:MicrosoftAccount:ClientSecret"];
                    microsoftAccountOptions.SaveTokens = true;
                });
            }
            if (Configuration["Authentication:Github:ClientId"] != null)
            {
                authServices.AddOAuth("Github", options =>
                {
                    options.ClientId = Configuration["Authentication:Github:ClientId"];
                    options.ClientSecret = Configuration["Authentication:Github:ClientSecret"];
                    options.CallbackPath = new PathString("/signin-github");
                    options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                    options.TokenEndpoint = "https://github.com/login/oauth/access_token";
                    options.UserInformationEndpoint = "https://api.github.com/user";
                    options.ClaimsIssuer = "OAuth2-Github";
                    options.SaveTokens = true;
                    // Retrieving user information is unique to each provider.
                    options.Events = new OAuthEvents
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
                    };
                });
            }
        }

        private void ConfigureEntity(IServiceCollection services)
        {
            var identityBuilder = services.AddIdentity<ChatLeUser, IdentityRole>(options =>
                        {
                            var userOptions = options.User;
                            userOptions.AllowedUserNameCharacters += " ";
                        })
                .AddDefaultTokenProviders();

            var identityEngine = (DBEngine)Enum.Parse(typeof(DBEngine), Configuration["IdentityDatabase"]);

            switch(identityEngine)
            {
                case DBEngine.Redis:
                    identityBuilder.AddRedisStores(Configuration["Data:DefaultConnection:ConnectionString"]);
                    break;
                case DBEngine.Firebase:
                    identityBuilder.AddFirebaseStores(Configuration["FirebaseOptions:DatabaseUrl"], provider =>
                    {
                        using (var utility = new Utility(Configuration["FirebaseOptions:SecureKey"]))
                        {
                            using (var stream = utility.DecryptFile("firebase-key.json.enc").GetAwaiter().GetResult())
                            {
                                return GoogleCredential.FromStream(stream)
                                    .CreateScoped("https://www.googleapis.com/auth/userinfo.email", "https://www.googleapis.com/auth/firebase.database")
                                    .UnderlyingCredential;
                            }
                        }
                    });
                    break;
                default:
                    identityBuilder.AddEntityFrameworkStores<ChatLeIdentityDbContext>();
                    break;
            }

            var dbEngine = (DBEngine)Enum.Parse(typeof(DBEngine), Configuration["DatabaseEngine"]);
            
            if (dbEngine != DBEngine.Firebase)
            {
                services.AddDbContext<ChatLeIdentityDbContext>(options =>
                {
                    if (_environment.IsDevelopment())
                        options.EnableSensitiveDataLogging();

                    switch (dbEngine)
                    {
                        case DBEngine.InMemory:
                            options.UseInMemoryDatabase("chatle");
                            break;
                        case DBEngine.SqlServer:
                            options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"], o => o.MigrationsAssembly("ChatLe.Repository.Identity.SqlServer"));
                            break;
                        case DBEngine.SQLite:
                            options.UseSqlite(Configuration["Data:DefaultConnection:ConnectionString"], o => o.MigrationsAssembly("ChatLe.Repository.Identity.Sqlite"));
                            break;
                }
                });
            }
            else
            {
                services.AddFirebaseChatStore();
            }
        }

        public virtual void Configure(IApplicationBuilder app, IAntiforgery antiforgery, ILoggerFactory loggerFactory)
        {
            ConfigureErrors(app);
            app.Use(async (context, next) =>
            {
                var logger = loggerFactory.CreateLogger("ValidRequestMW");

                logger.LogInformation("Request Cookie is " + context.Request.Cookies["XSRF-TOKEN"]);
                logger.LogInformation("Request Header is " + context.Request.Headers["X-XSRF-TOKEN"]);

                await next();
            })
                .UseRouting()
                .UseCors(
                builder => builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod())
                .UseStaticFiles()
                .UseWebSockets()
                .UseAuthorization()
                .UseAuthentication()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}");
                    endpoints.MapHub<ChatHub>("/chat");
                    endpoints.Map("/cls", async context =>
                    {
                        var response = context.Response;
                        foreach (var cookie in context.Request.Cookies)
                        {
                            response.Cookies.Delete(cookie.Key);
                        }
                        await context.Response.WriteAsync(string.Empty);
                    });
                    endpoints.Map("/xhrf", async context =>
                    {
                        var tokens = antiforgery.GetAndStoreTokens(context);
                        await context.Response.WriteAsync(tokens.RequestToken);
                    });
                })
                .UseChatLe();
        }

        protected virtual void ConfigureErrors(IApplicationBuilder app)
        {
            if (string.Equals(_environment.EnvironmentName, "Development", StringComparison.OrdinalIgnoreCase))
            {
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
