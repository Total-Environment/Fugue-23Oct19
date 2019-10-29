using System;
using System.Globalization;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository.Dao
{
    /// <summary>
    /// The DAO for coefficients
    /// </summary>
    public class CoefficientDao
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public MoneyDao Value { get; set; }

        /// <summary>
        /// Gets or sets the percentage.
        /// </summary>
        /// <value>
        /// The percentage.
        /// </value>
        public string Percentage { get; set; }

        /// <summary>
        /// Domains the specified bank.
        /// </summary>
        /// <param name="bank">The bank.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        /// Coefficient cannot have value, factor and percentage.
        /// or
        /// Coefficient cannot have value, factor and percentage as null.
        /// </exception>
        public ICoefficient Domain(IBank bank)
        {
            if (Value != null && Percentage != null)
                throw new ArgumentException("Coefficient cannot have value and factor.");
            if (Value != null)
                return new SumCoefficient(Name, Value.Domain(bank));
            if (Percentage != null)
                return new PercentageCoefficient(Name, decimal.Parse(Percentage));
            throw new ArgumentException("Coefficient cannot have value, factor and percentage as null.");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoefficientDao"/> class.
        /// </summary>
        /// <param name="coefficient">The coefficient.</param>
        /// <exception cref="System.ArgumentException">Coefficient Cannot be null.</exception>
        public CoefficientDao(ICoefficient coefficient)
        {
            if (coefficient == null)
                throw new ArgumentException("Coefficient Cannot be null.");
            Name = coefficient.Name;
            var coefficientType = coefficient.GetType();
            if (coefficientType == typeof(SumCoefficient))
            {
                var c = coefficient as SumCoefficient;
                Value = new MoneyDao(c?.Value);
            }
            else if (coefficientType == typeof(PercentageCoefficient))
            {
                var c = coefficient as PercentageCoefficient;
                Percentage = c.Value.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoefficientDao"/> class.
        /// </summary>
        public CoefficientDao()
        {

        }
    }
}