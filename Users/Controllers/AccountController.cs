using Users.Models;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Users.Infrastructure;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Security.Claims;
using Microsoft.AspNet.Identity.Owin;

namespace Users.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return View("Error", new string[] { "В доступе отказано" });
            }

            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login (loginViewModel details, string returnUrl)
        {
            AppUser user = await UserManager.FindAsync(details.Name, details.Password);

            if(user == null)
            {
                ModelState.AddModelError("", "Некорректное имя или пароль.");
            }
            else
            {
                ClaimsIdentity ident = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                AuthManager.SignOut();
                AuthManager.SignIn(new AuthenticationProperties { IsPersistent = false }, ident);
                return Redirect(returnUrl);
            }

            return View(details);
        }

        [Authorize]
        public ActionResult Logout()
        {
            AuthManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        private IAuthenticationManager AuthManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }
        private AppUserManager UserManager { get { return HttpContext.GetOwinContext().GetUserManager<AppUserManager>(); } }
    }
}