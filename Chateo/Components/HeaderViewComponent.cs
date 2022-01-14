using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chateo.Components
{
    public class HeaderViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            if (ViewBag.Header != null)
                return View($"{ViewBag.Header}Header");

            return View($"{ViewBag.Title}Header");
        }
    }
}
