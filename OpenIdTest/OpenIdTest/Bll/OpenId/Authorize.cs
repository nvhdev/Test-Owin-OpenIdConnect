using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenIdTest.Bll.OpenId
{
    public class Authorize
    {
        public Authorize()
        {
        }
        public string CreateAuthorizeUrl()
        {
            string nonce = Guid.NewGuid().ToString("N");
            string state = Guid.NewGuid().ToString("N");

            var tempAuthentication = new TempAuthentication();
            tempAuthentication.SetTempState(state, nonce);

            return string.Format(OpenIdSettings.Settings.AuthorizeUrlFormat,
                OpenIdConfig.Config.AuthorizationEndpoint,
                HttpUtility.UrlEncode(OpenIdSettings.Settings.ClientId),
                HttpUtility.UrlEncode(OpenIdSettings.Settings.ResponseType),
                HttpUtility.UrlEncode(OpenIdSettings.Settings.Scope),
                HttpUtility.UrlEncode(OpenIdSettings.Settings.RedirectUri),
                HttpUtility.UrlEncode(state),
                HttpUtility.UrlEncode(nonce));
        }
    }
}