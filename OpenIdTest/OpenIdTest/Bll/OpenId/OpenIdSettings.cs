using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace OpenIdTest.Bll.OpenId
{
    public class OpenIdSettings : ConfigurationSection
    {
        private static OpenIdSettings settings = ConfigurationManager.GetSection("OpenIdSettings") as OpenIdSettings;

        public static OpenIdSettings Settings
        {
            get
            {
                return settings;
            }
        }
        [ConfigurationProperty("clientid", IsRequired = true)]
        public string ClientId {
            get { return (string)this["clientid"]; }
            set { this["clientid"] = value; }
        }
        [ConfigurationProperty("clientsecret", IsRequired = true)]
        public string ClientSecret
        {
            get { return (string)this["clientsecret"]; }
            set { this["clientsecret"] = value; }
        }
        [ConfigurationProperty("responsetype", IsRequired = true)]
        public string ResponseType
        {
            get { return (string)this["responsetype"]; }
            set { this["responsetype"] = value; }
        }
        [ConfigurationProperty("scope", IsRequired = true)]
        public string Scope
        {
            get { return (string)this["scope"]; }
            set { this["scope"] = value; }
        }
        [ConfigurationProperty("redirecturi", IsRequired = true)]
        public string RedirectUri
        {
            get { return (string)this["redirecturi"]; }
            set { this["redirecturi"] = value; }
        }

        [ConfigurationProperty("wellknownuri", IsRequired = true)]
        public string WellKnownUri
        {
            get { return (string)this["wellknownuri"]; }
            set { this["wellknownuri"] = value; }
        }

        public string AuthorizeUrlFormat { get; private set; }

        public OpenIdSettings()
        {
            AuthorizeUrlFormat = "{0}?client_id={1}&response_type={2}&scope={3}&redirect_uri={4}&state={5}&nonce={6}";
        }
    }
}