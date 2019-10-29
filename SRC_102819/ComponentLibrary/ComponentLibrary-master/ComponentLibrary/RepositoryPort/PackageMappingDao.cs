using System.Collections.Generic;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
	/// <summary>
	/// </summary>
	/// <seealso cref="TE.ComponentLibrary.ComponentLibrary.RepositoryPort.CompositeComponentMappingDao"/>
	public class PackageMappingDao : CompositeComponentMappingDao
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PackageMappingDao"/> class.
		/// </summary>
		public PackageMappingDao()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PackageMappingDao"/> class.
		/// </summary>
		/// <param name="mapping">The mapping.</param>
		public PackageMappingDao(CompositeComponentMapping mapping) : base(mapping)
		{
		}
	}
}