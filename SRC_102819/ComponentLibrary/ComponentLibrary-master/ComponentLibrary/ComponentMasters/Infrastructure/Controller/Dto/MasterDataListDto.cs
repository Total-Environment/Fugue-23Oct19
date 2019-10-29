using System.Collections.Generic;
using System.Linq;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto
{
    /// <summary>
    /// Represents a master data list dto.
    /// </summary>
    public class MasterDataListDto
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public IEnumerable<string> Values { get; set; }
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterDataListDto"/> class.
        /// </summary>
        /// <param name="masterData">The master data.</param>
        public MasterDataListDto(MasterDataList masterData)
        {
            Name = masterData.Name;
            Values = masterData.Values.Select(v => v.Value);
            Id = masterData.Id;
        }

        /// <summary>
        /// Domains this instance.
        /// </summary>
        /// <returns></returns>
        public MasterDataList Domain()
        {
            var masterDataValues = Values.Select(v=>new MasterDataValue(v)).ToList();
            var masterDataList = new MasterDataList(Name, masterDataValues);
            return masterDataList;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterDataListDto"/> class.
        /// </summary>
        public MasterDataListDto()
        {
            
        }
    }
}