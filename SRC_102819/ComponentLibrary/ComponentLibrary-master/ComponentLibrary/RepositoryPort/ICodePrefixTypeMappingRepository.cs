using System.Threading.Tasks;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
	/// <summary>
	/// </summary>
	public interface ICodePrefixTypeMappingRepository
	{
		/// <summary>
		/// Adds the specified code prefix type mapping.
		/// </summary>
		/// <param name="codePrefixTypeMapping">The code prefix type mapping.</param>
		/// <returns></returns>
		Task Add(CodePrefixTypeMapping codePrefixTypeMapping);

		/// <summary>
		/// Gets the specified code prefix.
		/// </summary>
		/// <param name="codePrefix">The code prefix.</param>
		/// <returns></returns>
		Task<CodePrefixTypeMapping> Get(string codePrefix);
	}
}