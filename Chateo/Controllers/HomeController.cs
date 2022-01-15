using Chateo.Infrastructure.Repositories;
using Chateo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Chateo.Extensions;
using Chateo.Models.ViewModels;
using Chateo.Databases;

namespace Chateo.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly AppDbContext _context;
        private readonly IAppRepository _chatRepository;

        public HomeController(
            UserManager<User> userManager, 
            SignInManager<User> signInManager,
            AppDbContext context,
            IAppRepository chatRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _chatRepository = chatRepository;
        }

        public IActionResult Profile(int userName)
        {
            return View();
        }

        public IActionResult More()
        {
            return View();
        }

        public IActionResult Index()
        {
            string currentUserId = this.GetCurrentUserId();

            var friends = _chatRepository.GetUserFriends(currentUserId);

            return View("Contacts", friends);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFriendRequest(string userName)
        {
            User targetUser = await _userManager.FindByNameAsync(userName);

            string userId = targetUser.Id;

            await _chatRepository.DeleteFriendRequestAsync(this.GetCurrentUserId(), userId);

            return Ok();
        }

        [HttpPost] 
        public async Task<IActionResult> ConfirmFriendRequest(string userName)
        {
            User targetUser = await _userManager.FindByNameAsync(userName);

            string userId = targetUser.Id;

            string currentUserId = this.GetCurrentUserId();

            await _chatRepository.ConfirmFriendRequestAsync(userId, currentUserId);
            await _chatRepository.CreatePrivateChatAsync(userId, currentUserId);

            return Ok();

        }

        [HttpPost]
        public async Task<IActionResult> SendFriendRequest(string userName)
        {
            User targetUser = await _userManager.FindByNameAsync(userName);

            string userId = targetUser.Id;

            string currentUserId = this.GetCurrentUserId();

            await _chatRepository.CreateFriendRequestAsync(currentUserId, userId);

            return Ok();
        }


        public IActionResult AddFriend()
        {
            string currentUserId = this.GetCurrentUserId();

            var model = new AddFriendViewModel
            {
                NotFriends = _chatRepository.GetUserNotFriends(currentUserId),
                RequestsFrom = _chatRepository.GetFriendRequestsTo(currentUserId).Select(f => f.UserFrom),
                RequestsTo = _chatRepository.GetFriendRequestsFrom(currentUserId).Select(f => f.UserTo)
            };

            return View(model);
        }


        public IActionResult Chats()
        {
            string currentUserId = this.GetCurrentUserId();

            var chats = _chatRepository.GetChatsByUserId(currentUserId);

            foreach(var chat in chats)
            {
                if (chat.ChatType == ChatType.Private)
                    chat.Title = chat.Users.First(u => u.Id != currentUserId).UserName;
            }

            return View(chats);
        }
    }
}
