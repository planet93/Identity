using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Linq;


namespace Users.Infrastructure
{
    public class CustomPasswordValidator:PasswordValidator
    {
        public override async Task<IdentityResult> ValidateAsync(string item)
        {
            IdentityResult result = await base.ValidateAsync(item);
            if (item.Contains("12345"))
            {
                var errors = result.Errors.ToList();
                errors.Add("Пароль не должен содержать последовательности чисел.");
                result = new IdentityResult(errors);
            }
            return result;
        }
    }
}