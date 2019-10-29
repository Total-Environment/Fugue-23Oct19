using System.Linq;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.DataAdaptors
{
    /// <summary>
    /// Represents the DAO for Asset Definition
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao.ComponentDefinitionDao" />
    public class AssetDefinitionDao : ComponentDefinitionDao
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssetDefinitionDao"/> class.
        /// </summary>
        /// <param name="assetDefinition">The asset definition.</param>
        public AssetDefinitionDao(IComponentDefinition assetDefinition) : base(assetDefinition)
        {
        }

        /// <inheritdoc/>
        public async Task<AssetDefinition> GetDomain(IDataTypeFactory factory,
            IDependencyDefinitionRepository dependencyDefinitionRepository)
        {
            return new AssetDefinition(Name)
            {
                Code = Code,
                Id = Id,
                Headers =
                    (await Task.WhenAll(Headers.Select(h => h.GetDomain(factory, dependencyDefinitionRepository))))
                    .ToList()
            };
        }
    }
}