using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Users.Infrastructure;
using Microsoft.Owin.Security.Google;
using System.Web.Configuration;
using Microsoft.Owin.Security;
using System;

namespace Users.App_Start
{
    public class IdentityConfig
    {
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext<AppIdentityDbContext>(AppIdentityDbContext.Create);
            app.CreatePerOwinContext<AppRoleManager>(AppRoleManager.Create);
            app.CreatePerOwinContext<AppUserManager>(AppUserManager.Create);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
            {
                AuthenticationType = "Google",
                ClientId = "775280464342-a5fdot8qcqj1p45k0dfp8kcsau6t1fc1.apps.googleusercontent.com",
                ClientSecret = "7eFWrBWBOzCMYLfn30OAoC_f",
                Caption = "Авторизация через Google+",
                CallbackPath = new PathString("/GoolgleLoginCallback"),
                AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Passive,
                SignInAsAuthenticationType = app.GetDefaultSignInAsAuthenticationType(),
                BackchannelTimeout = TimeSpan.FromSeconds(60),
                BackchannelHttpHandler = new System.Net.Http.WebRequestHandler(),
                BackchannelCertificateValidator = null,
                Provider = new GoogleOAuth2AuthenticationProvider()
            }
            );

        }
    }
}