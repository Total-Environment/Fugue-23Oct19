using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace TE.ComponentLibrary.ComponentLibrary.Helpers
{
    /// <summary>
    /// Claims Helper.
    /// </summary>
    public class ClaimsHelper
    {
        /// <summary>
        /// Fetch active logged in user's full name.
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentUserFullName()
        {
            ClaimsPrincipal cp = ClaimsPrincipal.Current;
            var firstName = cp.FindFirst(ClaimTypes.GivenName);
            var lastName = cp.FindFirst(ClaimTypes.Surname);
            var userFullName = "TE";
            if (firstName != null && lastName != null)
            {
                userFullName = $"{firstName.Value} {lastName.Value}";
            }
            return userFullName;
        }
    }
}