using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
	/// <summary>
	/// Repository for SFG mapping.
	/// </summary>
	public interface ICompositeComponentMappingRepository
	{
		/// <summary>
		/// Gets the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		Task<CompositeComponentMapping> Get(string type);

		/// <summary>
		/// Saves the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="mapping">The mapping.</param>
		/// <returns></returns>
		Task<CompositeComponentMapping> Save(string type, CompositeComponentMapping mapping);
	}
}