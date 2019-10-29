using System;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <summary>
	/// </summary>
	public interface IPriceService
	{
	    /// <summary>
	    /// Gets the price.
	    /// </summary>
	    /// <param name="code">The code.</param>
	    /// <param name="location">The location.</param>
	    /// <param name="appliedOn">The applied on.</param>
	    /// <param name="projectCode">The project code.</param>
	    /// <returns></returns>
	    Task<ComponentPrice> GetPrice(string code, string location, DateTime appliedOn, string projectCode = null);
	}
}