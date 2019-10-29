using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain
{
    /// <summary>
    /// The interface for coefficients
    /// </summary>
    public interface ICoefficient
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }
        /// <summary>
        /// Applies the specified money.
        /// </summary>
        /// <param name="money">The money.</param>
        /// <returns></returns>
        Money Apply(Money money);
    }
}