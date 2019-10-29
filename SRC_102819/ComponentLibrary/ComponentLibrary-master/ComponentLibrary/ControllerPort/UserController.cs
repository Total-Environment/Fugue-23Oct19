using System.Web.Http;
using TE.ComponentLibrary.ComponentLibrary.App_Start;
using TE.ComponentLibrary.ComponentLibrary.Utils;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerPort
{
	/// <summary>
	/// </summary>
	/// <seealso cref="System.Web.Http.ApiController"/>
	public class UserController : ApiController
	{
        /// <summary>
        /// Gets the groups.
        /// </summary>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Get List of User Groups")]
        [HttpGet]
		[Route("groups")]
		public IHttpActionResult GetGroups()
		{
            var groups = new GroupHelper().GetGroups();
			return Ok(groups);
		}

        /// <summary>
        /// Gets the permissions.
        /// </summary>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Get List of User Permissions")]
        [HttpGet]
		[Route("permissions")]
		public IHttpActionResult GetPermissions()
		{
			var groups = new GroupHelper().GetGroups();
			var permissions = new PermissionHelper().GetPemissions(groups);
			return Ok(permissions);
		}

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Get User Information")]
        [HttpGet]
	    [Route("user")]
	    public IHttpActionResult GetUser()
        {
            var userInformation = new UserHelper().GetUserInformation();
            return Ok(userInformation);
        }
	}
}