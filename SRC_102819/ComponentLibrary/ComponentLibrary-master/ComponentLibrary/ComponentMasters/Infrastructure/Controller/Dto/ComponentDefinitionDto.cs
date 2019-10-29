using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto
{
    /// <summary>
    /// Dto for ComponentDefinition
    /// </summary>
    public abstract class ComponentDefinitionDto
    {
        /// <summary>
        /// Component Identifer
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Component Code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Component Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of component headers
        /// </summary>
        public List<HeaderDefinitionDto> Headers { get; set; }

        /// <summary>
        /// Constructor for ComponentDefinitionDto
        /// </summary>
        /// <param name="componentDefinition"></param>
        protected ComponentDefinitionDto(IComponentDefinition componentDefinition)
        {
            Id = componentDefinition.Id;
            Code = componentDefinition.Code;
            Name = componentDefinition.Name;
            Headers = componentDefinition.Headers.Select(h => new HeaderDefinitionDto(h)).ToList();
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected ComponentDefinitionDto()
        { }

        /// <summary>
        /// Gets the domain.
        /// </summary>
        /// <param name="dependencyDefinitionRepository">The dependency definition repository.</param>
        /// <param name="dataTypeFactory">The data type factory.</param>
        /// <returns></returns>
        public abstract Task<IComponentDefinition> GetDomain(IDependencyDefinitionRepository dependencyDefinitionRepository,
            IDataTypeFactory dataTypeFactory);
    }
}