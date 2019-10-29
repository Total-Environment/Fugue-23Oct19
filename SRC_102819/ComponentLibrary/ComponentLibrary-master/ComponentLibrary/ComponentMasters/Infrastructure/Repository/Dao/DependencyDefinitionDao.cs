using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao
{
	/// <summary>
	/// Represents the Dependency Definition as DAO
	/// </summary>
	/// <seealso cref="TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao.Entity"/>
	public class DependencyDefinitionDao : Entity
	{
		/// <summary>
		/// Gets or sets the column list.
		/// </summary>
		/// <value>The column list.</value>
		public List<string> ColumnList { get; set; }

		/// <summary>
		/// Gets or sets the dependent blocks.
		/// </summary>
		/// <value>The dependent blocks.</value>
		public IEnumerable<DependentBlockDao> DependentBlocks { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		[BsonIgnore]
		public string Id
		{
			get { return ObjectId.ToString(); }
			set { SetObjectId(value); }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DependencyDefinitionDao"/> class.
		/// </summary>
		/// <param name="dependencyDefinition">The dependency definition.</param>
		public DependencyDefinitionDao(IDependencyDefinition dependencyDefinition)
		{
			ColumnList = dependencyDefinition.ColumnList;
			DependentBlocks = dependencyDefinition.Blocks.Select(d => new DependentBlockDao(d));
			Name = dependencyDefinition.Name;
			ObjectId = SetObjectId(dependencyDefinition.Id);
		}

		/// <summary>
		/// Domains this instance.
		/// </summary>
		/// <returns></returns>
		public DependencyDefinition Domain()
		{
			return new DependencyDefinition(Name, ColumnList, DependentBlocks.Select(d => d.Domain())) { Id = ObjectId.ToString() };
		}
	}
}