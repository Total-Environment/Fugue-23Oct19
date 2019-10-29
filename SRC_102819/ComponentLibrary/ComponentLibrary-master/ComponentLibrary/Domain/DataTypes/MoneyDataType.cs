using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes
{
    /// <summary>
    /// Represents money data type.
    /// </summary>
    public class MoneyDataType : ISimpleDataType
    {
        /// <summary>
        /// Parses the specified column data.
        /// </summary>
        /// <param name="columnData">The column data.</param>
        /// <returns></returns>
        /// <exception cref="FormatException">
        /// Expected an object of type Money. Got {columnData} or Amount is required. or Currency is
        /// required. or Amount should be numeric or Amount should not be greater than
        /// 100000000000000000000000000000. That is 28 zeroes.
        /// </exception>
        public Task<object> Parse(object columnData)
        {
            if (!(columnData is IDictionary<string, object>))
                throw new FormatException($"Expected an object of type Money. Got {columnData}");
            var dictionary = (IDictionary<string, object>)columnData;
            var amountValue = GetValueFromDictionary(dictionary, "Amount");
            var currencyValue = GetValueFromDictionary(dictionary, "Currency");
            if (amountValue == null)
                throw new FormatException("Amount is required.");
            if (currencyValue == null)
                throw new FormatException("Currency is required.");
            decimal amount;
            try
            {
                amount = Convert.ToDecimal(amountValue);
            }
            catch (FormatException)
            {
                throw new FormatException("Amount should be numeric");
            }
            catch (OverflowException)
            {
                throw new FormatException(
                    "Amount should not be greater than 100000000000000000000000000000. That is 28 zeroes.");
            }
            return Task.FromResult<object>(new MoneyValue(amount, (string)currencyValue));
        }
        private static object GetValueFromDictionary(IDictionary<string, object> dictionary, string key)
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key];
            return dictionary.ContainsKey(key.ToLower()) ? dictionary[key.ToLower()] : null;
        }
    }
}