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
            return Redirect("/Home/Index");
        }
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                ViewBag.Href = returnUrl;
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

                ident.AddClaims(LocationClaimsProvider.GetClaims(ident));
                ident.AddClaims(ClaimsRoles.CreateRolesFromClaims(ident));
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

        [HttpPost]
        [AllowAnonymous]
        public ActionResult GoogleLogin(string returnUrl)
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleLoginCallback", new { returnUrl = returnUrl })
            };

            HttpContext.GetOwinContext().Authentication.Challenge(properties, "Google");
            return new HttpUnauthorizedResult();
        }

        [AllowAnonymous]
        public async Task<ActionResult> GoogleLoginCallback(string returnUrl)
        {
            ExternalLoginInfo loginInfo = await AuthManager.GetExternalLoginInfoAsync();
            AppUser user = await UserManager.FindAsync(loginInfo.Login);

            if (user == null)
            {
                user = new AppUser
                {
                    Email = loginInfo.Email,
                    UserName = loginInfo.DefaultUserName,
                    City = Cities.LONDON,
                    Country = Countries.ENG
                };

                IdentityResult result = await UserManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return View("Error", result.Errors);
                }
                else
                {
                    result = await UserManager.AddLoginAsync(user.Id, loginInfo.Login);
                    if (!result.Succeeded)
                    {
                        return View("Error", result.Errors);
                    }
                }
            }

            ClaimsIdentity ident = await UserManager.CreateIdentityAsync(user,
                DefaultAuthenticationTypes.ApplicationCookie);

            ident.AddClaims(loginInfo.ExternalIdentity.Claims);

            AuthManager.SignIn(new AuthenticationProperties
            {
                IsPersistent = false
            }, ident);

            return Redirect(returnUrl ?? "/");


        }
    }
}