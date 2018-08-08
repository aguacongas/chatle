using ChatLe.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ChatLe.Repository.Firebase.Test
{
    public class FirebaseChatStoreTest: IClassFixture<FirebaseStoreFixture>
    {
        private readonly FirebaseStoreFixture _fixture;

        public FirebaseChatStoreTest(FirebaseStoreFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task FirebaseChatStoreMethodsThrowArgumentNullExceptionTest()
        {
            var sut = _fixture.GetStore();

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
            var sut = _fixture.GetStore();
            var manager = _fixture.GetChatManager();
            var userManager = _fixture.GetUserManager();

            var user1 = new ChatLeUser { UserName = "test1" };
            var user2 = new ChatLeUser { UserName = "test2" };

            await userManager.CreateAsync(user1);
            await userManager.CreateAsync(user2);

            var conversation1 = await manager.GetOrCreateConversationAsync(user1.UserName, user2.UserName, "test");
            Assert.NotNull(conversation1);

            var conversation2 = await manager.GetOrCreateConversationAsync(user2.UserName, user1.UserName, "test");

            Assert.NotNull(conversation2);
            Assert.Equal(conversation1.Id, conversation2.Id);
        }

        [Fact(Skip = "Too long")]
        public async Task GetUsersConnectedTest()
        {
            var sut = _fixture.GetStore();
            var manager = _fixture.GetChatManager();
            var userManager = _fixture.GetUserManager();

            var user1 = new ChatLeUser { UserName = "test1" };
            var user2 = new ChatLeUser { UserName = "test2" };

            await userManager.CreateAsync(user1);
            await userManager.CreateAsync(user2);

            var client = _fixture.GetClient();

            await client.DeleteAsync("connections");
            await client.DeleteAsync("connections-count");

            int? count;
            do
            {
                count = (await client.GetAsync<int?>("connections-count")).Data;
            }
            while (!count.HasValue || count != 0);

            await manager.AddConnectionIdAsync(user1.UserName, "test1", "test");
            await manager.AddConnectionIdAsync(user1.UserName, "test2", "test");

            while((await client.GetAsync<int>("connections-count")).Data != 2);
            var result = await manager.GetUsersConnectedAsync();

            Assert.Equal(2, result.Count());

            await manager.RemoveConnectionIdAsync("test2", "test", false);
            while ((await client.GetAsync<int>("connections-count")).Data != 1);

            result = await manager.GetUsersConnectedAsync();

            Assert.Single(result);

            await manager.RemoveConnectionIdAsync("test1", "test", false);
            do
            {
                count = (await client.GetAsync<int?>("connections-count")).Data;
            }
            while (!count.HasValue || count != 0);

            result = await manager.GetUsersConnectedAsync();

            Assert.Empty(result);
        }

        [Fact]
        public async Task RemoveConnectionIdAsyncTest()
        {
            var sut = _fixture.GetStore();
            var manager = _fixture.GetChatManager();
            var userManager = _fixture.GetUserManager();

            var user1 = new ChatLeUser { UserName = "test1" };
            var user2 = new ChatLeUser { UserName = "test2" };

            await userManager.CreateAsync(user1);
            await userManager.CreateAsync(user2);

            await manager.AddConnectionIdAsync(user1.UserName, "test1", "test");
            await manager.AddConnectionIdAsync(user1.UserName, "test2", "test");

            await manager.RemoveConnectionIdAsync("test2", "test", true);
        }

        [Fact]
        public async Task AddMessageTest()
        {
            var sut = _fixture.GetStore();
            var manager = _fixture.GetChatManager();
            var userManager = _fixture.GetUserManager();

            var user1 = new ChatLeUser { UserName = "test1" };
            var user2 = new ChatLeUser { UserName = "test2" };

            await userManager.CreateAsync(user1);
            await userManager.CreateAsync(user2);

            var conversation = await manager.GetOrCreateConversationAsync(user1.UserName, user2.UserName, "test");
            Assert.NotNull(conversation);

            var message = new Message { Text = Guid.NewGuid().ToString() };
            var result = await manager.AddMessageAsync(user1.UserName, conversation.Id, message);
            Assert.NotNull(result);
            Assert.Equal(conversation.Id, result.Id);
            Assert.Equal(2, result.Attendees.Count);
            Assert.Single(result.Messages);

            var messages = await manager.GetMessagesAsync(conversation.Id, max: 1);
            Assert.Single(messages);
            Assert.Equal(message.Text, messages.First().Text);
        }
    }
}
