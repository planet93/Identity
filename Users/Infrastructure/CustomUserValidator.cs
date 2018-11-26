using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Users.Models;
using System.Text.RegularExpressions;

namespace Users.Infrastructure
{
    public class CustomUserValidator:IIdentityValidator<AppUser>
    {

        public async Task<IdentityResult> ValidateAsync(AppUser item)
        {
            List<string> errors = new List<string>();
            if (!item.Email.ToLower().EndsWith("@mail.ru"))
            {
                errors.Add("Адрес должен быть @mail.ru");
            }
            if (string.IsNullOrEmpty(item.UserName.Trim()))
            {
                errors.Add("Вы указали пустое имя");
            }
            string userNamePattern = @"^[a-zA-Z0-9а-яА-Я]+$";
            if(!Regex.IsMatch(item.UserName, userNamePattern))
            {
                errors.Add("В имени разрешаются указывать буквы английского или русского языков, и цифры");
            }
            if(errors.Count > 0)
            {
                return IdentityResult.Failed(errors.ToArray());
            }

            return IdentityResult.Success;
        }
    }
}