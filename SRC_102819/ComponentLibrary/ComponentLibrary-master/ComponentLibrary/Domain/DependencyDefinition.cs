using System;
using System.Collections.Generic;
using System.Linq;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
	/// <summary>
	/// Represents a dependency definition.
	/// </summary>
	/// <seealso cref="IDependencyDefinition"/>
	public class DependencyDefinition : IDependencyDefinition
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DependencyDefinition"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="columnList">The column list.</param>
		/// <param name="blocks">The blocks.</param>
		/// <exception cref="System.ArgumentException">
		/// Name cannot be null or whitespace. or Column list cannot be null or empty.
		/// </exception>
		public DependencyDefinition(string name, IEnumerable<string> columnList,
			IEnumerable<DependentBlock> blocks = null)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Name cannot be null or whitespace.");

			var enumerable = columnList as IList<string> ?? columnList.ToList();
			if (columnList == null || !enumerable.Any())
				throw new ArgumentException("Column list cannot be null or empty.");

			ValidateBlock(blocks, enumerable);
			ColumnList = enumerable.ToList();
			Name = name;
			Blocks = blocks ?? new List<DependentBlock>();
		}

		/// <inheritdoc/>
		public List<string> ColumnList { get; }

		/// <inheritdoc/>
		public string Name { get; }

		/// <inheritdoc/>
		public IEnumerable<DependentBlock> Blocks { get; }

		/// <inheritdoc/>
		public bool Validate(string[] block)
		{
			return block.Length == ColumnList.Count()
				   && Blocks.Any(b => b.Validate(block));
		}

		/// <inheritdoc/>
		public bool Validate(IEnumerable<string> block)
		{
			return Validate(block.ToArray());
		}

		/// <inheritdoc/>
		public bool Validate(IDictionary<string, string> block)
		{
			try
			{
				return Validate(ColumnList.Select(c => block[c]).ToArray());
			}
			catch (KeyNotFoundException)
			{
				throw new FormatException("Dependecy data is wrong, probably you are missing a value.");
			}
		}

		/// <inheritdoc/>
		private void ValidateBlock(IEnumerable<DependentBlock> blocks, IEnumerable<string> columnList)
		{
			if (blocks != null && blocks.Any(b => b.Data.Length != columnList.Count()))
				throw new ArgumentException("Length of blocks should be equal to that of list of columns.");
		}

		/// <inheritdoc/>
		public string Id { get; set; }
	}
}