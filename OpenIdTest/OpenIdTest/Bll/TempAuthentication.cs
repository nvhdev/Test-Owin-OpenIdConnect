using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace OpenIdTest.Bll
{
    public class TempAuthentication
    {

        public void SetTempState(string state, string nonce)
        {
            var tempId = new ClaimsIdentity("TempState");
            tempId.AddClaim(new Claim("state", state));
            tempId.AddClaim(new Claim("nonce", nonce));

            HttpContext.Current.Request.GetOwinContext().Authentication.SignIn(tempId);
        }

        public async Task<Tuple<string, string>> GetTempStateAsync()
        {
            var data = await HttpContext.Current.GetOwinContext().Authentication.AuthenticateAsync("TempState");

            var state = data.Identity.FindFirst("state").Value;
            var nonce = data.Identity.FindFirst("nonce").Value;

            return Tuple.Create(state, nonce);
        }
        public void SignOut()
        {
            HttpContext.Current.Request.GetOwinContext().Authentication.SignOut("TempState");
        }

    }
}