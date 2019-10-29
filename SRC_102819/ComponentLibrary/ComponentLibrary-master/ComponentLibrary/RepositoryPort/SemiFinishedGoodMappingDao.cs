using System.Collections.Generic;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
	/// <summary>
	/// </summary>
	/// <seealso cref="TE.ComponentLibrary.ComponentLibrary.RepositoryPort.CompositeComponentMappingDao"/>
	public class SemiFinishedGoodMappingDao : CompositeComponentMappingDao
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SemiFinishedGoodMappingDao"/> class.
		/// </summary>
		public SemiFinishedGoodMappingDao()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SemiFinishedGoodMappingDao"/> class.
		/// </summary>
		/// <param name="mapping">The mapping.</param>
		public SemiFinishedGoodMappingDao(CompositeComponentMapping mapping) : base(mapping)
		{
		}
	}
}