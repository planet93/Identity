using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;


namespace Users.Controllers
{
    public class ClaimsController : Controller
    {
        // GET: Claims
        public ActionResult Index()
        {
            ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
            if(ident == null)
            {
                return View("Error", new string[] { "Not aceess" });
            }
            else
            {
                return View(ident.Claims);
            }
        }

        [Authorize(Roles ="DCStaff")]
        public string OtherAction()
        {
            return "Это защищенный метод действия";
        }
    }
}