using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs
{
    /// <summary>
    /// The Data Transfer Object for Brand Definition
    /// </summary>
    public class BrandDefinitionDto
    {
        /// <summary>
        /// Constructor to initialise the brand
        /// </summary>
        /// <param name="name">The name of the Brand Definition</param>
        /// <param name="columns">The columns associated with it</param>
        [JsonConstructor]
        public BrandDefinitionDto(string name, List<SimpleColumnDefinitionDto> columns)
        {
            Columns = columns;
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrandDefinitionDto"/> class.
        /// </summary>
        /// <param name="definition">The definition.</param>
        public BrandDefinitionDto(IBrandDefinition definition)
        {
            Columns = new List<SimpleColumnDefinitionDto>();
            foreach (var definitionColumn in definition.Columns)
            {
                var columnDefinitionDto = new SimpleColumnDefinitionDto(definitionColumn);
                Columns.Add(columnDefinitionDto);
            }
            Name = definition.Name;
        }

        /// <summary>
        /// Gets the name of the brand definition
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }

        /// <summary>
        /// Column Definitions that define a brand
        /// </summary>
        public List<SimpleColumnDefinitionDto> Columns { get; }

        /// <summary>
        /// Converts the DTO to a Brand Definition Object
        /// </summary>
        /// <param name="dataTypeFactory">The data type factory.</param>
        /// <returns></returns>
        public async Task<BrandDefinition> GetDomain(ISimpleDataTypeFactory dataTypeFactory)
        {
            var task = Columns.Select(c => c.GetDomain(dataTypeFactory));
            var columnDefinitions = (await Task.WhenAll(task)).ToList();
            var brandDefinition = new BrandDefinition(Name, columnDefinitions);
            return brandDefinition;
        }
    }
}