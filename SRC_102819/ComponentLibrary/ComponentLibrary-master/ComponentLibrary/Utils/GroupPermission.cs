using System.Collections.Generic;

namespace TE.ComponentLibrary.ComponentLibrary.Utils
{
	/// <summary>
	/// </summary>
	public class GroupPermission
	{
		/// <summary>
		/// Gets or sets the group.
		/// </summary>
		/// <value>The group.</value>
		public Group Group { get; set; }

		/// <summary>
		/// Gets or sets the permissions.
		/// </summary>
		/// <value>The permissions.</value>
		public List<Permission> Permissions { get; set; }
	}
}