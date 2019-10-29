using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao
{
    /// <summary>
    /// Represents a component definition dao.
    /// </summary>
    /// <seealso cref="Entity" />
    public class ComponentDefinitionDao : Entity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentDefinitionDao"/> class.
        /// </summary>
        public ComponentDefinitionDao()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentDefinitionDao"/> class.
        /// </summary>
        /// <param name="componentDefinition">The material definition.</param>
        public ComponentDefinitionDao(IComponentDefinition componentDefinition)
        {
            SetDomain(componentDefinition);
        }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the headers.
        /// </summary>
        /// <value>
        /// The headers.
        /// </value>
        public List<HeaderDefinitionDto> Headers { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [BsonIgnore]
        public string Id
        {
            get { return ObjectId.ToString(); }
            set { SetObjectId(value); }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets the domain.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="dependencyDefinitionRepository">The dependency definition repository.</param>
        /// <returns></returns>
        public async Task<IComponentDefinition> GetDomain(IDataTypeFactory factory,
            IDependencyDefinitionRepository dependencyDefinitionRepository)
        {
            var tasks = Headers.Select(h => h.GetDomain(factory, dependencyDefinitionRepository));

            var headers = (await Task.WhenAll(tasks)).ToList();
            return new ComponentDefinition(Name)
            {
                Id = ObjectId.ToString(),
                Code = Code,
                Headers = headers
            };
        }

        /// <summary>
        /// Sets the domain.
        /// </summary>
        /// <param name="value">The value.</param>
        private void SetDomain(IComponentDefinition value)
        {
            ObjectId = SetObjectId(value.Id);
            Name = value.Name;
            Code = value.Code;
            Headers = value.Headers.Select(h => new HeaderDefinitionDto(h)).ToList();
        }
    }
}