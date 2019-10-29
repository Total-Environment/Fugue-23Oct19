using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto
{
    /// <summary>
    ///     Money dto
    /// </summary>
    public class MoneyDto
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MoneyDto" /> class.
        /// </summary>
        /// <param name="currency">The currency.</param>
        /// <param name="amount">The amount.</param>
        public MoneyDto(string currency, decimal amount)
        {
            Currency = currency;
            Amount = amount;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MoneyDto"/> class.
        /// </summary>
        public MoneyDto()
        {}

        /// <summary>
        ///     Gets the currency.
        /// </summary>
        /// <value>
        ///     The currency.
        /// </value>
        public string Currency { get; set; }

        /// <summary>
        ///     Gets the amount.
        /// </summary>
        /// <value>
        ///     The amount.
        /// </value>
        public decimal Amount { get; set; }

        /// <summary>
        ///     Return domains for this DTO.
        /// </summary>
        /// <returns></returns>
        public MoneyValue Domain()
        {
            return new MoneyValue(Amount, Currency);
        }
    }
}