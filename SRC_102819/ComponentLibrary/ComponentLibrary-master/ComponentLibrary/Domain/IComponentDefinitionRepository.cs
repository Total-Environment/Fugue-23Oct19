using System.Threading.Tasks;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
	/// <summary>
	/// Represents a material definition repository.
	/// </summary>
	public interface IComponentDefinitionRepository<T> where T : IComponentDefinition
	{
		/// <summary>
		/// Adds the specified material definition.
		/// </summary>
		/// <param name="componentDefinition">The material definition.</param>
		/// <returns></returns>
		Task Add(T componentDefinition);

		/// <summary>
		/// Finds the specified name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		Task<T> Find(string name);

		/// <summary>
		/// Updates the specified material definition.
		/// </summary>
		/// <param name="componentDefinition">The material definition.</param>
		/// <returns></returns>
		Task Update(T componentDefinition);
	}
}