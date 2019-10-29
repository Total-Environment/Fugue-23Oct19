using System.Linq;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository.Dao;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
	/// <summary>
	/// </summary>
	/// <seealso cref="TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao.CompositeComponentDefinitionDao"/>
	public class SemiFinishedGoodDefinitionDao : CompositeComponentDefinitionDao
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceDefinitionDao"/> class.
		/// </summary>
		public SemiFinishedGoodDefinitionDao()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceDefinitionDao"/> class.
		/// </summary>
		/// <param name="serviceDefinition">The service definition.</param>
		public SemiFinishedGoodDefinitionDao(ICompositeComponentDefinition serviceDefinition) : base(serviceDefinition)
		{
		}
	}
}