using System.Collections.Generic;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
	/// <summary>
	/// Represents an interface for dependency definition.
	/// </summary>
	public interface IDependencyDefinition
	{
		/// <summary>
		/// Gets the blocks.
		/// </summary>
		/// <value>The blocks.</value>
		IEnumerable<DependentBlock> Blocks { get; }

		/// <summary>
		/// Gets the column list.
		/// </summary>
		/// <value>The column list.</value>
		List<string> ColumnList { get; }

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		string Name { get; }

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		string Id { get; set; }

		/// <summary>
		/// Validates the specified block.
		/// </summary>
		/// <param name="block">The block.</param>
		/// <returns></returns>
		bool Validate(string[] block);

		/// <summary>
		/// Validates the specified block.
		/// </summary>
		/// <param name="block">The block.</param>
		/// <returns></returns>
		bool Validate(IEnumerable<string> block);

		/// <summary>
		/// Validates the specified block.
		/// </summary>
		/// <param name="block">The block.</param>
		/// <returns></returns>
		bool Validate(IDictionary<string, string> block);
	}
}