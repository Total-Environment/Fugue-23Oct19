using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao
{
	/// <summary>
	/// Represents a composite component definition dao.
	/// </summary>
	/// <seealso cref="Entity"/>
	public class CostPriceRatioDefinitionDao : Entity
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CostPriceRatioDefinitionDao"/> class.
		/// </summary>
		public CostPriceRatioDefinitionDao()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CostPriceRatioDefinitionDao"/> class.
		/// </summary>
		/// <param name="componentDefinition">The material definition.</param>
		public CostPriceRatioDefinitionDao(CostPriceRatioDefinition componentDefinition)
		{
			SetDomain(componentDefinition);
		}

		/// <summary>
		/// Gets or sets the columns.
		/// </summary>
		/// <value>The columns.</value>
		public List<SimpleColumnDefinitionDto> Columns { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }

		/// <summary>
		/// Gets the domain.
		/// </summary>
		/// <param name="factory">The factory.</param>
		/// <returns></returns>
		public async Task<CostPriceRatioDefinition> GetDomain(ISimpleDataTypeFactory factory)
		{
			var tasks = Columns.Select(h => h.GetDomain(factory));

			var columns = (await Task.WhenAll(tasks)).ToList();
			return new CostPriceRatioDefinition(Name, columns);
		}

		/// <summary>
		/// Sets the domain.
		/// </summary>
		/// <param name="value">The value.</param>
		private void SetDomain(CostPriceRatioDefinition value)
		{
			Name = value.Name;
			Columns = value.Columns.Select(h => new SimpleColumnDefinitionDto(h)).ToList();
		}
	}
}