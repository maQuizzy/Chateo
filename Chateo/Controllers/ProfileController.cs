using Chateo.Extensions;
using Chateo.Infrastructure.Repositories;
using Chateo.Models;
using Chateo.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Chateo.Controllers
{
    [Authorize]
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
                userName = User.FindFirst(ClaimTypes.Name).Value;

            ViewBag.ProfileHeader = userName;

            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
                return RedirectToAction("Index");

            if(user.Id == this.GetCurrentUserId())
            {
                ViewBag.OwnProfile = true;
                return View(user);
            }

            if (_appRepository.GetUserFriends(this.GetCurrentUserId()).Contains(user))
            {
                ViewBag.IsFriends = true;
                ViewBag.ChatId = _appRepository.GetPrivateChatByUsersId(this.GetCurrentUserId(), user.Id).Id;
            }

            return View(user);
        }

        public async Task<IActionResult> Settings()
        {
            var user = await _userManager.GetUserAsync(User);

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Settings(string firstName, string lastName)
        {
            var user = await _userManager.GetUserAsync(User);

            user.FirstName = firstName;
            user.LastName = lastName;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return RedirectToAction("Index");

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UploadAvatar(IFormFile uploadedFile)
        {
            if (uploadedFile != null)
            {
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(uploadedFile.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)uploadedFile.Length);
                }
                var user = await _userManager.GetUserAsync(User);
                user.Avatar = imageData;

                var result = await _userManager.UpdateAsync(user);

            }

            return RedirectToAction("Index");
        }
    }
}
