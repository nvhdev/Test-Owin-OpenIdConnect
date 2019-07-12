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
        public ActionResult Login()
        {
            var openId = new OpenIdRequest();
            return Redirect(openId.CreateAuthorizeRequest());
        }

        public ActionResult Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut();

            return RedirectToAction("Index", "Home");
        }
    }
}