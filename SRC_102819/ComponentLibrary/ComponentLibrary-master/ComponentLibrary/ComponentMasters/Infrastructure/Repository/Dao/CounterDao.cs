namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao.Entity" />
    public class CounterDao : Entity
    {
        /// <summary>
        /// Gets or sets the counter identifier.
        /// </summary>
        /// <value>
        /// The counter identifier.
        /// </value>
        public string CounterId { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public int Value { get; set; }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        protected bool Equals(CounterDao other)
        {
            return string.Equals(CounterId, other.CounterId) && Value == other.Value;
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
            return Equals((CounterDao) obj);
        }


        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return (CounterId != null ? CounterId.GetHashCode() : 0);
        }
    }
}