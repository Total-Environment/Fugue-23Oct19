using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <summary>
	/// Builder to create Semi Finished Good
	/// </summary>
	public interface ICompositeComponentBuilder
	{
		/// <summary>
		/// Builds SFG by cloning the service.
		/// </summary>
		/// <param name="service">The service.</param>
		/// <param name="sfgDefintion">The SFG defintion.</param>
		/// <param name="componentComposition"></param>
		/// <returns>Created SFG.</returns>
		Task<CompositeComponent> CloneFromService(IService service, ICompositeComponentDefinition sfgDefintion, IComponentComposition componentComposition);

		/// <summary>
		/// Creates the SFG from data.
		/// </summary>
		/// <param name="sfgDefintion">The SFG defintion.</param>
		/// <param name="sfgTobeCreated">The SFG data.</param>
		/// <returns>Created SFG.</returns>
		Task<CompositeComponent> Create(string type, ICompositeComponentDefinition sfgDefintion, CompositeComponent sfgTobeCreated);

		/// <summary>
		/// Updates the specified SFG defintion.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="compositeComponentDefinition">The SFG defintion.</param>
		/// <param name="compositeComponentTobeUpdated">The SFG tobe updated.</param>
		/// <returns></returns>
		Task<CompositeComponent> Update(string type, ICompositeComponentDefinition compositeComponentDefinition, CompositeComponent compositeComponentTobeUpdated);
	}
}