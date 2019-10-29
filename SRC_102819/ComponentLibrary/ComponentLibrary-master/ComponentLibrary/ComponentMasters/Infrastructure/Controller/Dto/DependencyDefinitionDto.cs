using System;
using System.Collections.Generic;
using System.Linq;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto
{
	/// <summary>
	/// Represents a dependency definition dto.
	/// </summary>
	public class DependencyDefinitionDto
	{
		/// <summary>
		/// Gets or sets the blocks.
		/// </summary>
		/// <value>The blocks.</value>
		public IEnumerable<DependentBlockDto> Blocks { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="DependencyDefinitionDto"/> class.
		/// </summary>
		/// <param name="dependencyDefinition">The dependency definition.</param>
		/// <exception cref="ArgumentException">Domain Object is null.</exception>
		public DependencyDefinitionDto(DependencyDefinition dependencyDefinition)
		{
			if (dependencyDefinition == null)
				throw new ArgumentException("Domain Object is null.");
			Name = dependencyDefinition.Name;
			ColumnList = dependencyDefinition.ColumnList;
			Blocks = dependencyDefinition.Blocks.Select(d => new DependentBlockDto(d));
			Id = dependencyDefinition.Id;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DependencyDefinitionDto"/> class.
		/// </summary>
		public DependencyDefinitionDto()
		{
		}

		/// <summary>
		/// Domains this instance.
		/// </summary>
		/// <returns></returns>
		public DependencyDefinition Domain()
		{
			IEnumerable<DependentBlock> dependentBlocks = null;
			if (Blocks != null)
			{
				dependentBlocks = Blocks.Select(b => b.Domain());
			}
			return new DependencyDefinition(Name, ColumnList, dependentBlocks) { Id = Id };
		}

		/// <summary>
		/// Gets or sets the column list.
		/// </summary>
		/// <value>The column list.</value>
		public IEnumerable<string> ColumnList { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		public string Id { get; set; }
	}
}