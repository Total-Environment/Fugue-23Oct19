using System.Linq;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors
{
    /// <summary>
    /// Represents a material definition DAO.
    /// </summary>
    /// <seealso cref="Entity"/>
    public class MaterialDefinitionDao : ComponentDefinitionDao
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialDefinitionDao"/> class.
        /// </summary>
        public MaterialDefinitionDao()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialDefinitionDao"/> class.
        /// </summary>
        /// <param name="componentDefinition">The material definition.</param>
        public MaterialDefinitionDao(IMaterialDefinition componentDefinition) : base(componentDefinition)
        {
        }

        /// <summary>
        /// Gets the domain.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="dependencyDefinitionRepository">The dependency definition repository.</param>
        /// <returns></returns>
        public async Task<IMaterialDefinition> GetDomain(IDataTypeFactory factory,
            IDependencyDefinitionRepository dependencyDefinitionRepository)
        {
            var tasks = Headers.Select(h => h.GetDomain(factory, dependencyDefinitionRepository));

            var headers = (await Task.WhenAll(tasks)).ToList();
            return new MaterialDefinition(Name)
            {
                Id = ObjectId.ToString(),
                Code = Code,
                Headers = headers
            };
        }
    }
}