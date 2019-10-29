using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes
{
    /// <summary>
    /// Represents the range data type.
    /// </summary>
    /// <seealso cref="IDataType"/>
    public class RangeDataType : ISimpleDataType
    {
        /// <summary>
        /// Represents a range data type.
        /// </summary>
        /// <param name="unit"></param>
        public RangeDataType(string unit)
        {
            Unit = unit;
        }

        /// <summary>
        /// Gets the unit.
        /// </summary>
        /// <value>The unit.</value>
        public string Unit { get; }

        /// <inheritdoc/>
        public Task<object> Parse(object columnData)
        {
            try
            {
                var dict = (IDictionary<string, object>)columnData;
                double from;
                var fromValue = GetValueFromDictionary(dict, "From");
                if(fromValue == null)
                    throw new FormatException($"Range Data type with value.'From' is required");
                try
                {
                    from = Convert.ToDouble(fromValue);
                }
                catch (FormatException)
                {
                    throw new FormatException(
                        $"Range Data type with value .Expected 'From' to be of type 'decimal'. Got '{dict["From"]}'");
                }
                catch (Exception e)
                {
                    throw new FormatException(
                        $"Invalid option for range {JsonConvert.SerializeObject(columnData)}, unable to parse.", e);
                }

                double? to;
                var toValue = GetValueFromDictionary(dict, "To");

                if(toValue == null)
                    to = null;
                else
                    try
                    {
                        to = Convert.ToDouble(toValue);
                    }
                    catch (FormatException)
                    {
                        throw new FormatException(
                            $"Range Data type with value. Expected 'To' to be of type 'decimal'. Got '{dict["To"]}'");
                    }
                    catch (Exception e)
                    {
                        throw new FormatException(
                            $"Invalid option for range {JsonConvert.SerializeObject(columnData)}, unable to parse.", e);
                    }
                string unit;
                var unitValue = GetValueFromDictionary(dict,"Unit");
                try
                {
                    if (unitValue as string != Unit)
                        throw new FormatException(
                            $"Range Data type with value. Expected 'Unit' to be '{Unit}'. Got '{dict["Unit"]}'");
                    unit = unitValue as string;
                }
                catch (InvalidCastException)
                {
                    throw new FormatException(
                        $"Range Data type with value. Expected 'Unit' to be of type 'string'. Got '{dict["Unit"]}'");
                }
                return Task.FromResult<object>(new RangeValue(from, to, unit));
            }
            catch (InvalidCastException e)
            {
                throw new FormatException($"Invalid option {columnData}, unable to parse.", e);
            }
        }

        private static object GetValueFromDictionary(IDictionary<string, object> dictionary, string key)
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key];
            return dictionary.ContainsKey(key.ToLower()) ? dictionary[key.ToLower()] : null;
        }
    }
}