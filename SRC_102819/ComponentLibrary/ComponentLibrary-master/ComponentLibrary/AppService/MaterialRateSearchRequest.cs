using System;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <summary>
    /// Represents the structure for a material search request
    /// </summary>
    public class MaterialRateSearchRequest : IRateSearchRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialRateSearchRequest"/> class.
        /// </summary>
        public MaterialRateSearchRequest()
        {
            PageNumber = -1;
            BatchSize = -1;
            SortColumn = "appliedOn";
            SortOrder = SortOrder.Descending;
            AppliedOn = null;
        }

        /// <inheritdoc/>
        public int BatchSize { get; set; }

        /// <inheritdoc/>
        public int PageNumber { get; set; }

        /// <inheritdoc/>
        public string SortColumn { get; set; }

        /// <inheritdoc/>
        public SortOrder SortOrder { get; set; }

        /// <inheritdoc/>
        public string Location { get; set; }

        /// <inheritdoc/>
        public string TypeOfPurchase { get; set; }

        /// <inheritdoc/>
        public DateTime? AppliedOn { get; set; }

        protected bool Equals(MaterialRateSearchRequest other)
        {
            return BatchSize == other.BatchSize && PageNumber == other.PageNumber && string.Equals(SortColumn, other.SortColumn) && SortOrder == other.SortOrder && string.Equals(Location, other.Location) && string.Equals(TypeOfPurchase, other.TypeOfPurchase) && AppliedOn.Equals(other.AppliedOn);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((MaterialRateSearchRequest)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = BatchSize;
                hashCode = (hashCode * 397) ^ PageNumber;
                hashCode = (hashCode * 397) ^ (SortColumn != null ? SortColumn.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)SortOrder;
                hashCode = (hashCode * 397) ^ (Location != null ? Location.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TypeOfPurchase != null ? TypeOfPurchase.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ AppliedOn.GetHashCode();
                return hashCode;
            }
        }
    }
}