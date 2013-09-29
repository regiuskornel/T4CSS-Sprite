using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApp.Controllers
{
    public class HomeController : Controller
    {
        private static int callNumber;
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AjaxCall()
        {
            return new ContentResult() { Content = "*** Success " + (callNumber++) + ". ***" };
        }

    }
}
