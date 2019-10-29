using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using Newtonsoft.Json;

namespace TE.ComponentLibrary.ComponentLibrary.Utils
{
	/// <summary>
	/// </summary>
	public class PermissionHelper
	{
        private static readonly string[] EmptyArray = new string[0];
        /// <summary>
        /// Gets the pemissions.
        /// </summary>
        /// <param name="groups">The groups.</param>
        /// <returns></returns>
        public List<string> GetPemissions(List<string> groups)
		{
			string groupPermissionsText;
			var assembly = Assembly.GetExecutingAssembly();
			using (
				var stream =
					assembly.GetManifestResourceStream("TE.ComponentLibrary.ComponentLibrary.Utils.GroupPermissions.json"))
			{
				using (var streamReader = new StreamReader(stream))
				{
					groupPermissionsText = streamReader.ReadToEnd();
				}
			}
			var groupPermissions = JsonConvert.DeserializeObject<List<GroupPermission>>(groupPermissionsText);

			var query = from @group in groups
						from groupPermission in groupPermissions
						where @group == groupPermission.Group.Name
						from permission in groupPermission.Permissions
						select permission.Name;

			return query.Distinct().ToList();
		}

		/// <summary>
		/// Gets the client permissions.
		/// </summary>
		/// <returns></returns>
		public List<string> GetClientPermissions()
		{
			string clientId = null;
			var appid = ClaimsPrincipal.Current.Claims.FirstOrDefault(cl => cl.Type == Globals.AppIdClaimType);
			if (appid != null)
			{
				clientId = appid.Value;
			}

			string clientName = null;
			string clientPermissionsText = null;
		    var clientsFromConifg = ConfigurationManager.AppSettings["CLIENTS"];
		    var splitstrings = SplitString(clientsFromConifg);
		    Dictionary<string, string> clients = splitstrings.ToDictionary(s => s, s => ConfigurationManager.AppSettings[s]);
			var client = clients.FirstOrDefault(c => c.Value == clientId);
			var assembly = Assembly.GetExecutingAssembly();
			clientName = client.Key;
			using (var stream =
				assembly.GetManifestResourceStream("TE.ComponentLibrary.ComponentLibrary.Utils.ClientPermissions.json"))
			{
				if (stream != null)
					using (var streamReader = new StreamReader(stream))
					{
						clientPermissionsText = streamReader.ReadToEnd();
					}
			}
			var clientPermissions = JsonConvert.DeserializeObject<List<ClientPermission>>(clientPermissionsText);

			var query =
						from groupPermission in clientPermissions
						where clientName == groupPermission.Client.Name
						from permission in groupPermission.Permissions
						select permission.Name;

			return query.Distinct().ToList();
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
    }
}