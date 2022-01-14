using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chateo.Components
{
    public class FooterViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            if(ViewBag.Footer !=null)
                return View($"{ViewBag.Footer}Footer");

            return View($"{ViewBag.Title}Footer");
        }
    }
}
