using System.Collections.Generic;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
	/// <summary>
	/// </summary>
	/// <seealso cref="TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao.Entity"/>
	public class CompositeComponentMappingDao : Entity
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CompositeComponentMappingDao"/> class.
		/// </summary>
		public CompositeComponentMappingDao()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CompositeComponentMappingDao"/> class.
		/// </summary>
		/// <param name="mapping">The mapping.</param>
		public CompositeComponentMappingDao(CompositeComponentMapping mapping)
		{
			ServiceColumnMapping = mapping.ServiceColumnMapping;
			GroupCodeMapping = mapping.GroupCodeMapping;
		}

		/// <summary>
		/// Gets or sets the group code mapping.
		/// </summary>
		/// <value>The group code mapping.</value>
		public Dictionary<string, string> GroupCodeMapping { get; set; }

		/// <summary>
		/// Gets or sets the service column mapping.
		/// </summary>
		/// <value>The service column mapping.</value>
		public Dictionary<string, Dictionary<string, string>> ServiceColumnMapping { get; set; }
	}
}