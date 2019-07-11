using IdentityModel;
using IdentityModel.Client;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace OpenIdTest.Bll
{
    public class OpenIdRequest
    {
        private string _clientId;
        private string _clientSecret;
        private string _responseType;
        private string _scope;
        private string _redirectUri;
        private string _authorizeUri;
        private string _tokenUri;
        private string _auth0Domain;
        private string _wellKnownUri;

        private string _authorizeUrlFormat;

        public OpenIdRequest()
        {
            _authorizeUrlFormat = "{0}?client_id={1}&response_type={2}&scope={3}&redirect_uri={4}&state={5}&nonce={6}";
            _clientId = ConfigurationManager.AppSettings["_clientId"];
            _clientSecret = ConfigurationManager.AppSettings["_clientSecret"];
            _authorizeUri = ConfigurationManager.AppSettings["_authorizeUri"];
            _redirectUri = ConfigurationManager.AppSettings["_redirectUri"];
            _tokenUri = ConfigurationManager.AppSettings["_tokenUri"];
            _responseType = ConfigurationManager.AppSettings["_responseType"];
            _scope = ConfigurationManager.AppSettings["_scope"];
            _auth0Domain = ConfigurationManager.AppSettings["_auth0Domain"];
            _wellKnownUri = ConfigurationManager.AppSettings["_wellKnownUri"];
        }
        public string CreateAuthorizeRequest()
        {
            string nonce = Guid.NewGuid().ToString("N");
            string state = Guid.NewGuid().ToString("N");

            var tempAuthentication = new TempAuthentication();
            tempAuthentication.SetTempState(state, nonce);

            return string.Format(_authorizeUrlFormat,
                _authorizeUri,
                HttpUtility.UrlEncode(_clientId),
                HttpUtility.UrlEncode(_responseType),
                HttpUtility.UrlEncode(_scope),
                HttpUtility.UrlEncode(_redirectUri),
                HttpUtility.UrlEncode(state),
                HttpUtility.UrlEncode(nonce));
        }
        public async Task<TokenResponse> RequestToken(string code)
        {
            var credentials = string.Format("{0}:{1}", _clientId, _clientSecret);
            using (var client = new HttpClient())
            {
                //Define Headers
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials)));

                //Prepare Request Body
                List<KeyValuePair<string, string>> requestData = new List<KeyValuePair<string, string>>();
                requestData.Add(new KeyValuePair<string, string>("grant_type", "authorization_code"));
                requestData.Add(new KeyValuePair<string, string>("code", code));
                requestData.Add(new KeyValuePair<string, string>("redirect_uri", _redirectUri));

                FormUrlEncodedContent requestBody = new FormUrlEncodedContent(requestData);

                //Request Token
                var request = await client.PostAsync(_tokenUri, requestBody);
                var response = await request.Content.ReadAsStringAsync();

                return new TokenResponse(response); 
            }
        }
        public async Task ValidateResponseAndSignInAsync(TokenResponse response)
        {
            var tempAuthentication = new TempAuthentication();
            var tempState = await tempAuthentication.GetTempStateAsync();
            tempAuthentication.SignOut();

            if (!string.IsNullOrWhiteSpace(response.IdentityToken))
            {
                var claims = await ValidateTokenAsync(response.IdentityToken, tempState.Item2);
                
                if (!string.IsNullOrWhiteSpace(response.AccessToken))
                {
                    //claims.AddRange(await GetUserInfoClaimsAsync(response.AccessToken));

                    claims.Add(new Claim("access_token", response.AccessToken));
                    claims.Add(new Claim("expires_at", (DateTime.UtcNow.ToEpochTime() + response.ExpiresIn).ToDateTimeFromEpoch().ToString()));
                }

                if (!string.IsNullOrWhiteSpace(response.RefreshToken))
                {
                    claims.Add(new Claim("refresh_token", response.RefreshToken));
                }

                var id = new ClaimsIdentity(claims, "Cookies");
                HttpContext.Current.Request.GetOwinContext().Authentication.SignIn(id);
            }
        }

        private async Task<List<Claim>> ValidateTokenAsync(string token, string nonce)
        {
            IConfigurationManager<OpenIdConnectConfiguration> configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(_wellKnownUri, new OpenIdConnectConfigurationRetriever());
            OpenIdConnectConfiguration openIdConfig = await configurationManager.GetConfigurationAsync(CancellationToken.None);

            var validationParameters = new TokenValidationParameters
            {
                ValidIssuer = _auth0Domain,                
                ValidAudiences = new[] { _clientId },
                IssuerSigningKeys = openIdConfig.SigningKeys
            };

            SecurityToken jwt;
            var principal = new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out jwt);

            //validate nonce
            var nonceClaim = principal.FindFirst("nonce");

            if (!string.Equals(nonceClaim.Value, nonce, StringComparison.Ordinal))
            {
                throw new Exception("invalid nonce");
            }

            return principal.Claims.ToList();
        }
    }
}