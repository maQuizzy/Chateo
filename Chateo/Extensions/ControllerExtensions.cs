using Chateo.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Chateo.Extensions
{
    public static class ControllerExtensions
    {
        public static string GetCurrentUserId (this Controller controller)
        {
            return controller.User?.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
    }
}
