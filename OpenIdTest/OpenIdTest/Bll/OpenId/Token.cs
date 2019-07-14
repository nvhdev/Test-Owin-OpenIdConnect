using IdentityModel;
using IdentityModel.Client;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenIdTest.Bll.OpenId
{
    public class Token
    {
        public async Task<List<Claim>> RequestAndValidateTokenAsync(string code)
        {
            List<Claim> claims = null;
            // Request
            var response = await RequestTokenAsync(code);
            
            // Validate
            var tempAuthentication = new TempAuthentication();
            var tempState = await tempAuthentication.GetTempStateAsync();
            tempAuthentication.SignOut();

            if (!string.IsNullOrWhiteSpace(response.IdentityToken))
            {
                claims = ValidateToken(response.IdentityToken, tempState.Item2);

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
            }
            return claims;
        }

        private async Task<TokenResponse> RequestTokenAsync(string code)
        {
            var credentials = string.Format("{0}:{1}", OpenIdSettings.Settings.ClientId, OpenIdSettings.Settings.ClientSecret);
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
                requestData.Add(new KeyValuePair<string, string>("redirect_uri", OpenIdSettings.Settings.RedirectUri));

                FormUrlEncodedContent requestBody = new FormUrlEncodedContent(requestData);

                //Request Token
                var request = await client.PostAsync(OpenIdConfig.Config.TokenEndpoint, requestBody);
                var response = await request.Content.ReadAsStringAsync();

                return new TokenResponse(response);
            }
        }
        private List<Claim> ValidateToken(string token, string nonce)
        {
            
            var validationParameters = new TokenValidationParameters
            {
                ValidIssuer = OpenIdConfig.Config.Issuer,
                ValidAudiences = new[] { OpenIdSettings.Settings.ClientId },
                IssuerSigningKeys = OpenIdConfig.Config.SigningKeys
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