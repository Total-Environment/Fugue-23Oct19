using System;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <summary>
	/// </summary>
	public interface ICompositeComponentValidator
	{
		/// <summary>
		/// Validates the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="componentComposition">The component composition.</param>
		/// <returns></returns>
		Task<Tuple<bool, string>> Validate(string type, IComponentComposition componentComposition);
	}
}