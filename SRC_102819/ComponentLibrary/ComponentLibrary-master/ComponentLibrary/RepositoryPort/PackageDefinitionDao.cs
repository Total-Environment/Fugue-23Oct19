using System.Linq;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository.Dao;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
	/// <summary>
	/// </summary>
	/// <seealso cref="TE.ComponentLibrary.ComponentLibrary.RepositoryPort.CompositeComponentDefinitionDao"/>
	public class PackageDefinitionDao : CompositeComponentDefinitionDao
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PackageDefinitionDao"/> class.
		/// </summary>
		public PackageDefinitionDao()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PackageDefinitionDao"/> class.
		/// </summary>
		/// <param name="serviceDefinition">The service definition.</param>
		public PackageDefinitionDao(ICompositeComponentDefinition serviceDefinition) : base(serviceDefinition)
		{
		}
	}
}