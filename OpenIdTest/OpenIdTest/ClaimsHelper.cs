using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace OpenIdTest
{
    public static class ClaimsHelper
    {
        public static bool CanDoX(this IPrincipal principal, string claimType, string claimValue)
        {
            return ((ClaimsIdentity)principal.Identity).HasClaim(claimType, claimValue);
        }
    }
}