using ChatLe.Models;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ChatLe.Repository.Firebase.Test
{
    public class FirebaseChatStoreTest
    {
        [Fact]
        public async Task FirebaseChatStoreMethodsThrowArgumentNullExceptionTest()
        {
            var provider = Initialize();
            var sut = GetStore(provider);

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.CreateAttendeeAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.CreateConversationAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.CreateMessageAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.CreateNotificationConnectionAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.DeleteNotificationConnectionAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.DeleteUserAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.GetConversationAsync(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>(
                async () => await sut.GetConversationAsync(new ChatLeUser(), null));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.GetConversationsAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.GetMessagesAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.GetNotificationConnectionAsync(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.GetNotificationConnectionAsync("test", null));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.IsGuess(null));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.UserHasConnectionAsync(null));
        }

        [Fact]
        public async Task CreateConversationByManagerTest()
        {
            var provider = Initialize();
            var sut = GetStore(provider);
            sut.Init();
            var manager = GetChatManager(provider);
            var userManager = provider.GetRequiredService<UserManager<ChatLeUser>>();

            var user1 = new ChatLeUser { UserName = "test1", NormalizedUserName= "TEST1" };
            var user2 = new ChatLeUser { UserName = "test2", NormalizedUserName = "TEST2" };

            await userManager.CreateAsync(user1);
            await userManager.CreateAsync(user2);

            var conversation1 = await manager.GetOrCreateConversationAsync(user1.NormalizedUserName, user2.NormalizedUserName, "test");
            Assert.NotNull(conversation1);

            var conversation2 = await manager.GetOrCreateConversationAsync(user2.NormalizedUserName, user1.NormalizedUserName, "test");

            Assert.NotNull(conversation2);
            Assert.Equal(conversation1.Id, conversation2.Id);
        }

        [Fact]
        public async Task GetUsersConnectedTest()
        {
            var provider = Initialize();
            var sut = GetStore(provider);
            sut.Init();
            var manager = GetChatManager(provider);
            var userManager = provider.GetRequiredService<UserManager<ChatLeUser>>();

            var user1 = new ChatLeUser { UserName = "test1", NormalizedUserName = "TEST1" };
            var user2 = new ChatLeUser { UserName = "test2", NormalizedUserName = "TEST2" };

            await userManager.CreateAsync(user1);
            await userManager.CreateAsync(user2);

            await manager.AddConnectionIdAsync(user1.NormalizedUserName, "test1", "test");
            await manager.AddConnectionIdAsync(user1.NormalizedUserName, "test2", "test");

            Thread.Sleep(500); // wait for connection count computed

            var result = await manager.GetUsersConnectedAsync();

            Thread.Sleep(500); // wait for connection count computed

            Assert.Equal(2, result.Count());

            await manager.RemoveConnectionIdAsync("test2", "test", false);

            Thread.Sleep(500); // wait for connection count computed

            result = await manager.GetUsersConnectedAsync();

            Assert.Single(result);

            await manager.RemoveConnectionIdAsync("test1", "test", false);

            result = await manager.GetUsersConnectedAsync();

            Assert.Empty(result);
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
                .AddFirebaseStores(configuration["FirebaseOptions:DatabaseUrl"], p =>
                {
                    return GoogleCredential.FromFile(@"..\..\..\..\privatekey.json")
                        .CreateScoped("https://www.googleapis.com/auth/userinfo.email", "https://www.googleapis.com/auth/firebase.database")
                        .UnderlyingCredential;
                })
                .AddDefaultTokenProviders();

            services.AddLogging()
                .AddChatLe()
                .AddFirebaseChatStore<ChatLeUser, Conversation, Attendee, Message, NotificationConnection>();

            return services.BuildServiceProvider();
        }

        private IChatStore<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection> GetStore(IServiceProvider provider)
        {
            return provider.GetRequiredService<IChatStore<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>>();
        }

        private IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection> GetChatManager(IServiceProvider provider)
        {
            return provider.GetRequiredService<IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>>();
        }
    }
}
