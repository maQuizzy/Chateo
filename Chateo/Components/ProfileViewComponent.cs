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

namespace Chateo.Components
{
    public class ProfileViewComponent : ViewComponent
    {
        private readonly IAppRepository _appRepository;
        private readonly UserManager<User> _userManager;

        public ProfileViewComponent(IAppRepository appRepository, UserManager<User> userManager)
        {
            _appRepository = appRepository;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentUser = await _userManager.GetUserAsync(UserClaimsPrincipal);

            var model = new ProfileComponentViewModel
            {
                TopChats = _appRepository.GetChatsByUserId(currentUser.Id)
                .Take(5)
                .OrderByDescending(c => c.Messages.Count),
                User = currentUser
            };


            foreach (var chat in model.TopChats)
            {
                if (chat.ChatType == ChatType.Private)
                {
                    var otherUser = chat.Users.First(u => u.Id != currentUser.Id);
                    chat.Title = otherUser.UserName;
                    chat.Avatar = otherUser.Avatar;
                    chat.OtherUser = otherUser;
                }
            }

            return View(model);
        }
    }
}
