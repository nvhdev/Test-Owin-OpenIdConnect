using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using OpenIdTest.Bll;
using OpenIdTest.Bll.OpenId;
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
            var authorizeOpenId = new Authorize();
            return Redirect(authorizeOpenId.CreateAuthorizeUrl());
        }

        public ActionResult Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut();

            return RedirectToAction("Index", "Home");
        }
    }
}