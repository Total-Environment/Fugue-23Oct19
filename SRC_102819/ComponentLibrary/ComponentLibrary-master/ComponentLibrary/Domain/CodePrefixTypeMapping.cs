using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
	/// <summary>
	/// </summary>
	public class CodePrefixTypeMapping
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CodePrefixTypeMapping"/> class.
		/// </summary>
		/// <param name="codePrefix">The code prefix.</param>
		/// <param name="componentType">Type of the component.</param>
		public CodePrefixTypeMapping(string codePrefix, ComponentType componentType)
		{
			CodePrefix = codePrefix;
			ComponentType = componentType;
		}

		/// <summary>
		/// Gets the code prefix.
		/// </summary>
		/// <value>The code prefix.</value>
		public string CodePrefix { get; }

		/// <summary>
		/// Gets the type of the component.
		/// </summary>
		/// <value>The type of the component.</value>
		public ComponentType ComponentType { get; }
	}
}