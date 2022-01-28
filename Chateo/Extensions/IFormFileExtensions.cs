using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chateo.Extensions
{
    public static class IFormFileExtensions
    {
        public static bool IsImage(this IFormFile file)
        {
            if (file.Length > 0 && file.ContentType.Contains("image"))
                return true;
            
            return false;
        }
    }
}
