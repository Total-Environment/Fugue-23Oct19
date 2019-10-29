using System;
using System.Threading.Tasks;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Core
{
	/// <summary>
	/// The class for money
	/// </summary>
	public class Money
	{
		private readonly IBank _bank;
		private readonly decimal _value;

		/// <summary>
		/// Initializes a new instance of the <see cref="Money"/> class.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="currency">The currency.</param>
		/// <param name="bank">The bank.</param>
		/// <exception cref="ArgumentException">
		/// Value of Money cannot be zero or negative. or Currency Cannot be null or empty. or Bank
		/// cannot be null.
		/// </exception>
		public Money(decimal value, string currency, IBank bank)
		{
			if (value < 0)
				throw new ArgumentException("Value of Money cannot be zero or negative.");
			if (string.IsNullOrWhiteSpace(currency))
				throw new ArgumentException("Currency Cannot be null or empty.");
			if (bank == null)
				throw new ArgumentException("Bank cannot be null.");
			_value = value;
			Currency = currency;
			_bank = bank;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Money"/> class.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="currency">The currency.</param>
		public Money(decimal value, string currency)
		{
			_value = value;
			Currency = currency;
		}

		/// <inheritdoc/>
		public string Currency { get; }

		/// <inheritdoc/>
		public decimal Value
		{
			get { return Math.Round(_value, 2); }
		}

		/// <summary>
		/// Adds the specified money.
		/// </summary>
		/// <param name="money">The money.</param>
		/// <returns></returns>
		public async Task<Money> Add(Money money)
		{
			var convertMoney = await _bank.ConvertTo(money, Currency);
			return new Money(Value + convertMoney.Value, Currency, _bank);
		}

		/// <inheritdoc/>
		public async Task<int> CompareTo(object obj)
		{
			if (!(obj is Money))
			{
				throw new InvalidOperationException("Cannot compare with value which is not money.");
			}

			var other = (Money)obj;
			var current = this;

			if (Currency != other.Currency)
			{
				current = await ConvertToCurrency(other.Currency);
			}

			return current.Value.CompareTo(other.Value);
		}

	    /// <summary>
	    /// Converts to currency.
	    /// </summary>
	    /// <param name="currency">The currency.</param>
	    /// <param name="appliedOn"></param>
	    /// <returns></returns>
	    public async Task<Money> ConvertToCurrency(string currency, DateTime appliedOn)
		{
            return await _bank.ConvertTo(this, currency, appliedOn);
		}

        /// <summary>
        /// Converts to currency.
        /// </summary>
        /// <param name="currency">The currency.</param>
        /// <returns></returns>
        public async Task<Money> ConvertToCurrency(string currency)
        {
            return await _bank.ConvertTo(this, currency);
        }

        /// <summary>
        /// Divides the specified dividend.
        /// </summary>
        /// <param name="dividend">The dividend.</param>
        /// <returns></returns>
        public Money Divide(int dividend)
		{
			return new Money(Value / dividend, Currency, _bank);
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/>, is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
		/// <returns>
		/// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance;
		/// otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj)
		{
			var money = obj as Money;
			return money != null && Value == money.Value && Currency.Equals(money.Currency);
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures
		/// like a hash table.
		/// </returns>
		public override int GetHashCode()
		{
			unchecked
			{
				return (Value.GetHashCode() * 397) ^ (Currency?.GetHashCode() ?? 0);
			}
		}

		/// <summary>
		/// Percentages the specified percentage.
		/// </summary>
		/// <param name="percentage">The percentage.</param>
		/// <returns></returns>
		public Money Percentage(decimal percentage)
		{
			var money = new Money(Value * percentage / 100, Currency, _bank);
			return money;
		}

		/// <summary>
		/// Timeses the specified times.
		/// </summary>
		/// <param name="times">The times.</param>
		/// <returns></returns>
		public Money Times(decimal times)
		{
			var money = new Money(Value * times, Currency, _bank);
			return money;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents this instance.</returns>
		public override string ToString()
		{
			return $"{Value} {Currency}";
		}
	}
}