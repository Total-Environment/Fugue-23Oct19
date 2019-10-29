using System.Linq;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs
{
    /// <summary>
    /// Dto for Asset Definition.
    /// </summary>
    public class AssetDefinitionDto : ComponentDefinitionDto
    {
        /// <summary>
        /// Constructor for Asset definition Dto.
        /// </summary>
        /// <param name="assetdefinition"></param>
        public AssetDefinitionDto(AssetDefinition assetdefinition) : base(assetdefinition)
        {
        }

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public AssetDefinitionDto()
        {
        }

        /// <summary>
        /// Return Domain object.
        /// </summary>
        /// <param name="dependencyDefinitionRepository"></param>
        /// <param name="dataTypeFactory"></param>
        /// <returns></returns>
        public override async Task<IComponentDefinition> GetDomain(
            IDependencyDefinitionRepository dependencyDefinitionRepository,
            IDataTypeFactory dataTypeFactory)
        {
            var tasks = Headers.Select(h => h.GetDomain(dataTypeFactory, dependencyDefinitionRepository));
            return new AssetDefinition(Name)
            {
                Code = Code,
                Id = Id,
                Headers = (await Task.WhenAll(tasks)).ToList()
            };
        }
    }
}