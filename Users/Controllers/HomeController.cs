using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Users.Infrastructure;
using Users.Models;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;

namespace Users.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("Ключ", "Значение");

            return View(GetData("Index"));
        }

        [Authorize(Roles ="Users")]
        public ActionResult OtherAction()
        {
            return View("Index", GetData("OtherAction"));
        }
        private Dictionary<string, object> GetData(string actionName)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            dict.Add("Action", actionName);
            dict.Add("Пользователь", HttpContext.User.Identity.Name);
            dict.Add("Аутентифицирован?", HttpContext.User.Identity.IsAuthenticated);
            dict.Add("Тип аутентификации", HttpContext.User.Identity.AuthenticationType);
            dict.Add("В роли Users?", HttpContext.User.IsInRole("Users"));

            return dict;
        }

        [Authorize]
        public ActionResult UserProps()
        {
            return View(CurrentUser);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> UserProps(Cities city)
        {
            AppUser user = CurrentUser;
            user.City = city;

            user.SetCountryFromCity(city);

            await UserManagerCont.UpdateAsync(user);
            return View(user);
        }

        private AppUser CurrentUser
        {
            get { return UserManagerCont.FindByName(HttpContext.User.Identity.Name); }
        }

        private AppUserManager UserManagerCont
        {
            get { return HttpContext.GetOwinContext().GetUserManager<AppUserManager>(); }
        }

        [NonAction]
        public static string GetCityName<TEnum>(TEnum item) where TEnum: struct, IConvertible
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("Тип перечисления должен быть перечислением");
            }
            else
            {
                return item.GetType().GetMember(item.ToString()).First().GetCustomAttribute<DisplayAttribute>().Name;
            }
        }
    }
}