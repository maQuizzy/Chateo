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
        private readonly IAppRepository _appRepository;

        private const int AddFriendPageSize = 15;

        public HomeController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            AppDbContext context,
            IAppRepository chatRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _appRepository = chatRepository;
        }

        public async Task<IActionResult> More()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(user);
        }

        public IActionResult Index()
        {
            string currentUserId = this.GetCurrentUserId();

            var requestsFrom = _appRepository.GetFriendRequestsTo(currentUserId).Select(f => f.UserFrom);
            var requestsTo = _appRepository.GetFriendRequestsFrom(currentUserId).Select(f => f.UserTo);

            var friendViewModel = new AddFriendViewModel
            {
                RequestsFrom = requestsFrom,
                RequestsTo = requestsTo,
                NotFriends = _appRepository.GetUserNotFriends(currentUserId).Where(u => requestsFrom.Contains(u) || requestsTo.Contains(u))
            };

            var friends = _appRepository.GetUserFriends(currentUserId);

            var contactsModel = new ContactsViewModel
            {
                Users = friends,
                AddFriendViewModel = friendViewModel
            };

            return View("Contacts", contactsModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFriendRequest(string userName)
        {
            User targetUser = await _userManager.FindByNameAsync(userName);

            string userId = targetUser.Id;

            await _appRepository.DeleteFriendRequestAsync(this.GetCurrentUserId(), userId);

            return Json(new
            {
                username = userName,
                result = "RequestDeleted"
            });
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmFriendRequest(string userName)
        {
            User targetUser = await _userManager.FindByNameAsync(userName);

            string userId = targetUser.Id;

            string currentUserId = this.GetCurrentUserId();

            await _appRepository.ConfirmFriendRequestAsync(userId, currentUserId);
            await _appRepository.CreatePrivateChatAsync(userId, currentUserId);

            return Json(new
            {
                username = userName,
                result = "RequestConfirmed"
            });

        }

        [HttpPost]
        public async Task<IActionResult> SendFriendRequest(string userName)
        {
            User targetUser = await _userManager.FindByNameAsync(userName);

            string userId = targetUser.Id;

            string currentUserId = this.GetCurrentUserId();

            await _appRepository.CreateFriendRequestAsync(currentUserId, userId);

            return Json(new
            {
                username = userName,
                result = "RequestSent"
            });
        }


        public IActionResult AddFriend(string search, int? pageNumber)
        {
            var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            string currentUserId = this.GetCurrentUserId();

            var model = new AddFriendViewModel
            {
                RequestsFrom = _appRepository.GetFriendRequestsTo(currentUserId).Select(f => f.UserFrom),
                RequestsTo = _appRepository.GetFriendRequestsFrom(currentUserId).Select(f => f.UserTo)
            };


            if (isAjax)
            {
                int page = pageNumber ?? 0;

                int itemsToSkip = page * AddFriendPageSize;

                model.NotFriends = _appRepository.GetUserNotFriends(currentUserId)
                    .Where(u => u.UserName.Contains(search, StringComparison.InvariantCultureIgnoreCase))
                    .Skip(itemsToSkip)
                    .Take(AddFriendPageSize);

               return PartialView("AddFriendList", model);
            }

            model.NotFriends = _appRepository.GetUserNotFriends(currentUserId);

            return View(model);
        }


        public IActionResult Chats()
        {
            string currentUserId = this.GetCurrentUserId();

            var chats = _appRepository.GetChatsByUserId(currentUserId);

            foreach (var chat in chats)
            {
                if (chat.ChatType == ChatType.Private)
                    chat.Title = chat.Users.First(u => u.Id != currentUserId).UserName;
            }

            return View(chats);
        }
    }
}
