using System;
using System.Threading.Tasks;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Core
{
    /// <summary>
    /// The interface for bank
    /// </summary>
    public interface IBank
    {
        /// <summary>
        /// Converts to.
        /// </summary>
        /// <param name="money">The money.</param>
        /// <param name="toCurrency">To currency.</param>
        /// <returns></returns>
        Task<Money> ConvertTo(Money money, string toCurrency);

        /// <summary>
        /// Converts to.
        /// </summary>
        /// <param name="money">The money.</param>
        /// <param name="toCurrency">To currency.</param>
        /// <param name="appliedOn">The applied on.</param>
        /// <returns></returns>
        Task<Money> ConvertTo(Money money, string toCurrency, DateTime appliedOn);
    }
}