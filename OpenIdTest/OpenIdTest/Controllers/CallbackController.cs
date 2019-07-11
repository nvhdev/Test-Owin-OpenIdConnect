using Newtonsoft.Json;
using OpenIdTest.Bll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OpenIdTest.Controllers
{
    public class CallbackController : Controller
    {
        public async Task<ActionResult> Index()
		{
            var openIdRequest = new OpenIdRequest();
            var code = Request.QueryString["code"] ?? "none";
            
            var state = Request.QueryString["state"];
            var tempAuthentiction = new TempAuthentication();
            var tempState = await tempAuthentiction.GetTempStateAsync();

            if (state.Equals(tempState.Item1, StringComparison.Ordinal))
            {
                ViewBag.State = state + " (valid)";
            }
            else
            {
                ViewBag.State = state + " (invalid)";
            }

            ViewBag.Error = Request.QueryString["error"] ?? "none";

            var tokenResponse = await openIdRequest.RequestToken(code);
            await openIdRequest.ValidateResponseAndSignInAsync(tokenResponse);

            return RedirectToAction("Index", "Home");
		}
    }
}