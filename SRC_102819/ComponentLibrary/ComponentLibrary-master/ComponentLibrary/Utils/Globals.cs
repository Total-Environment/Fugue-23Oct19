namespace TE.ComponentLibrary.ComponentLibrary.Utils
{
	/// <summary>
	/// </summary>
	public static class Globals
	{
		private const string objectIdClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
		private const string tenantIdClaimType = "http://schemas.microsoft.com/identity/claims/tenantid";
		private const string surnameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname";
		private const string givennameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname";
		private const string issuerClaimType = "iss";
		private const string b2CAadEmailClaimType = "emails";
		private const string b2CAadUserNameClaimType = "name";
		private const string azureAadEmailClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
		private const string azureAadUserNameClaimType = "name";
		private const string appIdClaimType = "appid";

		/// <summary>
		/// Gets the type of the object identifier claim.
		/// </summary>
		/// <value>The type of the object identifier claim.</value>
		public static string ObjectIdClaimType => objectIdClaimType;

		/// <summary>
		/// Gets the type of the tenant identifier claim.
		/// </summary>
		/// <value>The type of the tenant identifier claim.</value>
		public static string TenantIdClaimType => tenantIdClaimType;

		/// <summary>
		/// Gets the type of the surname claim.
		/// </summary>
		/// <value>The type of the surname claim.</value>
		public static string SurnameClaimType => surnameClaimType;

		/// <summary>
		/// Gets the type of the givenname claim.
		/// </summary>
		/// <value>The type of the givenname claim.</value>
		public static string GivennameClaimType => givennameClaimType;

		/// <summary>
		/// Gets the type of the issuer claim.
		/// </summary>
		/// <value>The type of the issuer claim.</value>
		public static string IssuerClaimType => issuerClaimType;

		/// <summary>
		/// Gets the type of the b2 c aad email claim.
		/// </summary>
		/// <value>The type of the b2 c aad email claim.</value>
		public static string B2CAadEmailClaimType => b2CAadEmailClaimType;

		/// <summary>
		/// Gets the type of the b2 c aad user name claim.
		/// </summary>
		/// <value>The type of the b2 c aad user name claim.</value>
		public static string B2CAadUserNameClaimType => b2CAadUserNameClaimType;

		/// <summary>
		/// Gets the type of the azure aad email claim.
		/// </summary>
		/// <value>The type of the azure aad email claim.</value>
		public static string AzureAadEmailClaimType => azureAadEmailClaimType;

		/// <summary>
		/// Gets the type of the azure aad user name claim.
		/// </summary>
		/// <value>The type of the azure aad user name claim.</value>
		public static string AzureAadUserNameClaimType => azureAadUserNameClaimType;

		/// <summary>
		/// Gets the type of the application identifier claim.
		/// </summary>
		/// <value>The type of the application identifier claim.</value>
		public static string AppIdClaimType => appIdClaimType;
	}
}