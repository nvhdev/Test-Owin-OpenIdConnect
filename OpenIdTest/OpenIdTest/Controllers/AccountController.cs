using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using OpenIdTest.Bll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace OpenIdTest.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login(string returnUrl)
        {
            //var claims = new List<Claim>();

            //claims.Add(new Claim(ClaimTypes.Role, "admin"));
            //claims.Add(new Claim(ClaimTypes.NameIdentifier, "mijnid"));
            //claims.Add(new Claim(ClaimTypes.Name, "Kloase"));

            //var newId = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
            //Request.GetOwinContext().Authentication.SignIn(newId);
            var openId = new OpenIdRequest();


            return Redirect(openId.CreateAuthorizeRequest());// RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult Claims()
        {
            return View();
        }

        [Authorize]
        public ActionResult Tokens()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;

            // Extract tokens
            string accessToken = claimsIdentity?.FindFirst(c => c.Type == "access_token")?.Value;
            string idToken = claimsIdentity?.FindFirst(c => c.Type == "id_token")?.Value;

            // Now you can use the tokens as appropriate...
            return View();
        }
    }
}