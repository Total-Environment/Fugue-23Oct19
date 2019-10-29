using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <summary>
	/// </summary>
	public interface ICompositeComponentDataBuilder
	{
		/// <summary>
		/// Builds the data.
		/// </summary>
		/// <param name="componentComposition">The semi finished good composition.</param>
		/// <returns></returns>
		Task<IComponentComposition> BuildData(IComponentComposition componentComposition);
	}
}