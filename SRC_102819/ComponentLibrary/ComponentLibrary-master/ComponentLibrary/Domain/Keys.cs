namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
	/// <summary>
	/// Contains the set of constants used across the application.
	/// </summary>
	public static class Keys
	{
		/// <summary>
		/// Contains constants specific to classification definitions.
		/// </summary>
		public static class ClassificationKeys
		{
			/// <summary>
			/// The service classification colum prefix
			/// </summary>
			public const string ServiceClassificationColumPrefix = "service_level";

			/// <summary>
			/// The service classification key
			/// </summary>
			public const string ServiceClassificationKey = "service";

			/// <summary>
			/// The SFG classification colum prefix
			/// </summary>
			public const string SfgClassificationColumPrefix = "sfg_level";

			/// <summary>
			/// The SFG classification key
			/// </summary>
			public const string SfgClassificationKey = "sfg";

            /// <summary>
            /// The package classification colum prefix
            /// </summary>
		    public const string PackageClassificationColumPrefix = "pkg_level";

            /// <summary>
            /// The package classification key
            /// </summary>
            public const string PackageClassificationKey = "package";
		}

		/// <summary>
		/// Counter Collection Keys.
		/// </summary>
		public static class CounterCollections
		{
			/// <summary>
			/// The SFG collection key.
			/// </summary>
			public const string SfgKey = "SFG";

			/// <summary>
			/// The package key
			/// </summary>
			public const string PackageKey = "Package";
		}

		/// <summary>
		/// Contains constants specific to Sfg
		/// </summary>
		public static class Sfg
		{
			/// <summary>
			/// The SFG definition group
			/// </summary>
			public const string SfgDefinitionGroup = "Semi Finished Good";
		}

		/// <summary>
		/// Contains constants specific to Package
		/// </summary>
		public static class Package
		{
			/// <summary>
			/// The Pacakge definition group
			/// </summary>
			public const string PackageDefinitionGroup = "Package";
		}
	}
}