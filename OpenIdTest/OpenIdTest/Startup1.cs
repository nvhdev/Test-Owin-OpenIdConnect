using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Diagnostics;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;

[assembly: OwinStartup(typeof(OpenIdTest.Startup1))]

namespace OpenIdTest
{
    public class Startup1
    {
        public void Configuration(IAppBuilder app)
        {
            Configuration2(app);
        }
        private void Configuration2(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                CookieName = "InternalUser",
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                SlidingExpiration = true,
                ExpireTimeSpan = TimeSpan.FromHours(24)
            });
        }
    }
}
