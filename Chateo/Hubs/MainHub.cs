using Chateo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Chateo.Hubs
{
    [Authorize]
    public class MainHub : Hub
    {
        private readonly UserManager<User> _userManager;

        public MainHub(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public override async Task<Task> OnConnectedAsync()
        {
            await SetUserStatus(true);

            return base.OnConnectedAsync();
        }

        public override async Task<Task> OnDisconnectedAsync(Exception exception)
        {
            await SetUserStatus (false);

            return base.OnDisconnectedAsync(exception);
        }

        private async Task SetUserStatus(bool isOnline)
        {
            var user = await _userManager.GetUserAsync(Context.User);
            user.IsOnline = isOnline;
            await _userManager.UpdateAsync(user);
        }
    }
}
