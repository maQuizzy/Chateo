using Chateo.Extensions;
using Chateo.Infrastructure.Repositories;
using Chateo.Models;
using Chateo.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Chateo.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IAppRepository _appRepository;
        private readonly UserManager<User> _userManager;

        public ProfileController(IAppRepository appRepository, UserManager<User> userManager)
        {
            _appRepository = appRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string userName)
        {
            ViewBag.IsFriends = false;
            ViewBag.OwnProfile = false;

            if (userName == null)
            {
                userName = User.FindFirst(ClaimTypes.Name).Value;
                ViewBag.OwnProfile = true;
            }

            ViewBag.ProfileHeader = userName;

            var user = await _userManager.FindByNameAsync(userName);

            if (ViewBag.OwnProfile)
                return View(user);

            if (_appRepository.GetUserFriends(this.GetCurrentUserId()).Contains(user))
            {
                ViewBag.IsFriends = true;
                ViewBag.ChatId = _appRepository.GetPrivateChatByUsersId(this.GetCurrentUserId(), user.Id).Id;
            }

            return View(user);
        }

        public IActionResult Settings()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Settings(ProfileSettingsViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            await _userManager.UpdateAsync(user);

            return View(model);
        }
    }
}
