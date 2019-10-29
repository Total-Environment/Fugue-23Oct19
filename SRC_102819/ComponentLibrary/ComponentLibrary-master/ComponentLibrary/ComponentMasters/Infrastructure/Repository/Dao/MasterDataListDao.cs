using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao
{
    /// <summary>
    /// Represents the DAO for Master Data
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao.Entity" />
    public class MasterDataListDao : Entity
    {
        /// <summary>
        /// Used primarily by rates to find location master data list.
        /// </summary>
        public const string Location = "location";

        /// <summary>
        /// Used primarily by rates to find type of purchase master data list.
        /// </summary>
        public const string TypeOfPurchase = "type_of_purchase";

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterDataListDao"/> class.
        /// </summary>
        public MasterDataListDao()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterDataListDao"/> class.
        /// </summary>
        /// <param name="dataList">The data list.</param>
        public MasterDataListDao(MasterDataList dataList)
        {
            MasterDataList = dataList;
        }

        /// <summary>
        /// Gets or sets the master data list.
        /// </summary>
        /// <value>
        /// The master data list.
        /// </value>
        [BsonIgnore]
        public MasterDataList MasterDataList
        {
            get
            {
                return new MasterDataList(Name, Values) { Id = ObjectId.ToString() };
            }
            set
            {
                ObjectId = SetObjectId(value.Id);
                Name = value.Name;
                Values = value.Values?.ToList();
            }
        }

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
        public List<MasterDataValue> Values { get; set; }
    }
}