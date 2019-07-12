using Newtonsoft.Json;
using OpenIdTest.Bll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        public async Task<ActionResult> Index(string code, string state)
		{
            var openIdRequest = new OpenIdRequest();
            
            var tempAuthentiction = new TempAuthentication();
            var tempState = await tempAuthentiction.GetTempStateAsync();

            if (!state.Equals(tempState.Item1, StringComparison.Ordinal))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "state invalid"); 
            }

            var tokenResponse = await openIdRequest.RequestTokenAsync(code);
            await openIdRequest.ValidateResponseAndSignInAsync(tokenResponse);

            return RedirectToAction("Index", "Home");
		}
    }
}