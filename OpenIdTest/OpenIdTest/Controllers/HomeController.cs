using IdentityModel.Client;
using Microsoft.AspNet.Identity;
using System;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using static IdentityModel.OidcConstants;

namespace OpenIdTest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Request.GetOwinContext().Authentication.SignOut("Cookies");

            return View();
        }

        [HttpPost]
        public ActionResult Index(string scopes)
        {
            return View();
        }
    }
}