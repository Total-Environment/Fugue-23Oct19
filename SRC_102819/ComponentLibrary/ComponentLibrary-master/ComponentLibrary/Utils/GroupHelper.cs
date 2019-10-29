using System.Collections.Generic;
using System.Configuration;
using log4net;

namespace TE.ComponentLibrary.ComponentLibrary.Utils
{
	/// <summary>
	/// </summary>
	public class GroupHelper
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(GroupHelper));

		/// <summary>
		/// Gets the groups.
		/// </summary>
		/// <returns></returns>
		public List<string> GetGroups()
		{
			GraphHelper graphHelper;
			graphHelper = new GraphHelper(ConfigurationManager.AppSettings["Tenant"],
				ConfigurationManager.AppSettings["GraphClientId"], ConfigurationManager.AppSettings["GraphClientKey"]);
			var groups = graphHelper.GetGroups();
			return groups;
		}
	}
}