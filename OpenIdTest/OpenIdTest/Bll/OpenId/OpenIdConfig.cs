using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace OpenIdTest.Bll.OpenId
{
    public class OpenIdConfig
    {
        private static OpenIdConnectConfiguration settings = GetSettings();
        public static OpenIdConnectConfiguration Config
        {
            get
            {
                return settings;
            }
        }
        private static OpenIdConnectConfiguration GetSettings()
        {
            IConfigurationManager<OpenIdConnectConfiguration> configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(OpenIdSettings.Settings.WellKnownUri, new OpenIdConnectConfigurationRetriever());
            return configurationManager.GetConfigurationAsync(CancellationToken.None).Result;
        }
    }
}