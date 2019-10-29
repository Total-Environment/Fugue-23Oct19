namespace TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes
{
    /// <summary>
    ///     Represents a unit value.
    /// </summary>
    public class UnitValue
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UnitValue" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        public UnitValue(double value, string type)
        {
            Value = value;
            Type = type;
        }

        /// <summary>
        ///     Gets or sets the type.
        /// </summary>
        /// <value>
        ///     The type.
        /// </value>
        public string Type { get; set; }

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>
        ///     The value.
        /// </value>
        public double Value { get; set; }

        /// <summary>
        ///     Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((UnitValue) obj);
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (Value.GetHashCode() * 397) ^ (Type != null ? Type.GetHashCode() : 0);
            }
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents its value and type.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{Value}{Type}";
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        protected bool Equals(UnitValue other)
        {
            return Value == other.Value && string.Equals(Type, other.Type);
        }
    }
}