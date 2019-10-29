using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using log4net;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using TE.ComponentLibrary.ComponentLibrary.ControllerPort;

namespace TE.ComponentLibrary.ComponentLibrary.Utils
{
	/// <summary>
	/// </summary>
	public class GraphHelper
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(GraphHelper));
		private readonly string _graphClientId;
		private readonly string _graphClientKey;
		private readonly string _tenant;

		/// <summary>
		/// Initializes a new instance of the <see cref="GraphHelper"/> class.
		/// </summary>
		/// <param name="tenant">The tenant.</param>
		/// <param name="graphClientId">The graph client identifier.</param>
		/// <param name="graphClientKey">The graph client key.</param>
		public GraphHelper(string tenant, string graphClientId, string graphClientKey)
		{
			_tenant = tenant;
			_graphClientId = graphClientId;
			_graphClientKey = graphClientKey;
		}

		/// <summary>
		/// Acquires the token.
		/// </summary>
		/// <returns></returns>
		private async Task<string> AcquireToken()
		{
			var cred = new ClientCredential(_graphClientId, _graphClientKey);
			var authContext =
				new AuthenticationContext(string.Format(CultureInfo.InvariantCulture,
					ConfigurationManager.AppSettings["AADInstance"],
					_tenant), new TokenCache());
			var result = await authContext.AcquireTokenAsync(ConfigurationManager.AppSettings["GraphUrl"], cred);
			return result.AccessToken;
		}

		/// <summary>
		/// Gets the groups.
		/// </summary>
		/// <returns></returns>
		public List<string> GetGroups()
		{
			var groups = new List<string>();
			var graphClient =
				new ActiveDirectoryClient(
					new Uri(ConfigurationManager.AppSettings["GraphUrl"] + '/' + _tenant),
					async () => await AcquireToken());

			var oidClaim = ClaimsPrincipal.Current.FindFirst(Globals.ObjectIdClaimType);
			if (!string.IsNullOrWhiteSpace(oidClaim?.Value))
			{
				try
				{
					var pagedCollection = graphClient.Users.GetByObjectId(oidClaim.Value).MemberOf.ExecuteAsync().Result;

					do
					{
						var directoryObjects = pagedCollection.CurrentPage.ToList();
						foreach (var directoryObject in directoryObjects)
						{
							var group = directoryObject as Microsoft.Azure.ActiveDirectory.GraphClient.Group;
							if (group != null)
								groups.Add(group.DisplayName);
						}
						pagedCollection = pagedCollection.MorePagesAvailable ? pagedCollection.GetNextPageAsync().Result : null;
					} while (pagedCollection != null);
				}
				catch (Exception exception)
				{
					Log.ErrorFormat("[object Id = {0}] No groups Found might be client credential flow.", oidClaim.Value);
				}
			}
			return groups;
		}
	}
}