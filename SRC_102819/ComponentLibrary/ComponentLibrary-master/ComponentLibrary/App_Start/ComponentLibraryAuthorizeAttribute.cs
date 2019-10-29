using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using log4net;
using TE.ComponentLibrary.ComponentLibrary.Utils;

namespace TE.ComponentLibrary.ComponentLibrary.App_Start
{
	public class ComponentLibraryAuthorizeAttribute : AuthorizeAttribute
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(ComponentLibraryAuthorizeAttribute));

		private static readonly string[] EmptyArray = new string[0];
		private string _permissions;
		private string[] _permissionsSplit = EmptyArray;
		private string _clientsId;

		/// <summary>
		/// Gets or sets the permissions.
		/// </summary>
		/// <value>The permissions.</value>
		public string Permissions
		{
			get { return _permissions ?? string.Empty; }
			set
			{
				_permissions = value;
				_permissionsSplit = SplitString(value);
			}
		}

		private static string[] SplitString(string original)
		{
			if (string.IsNullOrEmpty(original))
				return EmptyArray;

			var split = from piece in original.Split(',')
						let trimmed = piece.Trim()
						where !string.IsNullOrEmpty(trimmed)
						select trimmed;
			return split.ToArray();
		}

		/// <summary>
		/// Indicates whether the specified control is authorized.
		/// </summary>
		/// <param name="actionContext">The context.</param>
		/// <returns>true if the control is authorized; otherwise, false.</returns>
		protected override bool IsAuthorized(HttpActionContext actionContext)
		{
			if (Convert.ToBoolean(ConfigurationManager.AppSettings["ENABLE_AUTH"]) == false)
				return true;

			var claimsPrincipal = ClaimsPrincipal.Current;
			if (claimsPrincipal?.Identity == null)
				return false;

			Log.DebugFormat("IsAuthenticated:{0}", claimsPrincipal.Identity.IsAuthenticated);
			if (!claimsPrincipal.Identity.IsAuthenticated)
				return false;

			Log.DebugFormat("PermissionsSplit:{0}", string.Join(",", _permissionsSplit));

			if (_permissionsSplit.Length <= 0) return true;

			if (_permissionsSplit.Length > 0)
			{
				var groups = new GroupHelper().GetGroups();
				Log.DebugFormat("Groups:{0}", string.Join(",", groups));
				var permissions = new PermissionHelper().GetPemissions(groups);
				permissions.AddRange(new PermissionHelper().GetClientPermissions());
				permissions = permissions.Distinct().ToList();
				Log.DebugFormat("Permissions:{0}", string.Join(",", permissions));

				if (_permissionsSplit.Any(p => permissions.Contains(p)))
					return true;
				return false;
			}
			return true;
		}
	}
}