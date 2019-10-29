using System.Threading.Tasks;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
	/// <summary>
	/// Represents a material definition repository.
	/// </summary>
	public interface ICompositeComponentDefinitionRepository<T> where T : ICompositeComponentDefinition
	{
		/// <summary>
		/// Adds the specified material definition.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="componentDefinition">The material definition.</param>
		/// <returns></returns>
		Task Add(string type, T componentDefinition);

		/// <summary>
		/// Finds the specified name.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		Task<T> Find(string type, string name);

		/// <summary>
		/// Updates the specified material definition.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="componentDefinition">The material definition.</param>
		/// <returns></returns>
		Task Update(string type, T componentDefinition);
	}
}