using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.DataAdaptors
{
    /// <summary>
    /// The data access object for Brand Definition
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao.Entity"/>
    public class BrandDefinitionDao : Entity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrandDefinitionDao"/> class.
        /// </summary>
        /// <param name="brandDefinition">The brand definition.</param>
        public BrandDefinitionDao(IBrandDefinition brandDefinition)
        {
            SetDomain(brandDefinition);
        }

        /// <inheritdoc/>
        [BsonIgnore]
        public string Id
        {
            get { return ObjectId.ToString(); }
            set { SetObjectId(value); }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>The columns.</value>
        public List<SimpleColumnDefinitionDto> Columns { get; set; }

        /// <summary>
        /// Sets the DAO given a brand definition
        /// </summary>
        /// <param name="brandDefinition">The brand definition.</param>
        public void SetDomain(IBrandDefinition brandDefinition)
        {
            ObjectId = SetObjectId(Id);
            Name = brandDefinition.Name;
            Columns = new List<SimpleColumnDefinitionDto>();
            foreach (var definitionColumn in brandDefinition.Columns)
            {
                var columnDefinitionDto = new SimpleColumnDefinitionDto(definitionColumn);
                Columns.Add(columnDefinitionDto);
            }
        }

        /// <summary>
        /// Gets a brand definition from a DAO
        /// </summary>
        /// <param name="simpleDataTypeFactory">The data type factory.</param>
        /// <returns></returns>
        public async Task<BrandDefinition> GetDomain(ISimpleDataTypeFactory simpleDataTypeFactory)
        {
            var task = Columns.Select(c => c.GetDomain(simpleDataTypeFactory));
            return new BrandDefinition(Name, (await Task.WhenAll(task)).ToList());
        }
    }
}