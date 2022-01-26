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
        private readonly IAppRepository _appRepository;

        private const int ChatPageSize = 15;

        public ChatController(UserManager<User> userManager, IAppRepository chatRepository)
        {
            _userManager = userManager;
            _appRepository = chatRepository;
        }

        public async Task<IActionResult> Index(int chatId,
            int? pageNumber,
            [FromServices] IHubContext<ChatHub> chatHub)
        {

            var chat = _appRepository.GetChatById(chatId);

            if (chat == null)
                return RedirectToAction("Index", "Home");

            int page = pageNumber ?? 0;

            var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (isAjax)
            {
                return PartialView("Messages", GetMessagesPage(chat.Messages, page));
            }

            var currentUser = await _userManager.GetUserAsync(User);

            if (!chat.Users.Contains(currentUser))
                return RedirectToAction("Index", "Home");

            if (chat == null)
                return RedirectToAction("Index", "Home");

            if (chat.ChatType == ChatType.Private)
            {
                var otherUser = chat.Users.First(user => user.Id != User.FindFirst(ClaimTypes.NameIdentifier).Value);

                ViewBag.ChatTitle = otherUser.UserName;
                ViewBag.OtherUserId = otherUser.Id;
                ViewBag.ChatPageSize = ChatPageSize;

                await ReadMessages(chat.Id, currentUser.Id, chatHub);

                return View("Private", chat);
            }

            return Ok();
        }

        private async Task ReadMessages(int chatId, string currentUserId, IHubContext<ChatHub> chatHub)
        {
            var notReadMessages = _appRepository.GetChatById(chatId).Messages
                .Where(m => m.Read == false && m.UserId != currentUserId);

            foreach (var notReadMess in notReadMessages)
            {
                await chatHub.Clients.User(notReadMess.User.UserName).SendAsync("ReadMessageReceive", notReadMess.Id);
                await _appRepository.ReadMessageAsync(notReadMess.Id);
            }
        }

        private IEnumerable<Message> GetMessagesPage(IEnumerable<Message> messages, int page = 1)
        {
            var itemsToSkip = page * ChatPageSize;

            var messagess = messages
                .SkipLast(itemsToSkip)
                .TakeLast(ChatPageSize);

            return messagess;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(
            int chatId,
            string messageText,
            [FromServices] IHubContext<ChatHub> chatHub)
        {
            if (string.IsNullOrEmpty(messageText))
                return Ok();

            var chat = _appRepository.GetChatById(chatId);
            var user = await _userManager.GetUserAsync(User);

            if (chat.Users.Contains(user))
            {

                var currentDate = DateTime.Now;

                var message = await _appRepository.CreateMessageAsync(
                chatId,
                user.Id,
                messageText,
                currentDate);

                await chatHub.Clients.Group(chatId.ToString())
                    .SendAsync("ReceiveMessage", message.Id, messageText, user.UserName, currentDate.Hour, currentDate.Minute);
            }

            return Ok();
        }
    }
}
