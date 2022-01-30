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
using Microsoft.AspNetCore.Http;
using Chateo.Extensions;
using System.IO;

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
                await _appRepository.ReadMessageAsync(currentUserId, notReadMess.Id);
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
            int? repliedMessageId,
            IFormFile uploadedFile,
            [FromServices] IHubContext<ChatHub> chatHub,
            [FromServices] IHubContext<MainHub> mainHub)
        {
            if (string.IsNullOrEmpty(messageText) && uploadedFile == null)
                return Ok();

            var chat = _appRepository.GetChatById(chatId);
            var user = await _userManager.GetUserAsync(User);

            if (chat.Users.Contains(user))
            {
                byte[] imageData = null;

                if (uploadedFile != null && uploadedFile.IsImage())
                {
                    using (var binaryReader = new BinaryReader(uploadedFile.OpenReadStream()))
                    {
                        imageData = binaryReader.ReadBytes((int)uploadedFile.Length);
                    }
                }

                var currentDate = DateTime.Now;

                var message = await _appRepository.CreateMessageAsync(
                chatId,
                user.Id,
                messageText,
                imageData,
                currentDate,
                repliedMessageId);

                Message repliedMessage = null;

                if(repliedMessageId != null)
                {
                    int repliedId = (int)repliedMessageId;
                    repliedMessage = _appRepository.GetMessageById(repliedId);
                }

                await chatHub.Clients.Group(chatId.ToString())
                    .SendAsync("ReceiveMessage",
                    message.Id,
                    messageText,
                    imageData,
                    user.UserName,
                    currentDate.ToString("t"),
                    repliedMessage?.User.UserName,
                    repliedMessage?.Text,
                    repliedMessage?.Image);

                var userIds = chat.Users
                    .Where(u => u.Id != user.Id)
                    .Select(u => u.UserName);

                await mainHub.Clients.Users(userIds)
                    .SendAsync("ReceiveMessageInChatList", 
                    chatId, 
                    messageText, 
                    user.UserName, 
                    currentDate.ToString("t"));
            }

            return Ok();
        }
    }
}
