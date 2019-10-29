using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
	/// <summary>
	/// </summary>
	/// <seealso cref="TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao.Entity"/>
	public class CodePrefixTypeMappingDao : Entity
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CodePrefixTypeMappingDao"/> class.
		/// </summary>
		/// <param name="codePrefixTypeMapping">The code prefix type mapping.</param>
		public CodePrefixTypeMappingDao(CodePrefixTypeMapping codePrefixTypeMapping)
		{
			this.CodePrefix = codePrefixTypeMapping.CodePrefix;
			this.ComponentType = codePrefixTypeMapping.ComponentType;
		}

		/// <summary>
		/// Gets or sets the code prefix.
		/// </summary>
		/// <value>The code prefix.</value>
		public string CodePrefix { get; set; }

		/// <summary>
		/// Gets or sets the type of the component.
		/// </summary>
		/// <value>The type of the component.</value>
		public ComponentType ComponentType { get; set; }

		/// <summary>
		/// To the code prefix type mapping.
		/// </summary>
		/// <returns></returns>
		public CodePrefixTypeMapping ToCodePrefixTypeMapping()
		{
			return new CodePrefixTypeMapping(CodePrefix, ComponentType);
		}
	}
}