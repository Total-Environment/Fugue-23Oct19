using System.Collections.Generic;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <summary>
    /// Contains the total cost of SFG and its breakdown
    /// </summary>
    public class CompositeComponentCost
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeComponentCost"/> class.
        /// </summary>
        public CompositeComponentCost()
        {
            ComponentCostBreakup = new List<ComponentCost>();
        }

        /// <summary>
        /// Gets or sets the total cost.
        /// </summary>
        /// <value>
        /// The total cost.
        /// </value>
        public Money TotalCost { get; set; }

        /// <summary>
        /// Gets the coefficient costs.
        /// </summary>
        /// <value>
        /// The coefficient costs.
        /// </value>
        public List<ComponentCost> ComponentCostBreakup { get; }

        /// <summary>
        /// Gets or sets the unit of measure.
        /// </summary>
        /// <value>
        /// The unit of measure.
        /// </value>
        public string UnitOfMeasure { get; set; }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        protected bool Equals(CompositeComponentCost other)
        {
            return Equals(TotalCost, other.TotalCost) &&
                Equals(ComponentCostBreakup, other.ComponentCostBreakup) && Equals(UnitOfMeasure, other.UnitOfMeasure);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CompositeComponentCost)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (TotalCost.GetHashCode() * 397) ^ (ComponentCostBreakup != null ? ComponentCostBreakup.GetHashCode() : 0);
            }
        }
    }
}