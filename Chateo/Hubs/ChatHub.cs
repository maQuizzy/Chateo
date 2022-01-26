using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Chateo.Infrastructure.Repositories;
using Chateo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Chateo.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IAppRepository _appRepository;

        public ChatHub(IAppRepository appRepository)
        {
            _appRepository = appRepository;
        }

        public async Task JoinChat(string chatId)
        {
            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (_appRepository.GetChatsByUserId(userId).First(c => c.Id.ToString() == chatId) != null)
                await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
        }

        public Task LeaveChat(string chatId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
        }

        public async Task ReadMessage(string chatId, int messageId)
        {
            var message = _appRepository.GetMessageById(messageId);

            await _appRepository.ReadMessageAsync(messageId);
            await Clients.User(message.User.UserName).SendAsync("ReadMessageReceive", messageId);
        }
    }
}
