using System.Collections.Generic;
using System.Threading.Tasks;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
	/// <summary>
	/// Represents dependency definition repository.
	/// </summary>
	public interface IDependencyDefinitionRepository
	{
		/// <summary>
		/// Creates the dependency definition.
		/// </summary>
		/// <param name="dependencyDefinition">The dependency definition.</param>
		/// <returns></returns>
		Task CreateDependencyDefinition(DependencyDefinition dependencyDefinition);

		/// <summary>
		/// Gets the dependency definition.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		Task<DependencyDefinition> GetDependencyDefinition(string name);

		/// <summary>
		/// Updates the dependency definition.
		/// </summary>
		/// <param name="dependencyDefinition">The dependency definition.</param>
		/// <returns></returns>
		Task UpdateDependencyDefinition(DependencyDefinition dependencyDefinition);

	    /// <summary>
	    /// Patches the dependency definition.
	    /// </summary>
	    /// <param name="domain">The domain.</param>
	    /// <param name="columnsList"></param>
	    /// <param name="name"></param>
	    /// <returns></returns>
	    Task PatchDependencyDefinition(IEnumerable<DependentBlock> domain, List<string> columnsList, string name);
	}
}