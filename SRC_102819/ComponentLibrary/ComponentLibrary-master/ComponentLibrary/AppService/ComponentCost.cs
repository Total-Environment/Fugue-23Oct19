using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <summary>
    /// Model for component code and its cost
    /// </summary>
    public class ComponentCost
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentCost"/> class.
        /// </summary>
        /// <param name="componentCode">The component code.</param>
        /// <param name="cost">The cost.</param>
        public ComponentCost(string componentCode, Money cost)
        {
            Cost = cost;
            ComponentCode = componentCode;
        }

        /// <summary>
        /// Gets the component code.
        /// </summary>
        /// <value>
        /// The component code.
        /// </value>
        public string ComponentCode { get; }

        /// <summary>
        /// Gets the cost.
        /// </summary>
        /// <value>
        /// The cost.
        /// </value>
        public Money Cost { get; }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        protected bool Equals(ComponentCost other)
        {
            return string.Equals(ComponentCode, other.ComponentCode)
                && Equals(Cost, other.Cost);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ComponentCost)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((ComponentCode != null ? ComponentCode.GetHashCode() : 0) * 397) ^ (Cost != null ? Cost.GetHashCode() : 0);
            }
        }
    }
}