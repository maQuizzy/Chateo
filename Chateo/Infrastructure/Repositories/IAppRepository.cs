using Chateo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chateo.Infrastructure.Repositories
{
    public interface IAppRepository
    {
        Task DeleteFriendRequestAsync(string userId1, string userId2);
        IEnumerable<FriendRequest> GetFriendRequestsFrom(string userFromId);
        IEnumerable<FriendRequest> GetFriendRequestsTo(string userToId);
        Task<Friendship> ConfirmFriendRequestAsync(string userFromId, string userToId);
        Task<FriendRequest> CreateFriendRequestAsync(string userFromId, string userToId);
        Task<Friendship> CreateFriendshipAsync(string userId1, string userId2);
        IEnumerable<User> GetUserFriends(string userId);
        IEnumerable<User> GetUserNotFriends(string userId);
        Chat GetChatById(int chatId);
        IEnumerable<Chat> GetChatsByUserId(string userId);
        Task<int> CreatePrivateChatAsync(string userId1, string userId2);
        Task<Message> CreateMessageAsync(int chatId, string userId, string messageText);
    }
}
