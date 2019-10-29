using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
	/// <summary>
	/// </summary>
	/// <seealso cref="TE.ComponentLibrary.ComponentLibrary.Domain.ICompositeComponentDefinition"/>
	public class CompositeComponentDefinition : ICompositeComponentDefinition
	{
		/// <summary>
		/// Gets or sets the code.
		/// </summary>
		/// <value>The code.</value>
		public string Code { get; set; }

		/// <summary>
		/// Gets or sets the headers.
		/// </summary>
		/// <value>The headers.</value>
		public List<IHeaderDefinition> Headers { get; set; }

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }
	}
}