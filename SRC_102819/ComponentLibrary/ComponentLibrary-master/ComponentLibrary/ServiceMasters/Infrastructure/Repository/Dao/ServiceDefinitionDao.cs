using System.Linq;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository.Dao
{
    /// <summary>
    /// Represents a service definition dao.
    /// </summary>
    /// <seealso cref="Entity"/>
    public class ServiceDefinitionDao : ComponentDefinitionDao
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceDefinitionDao"/> class.
        /// </summary>
        public ServiceDefinitionDao()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceDefinitionDao"/> class.
        /// </summary>
        /// <param name="serviceDefinition">The service definition.</param>
        public ServiceDefinitionDao(IServiceDefinition serviceDefinition) : base(serviceDefinition)
        {
        }

        /// <inheritdoc/>
        public async Task<IServiceDefinition> GetDomain(IDataTypeFactory factory,
            IDependencyDefinitionRepository dependencyDefinitionRepository)
        {
            var tasks = Headers.Select(h => h.GetDomain(factory, dependencyDefinitionRepository));

            var headers = (await Task.WhenAll(tasks)).ToList();
            return new ServiceDefinition(Name)
            {
                Id = ObjectId.ToString(),
                Code = Code,
                Headers = headers
            };
        }
    }
}