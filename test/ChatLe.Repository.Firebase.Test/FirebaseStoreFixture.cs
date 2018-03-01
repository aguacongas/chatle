using Aguacongas.Firebase;
using ChatLe.Cryptography;
using ChatLe.Models;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatLe.Repository.Firebase.Test
{
    public class FirebaseStoreFixture
    {
        public IServiceProvider Provider { get; private set; }

        public FirebaseStoreFixture()
        {
            Provider = Initialize();

            GetStore().Init();

            GetClient().DeleteAsync("/").GetAwaiter().GetResult();
        }

        private IServiceProvider Initialize()
        {
            var builder = new ConfigurationBuilder();
            var configuration = builder.AddUserSecrets<FirebaseChatStoreTest>()
                .AddEnvironmentVariables()
                .Build();

            var services = new ServiceCollection();
            services.AddIdentity<ChatLeUser, IdentityRole>(options =>
                {
                    var userOptions = options.User;
                    userOptions.AllowedUserNameCharacters += " ";
                })
                .AddFirebaseStores(configuration["FirebaseOptions:TestDatabaseUrl"], p =>
                {
                    using (var utility = new Utility(configuration["FirebaseOptions:SecureKey"]))
                    {
                        using (var stream = utility.DecryptFile(@"..\..\..\..\privatekey.json.enc").GetAwaiter().GetResult())
                        {
                            return GoogleCredential.FromStream(stream)
                                .CreateScoped("https://www.googleapis.com/auth/userinfo.email", "https://www.googleapis.com/auth/firebase.database")
                                .UnderlyingCredential;
                        }
                    }
                })
                .AddDefaultTokenProviders();

            services.AddLogging()
                .AddChatLe()
                .AddFirebaseChatStore<ChatLeUser, Conversation, Attendee, Message, NotificationConnection>();

            return services.BuildServiceProvider();
        }

        public IChatStore<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection> GetStore()
        {
            return Provider.GetRequiredService<IChatStore<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>>();
        }

        public IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection> GetChatManager()
        {
            return Provider.GetRequiredService<IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>>();
        }

        public UserManager<ChatLeUser> GetUserManager()
        {
            return Provider.GetRequiredService<UserManager<ChatLeUser>>();
        }

        public IFirebaseClient GetClient()
        {
            return Provider.GetRequiredService<IFirebaseClient>();
        }
    }
}
