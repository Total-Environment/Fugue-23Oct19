using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TE.ComponentLibrary.ComponentLibrary.Utils
{
	/// <summary>
	/// </summary>
	public class ClientPermission
	{
		/// <summary>
		/// Gets or sets the group.
		/// </summary>
		/// <value>The group.</value>
		public Client Client { get; set; }

		/// <summary>
		/// Gets or sets the permissions.
		/// </summary>
		/// <value>The permissions.</value>
		public List<Permission> Permissions { get; set; }
	}
}