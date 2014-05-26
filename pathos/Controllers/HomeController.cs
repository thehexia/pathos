using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace pathos.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return PostLogin();
            }
            else
            {
                return View();
            }
        }

        [Authorize]
        public ActionResult PostLogin()
        {
            return View("PostLogin", "~/Views/Shared/_PostLoginLayout.cshtml");
        }
    }
}
