using System;
using System.Configuration;
using System.Security.Claims;

namespace TE.ComponentLibrary.ComponentLibrary.Utils
{
    /// <summary>
    /// Represents user helper
    /// </summary>
    public class UserHelper
    {
        /// <summary>
        /// Gets the user information.
        /// </summary>
        /// <returns></returns>
        public User GetUserInformation()
        {
            var user = new User
            {
                Email = ClaimsPrincipal.Current.FindFirst(Globals.AzureAadEmailClaimType).Value,
                Name = ClaimsPrincipal.Current.FindFirst(Globals.B2CAadUserNameClaimType).Value,
                ObjectId = ClaimsPrincipal.Current.FindFirst(Globals.ObjectIdClaimType).Value
            };
            var groups = new GroupHelper().GetGroups();
            var permission = new PermissionHelper().GetPemissions(groups);
            user.Permissions = permission;
            user.Groups = groups;
            return user;
        }
    }
}