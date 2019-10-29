using System.Collections.Generic;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
	/// <summary>
	/// </summary>
	public interface ICompositeComponentDefinition
	{
		/// <summary>
		/// Gets or sets the code.
		/// </summary>
		/// <value>The code.</value>
		string Code { get; set; }

		/// <summary>
		/// Gets or sets the headers.
		/// </summary>
		/// <value>The headers.</value>
		List<IHeaderDefinition> Headers { get; set; }

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		string Id { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		string Name { get; set; }
	}
}