using System;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Core
{
    /// <summary>
    /// Interface for Exchange Rates
    /// </summary>
    public interface IExchangeRate
    {
        /// <summary>
        /// Gets from currency.
        /// </summary>
        /// <value>
        /// From currency.
        /// </value>
        string FromCurrency { get; }
        /// <summary>
        /// Gets to currency.
        /// </summary>
        /// <value>
        /// To currency.
        /// </value>
        string ToCurrency { get; }
        /// <summary>
        /// Rates this instance.
        /// </summary>
        /// <returns></returns>
        decimal Rate();
        /// <summary>
        /// Gets the applied from.
        /// </summary>
        /// <value>
        /// The applied from.
        /// </value>
        DateTime AppliedFrom { get; }
    }
}