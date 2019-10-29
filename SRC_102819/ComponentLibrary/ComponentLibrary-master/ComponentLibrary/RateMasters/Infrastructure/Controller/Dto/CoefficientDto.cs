using System;
using System.Globalization;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller.Dto
{
    /// <summary>
    /// The DTO for coefficients
    /// </summary>
    public class CoefficientDto
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
        public MoneyDto Value { get; set; }

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
        /// Initializes a new instance of the <see cref="CoefficientDto"/> class.
        /// </summary>
        /// <param name="coefficient">The coefficient.</param>
        /// <exception cref="System.ArgumentException">Coefficient Cannot be null.</exception>
        public CoefficientDto(ICoefficient coefficient)
        {
            if (coefficient == null)
                throw new ArgumentException("Coefficient Cannot be null.");
            Name = coefficient.Name;
            var coefficientType = coefficient.GetType();
            if (coefficientType == typeof(SumCoefficient))
            {
                var c = coefficient as SumCoefficient;
                Value = new MoneyDto(c?.Value);
            }
            else if (coefficientType == typeof(PercentageCoefficient))
            {
                var c = coefficient as PercentageCoefficient;
                Percentage = c.Value.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoefficientDto"/> class.
        /// </summary>
        public CoefficientDto()
        {

        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        protected bool Equals(CoefficientDto other)
        {
            return Equals(Name, other.Name) && Equals(Value, other.Value);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((CoefficientDto) obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Value.GetHashCode();
                hashCode = (hashCode * 397) ^ (Name?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Value?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return
                $"{nameof(Name)}: {Name}, {nameof(Value)}: {Value}, {nameof(Percentage)}: {Percentage}";
        }
    }
}