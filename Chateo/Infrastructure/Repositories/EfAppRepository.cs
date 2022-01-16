using Chateo.Databases;
using Chateo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chateo.Infrastructure.Repositories
{
    public class EfAppRepository : IAppRepository
    {
        private readonly AppDbContext _ctx;

        public EfAppRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task DeleteFriendRequestAsync(string userId1, string userId2)
        {
            var friendRequest = _ctx.FriendRequests
                                    .First(f => (f.UserFromId == userId1 && f.UserToId == userId2) ||
                                                (f.UserFromId == userId2 && f.UserToId == userId1));

            _ctx.FriendRequests.Remove(friendRequest);

            await _ctx.SaveChangesAsync();
        }

        public IEnumerable<FriendRequest> GetFriendRequestsFrom(string userFromId)
        {
            return _ctx.FriendRequests
                   .Include(f => f.UserFrom)
                   .Include(f => f.UserTo)
                   .Where(f => f.UserFromId == userFromId);
        }

        public IEnumerable<FriendRequest> GetFriendRequestsTo(string userToId)
        {
            return _ctx.FriendRequests
                   .Include(f => f.UserFrom)
                   .Include(f => f.UserTo)
                   .Where(f => f.UserToId == userToId);
        }

        public async Task<Friendship> ConfirmFriendRequestAsync(string userFromId, string userToId)
        {
            var friendRequest = _ctx.FriendRequests.First(f => f.UserFromId == userFromId && f.UserToId == userToId);

            _ctx.FriendRequests.Remove(friendRequest);

            return await CreateFriendshipAsync(userFromId, userToId);

        }

        public async Task<FriendRequest> CreateFriendRequestAsync(string userFromId, string userToId)
        {

            var friendRequestsTo = GetFriendRequestsTo(userFromId);

            if (friendRequestsTo.Select(f => f.UserToId).Contains(userFromId))
                return null;

            var userFrom = _ctx.Users.First(u => u.Id == userFromId);
            var userTo = _ctx.Users.First(u => u.Id == userToId);

            var friendRequest = new FriendRequest
            {
                UserFromId = userFromId,
                UserToId = userToId
            };

            _ctx.FriendRequests.Add(friendRequest);

            await _ctx.SaveChangesAsync();

            return friendRequest;
        }

        public async Task<Friendship> CreateFriendshipAsync(string userId1, string userId2)
        {
            var friendship = new Friendship();

            var user1 = _ctx.Users.First(u => u.Id == userId1);
            var user2 = _ctx.Users.First(u => u.Id == userId2);

            friendship.User = user1;
            friendship.UserFriend = user2;

            _ctx.Friendships.Add(friendship);

            await _ctx.SaveChangesAsync();

            return friendship;
        }

        public IEnumerable<User> GetUserFriends(string userId)
        {

            var friendships = _ctx.Friendships
                .Include(f => f.User)
                .Include(f => f.UserFriend)
                .Where(f => f.UserId == userId || f.UserFriendId == userId);

            var friends = new List<User>();

            foreach (var friendship in friendships)
            {
                if (friendship.UserId != userId)
                    friends.Add(friendship.User);
                else
                    friends.Add(friendship.UserFriend);
            }

            return friends;
        }

        public IEnumerable<User> GetUserNotFriends(string userId)
        {
            var friends = GetUserFriends(userId);

            return _ctx.Users
                .Include(u => u.FriendRequests)
                .Where(u => !friends.Contains(u) && u.Id != userId);
        }

        public Chat GetChatById(int chatId)
        {
            return _ctx.Chats
                .Include(c => c.Users)
                .Include(c => c.Messages)
                .FirstOrDefault(c => c.Id == chatId);
        }

        public Chat GetPrivateChatByUsersId(string userId1, string userId2)
        {
            var user1 = _ctx.Users.First(u => u.Id == userId1);
            var user2 = _ctx.Users.First(u => u.Id == userId2);

            return _ctx.Chats.First(c => c.ChatType == ChatType.Private &&
                                    c.Users.Contains(user1) &&
                                    c.Users.Contains(user2));
        }

        public IEnumerable<Chat> GetChatsByUserId(string userId)
        {
            var user = new User { Id = userId };

            _ctx.Users.Attach(user);

            var chats = _ctx.Chats
                .Include(c => c.Users)
                .Include(c => c.Messages)
                .Where(c => c.Users.Contains(user));

            return chats;
        }

        public async Task<Message> CreateMessageAsync(int chatId, string userId, string messageText)
        {
            var message = new Message
            {
                UserId = userId,
                ChatId = chatId,
                Text = messageText
            };

            _ctx.Messages.Add(message);
            await _ctx.SaveChangesAsync();

            return message;
        }

        public async Task<int> CreatePrivateChatAsync(string userId1, string userId2)
        {
            var chat = new Chat();
            chat.ChatType = ChatType.Private;

            var user1 = _ctx.Users.First(u => u.Id == userId1);
            var user2 = _ctx.Users.First(u => u.Id == userId2);

            chat.Users.Add(user1);
            chat.Users.Add(user2);

            _ctx.Chats.Add(chat);

            await _ctx.SaveChangesAsync();

            return chat.Id;
        }
    }
}
