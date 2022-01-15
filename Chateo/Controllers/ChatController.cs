using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Chateo.Hubs;
using Microsoft.AspNetCore.Identity;
using Chateo.Models;
using Chateo.Models.ViewModels;
using Chateo.Infrastructure;
using Chateo.Infrastructure.Repositories;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Chateo.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IAppRepository _chatRepository;

        public ChatController(UserManager<User> userManager, IAppRepository chatRepository)
        {
            _userManager = userManager;
            _chatRepository = chatRepository;
        }

        public IActionResult Index(int chatId)
        {
            var chat = _chatRepository.GetChatById(chatId);

            if (chat == null)
                return RedirectToAction("Index", "Home");

            if(chat.ChatType == ChatType.Private)
            {
                var otherUser = chat.Users.First(user => user.Id != User.FindFirst(ClaimTypes.NameIdentifier).Value);

                chat.Title = otherUser.UserName;
                ViewBag.ChatTitle = otherUser.UserName;

                return View("Private", chat);
            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(
            int chatId, 
            string messageText,
            [FromServices] IHubContext<ChatHub> chatHub)
        {
            if (string.IsNullOrEmpty(messageText))
                return Ok();

            var user = await _userManager.GetUserAsync(User);

            await _chatRepository.CreateMessageAsync(
                chatId, 
                user.Id, 
                messageText);

            await chatHub.Clients.Group(chatId.ToString())
                .SendAsync("ReceiveMessage", messageText, user.UserName);


            return Ok();
        }
    }
}
