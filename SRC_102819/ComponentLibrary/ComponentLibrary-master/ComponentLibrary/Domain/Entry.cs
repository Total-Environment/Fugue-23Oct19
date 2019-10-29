using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    ///     Represents an entry
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Entry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Entry"/> class.
        /// </summary>
        /// <param name="cells">The cells.</param>
        public Entry(IEnumerable<TextCell> cells)
        {
            Cells = cells.Select(c => c).ToList();
        }

        /// <summary>
        /// Gets or sets the cells.
        /// </summary>
        /// <value>
        /// The cells.
        /// </value>
        [JsonProperty("Cells")]
        public IEnumerable<TextCell> Cells { get; set; }

        /// <summary>
        ///     Gets the count.
        /// </summary>
        /// <value>
        ///     The count.
        /// </value>
        public int Count => Cells.Count();

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        protected bool Equals(Entry other)
        {
            return Cells.SequenceEqual(other.Cells);
        }

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
            return Equals((Entry) obj);
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return Cells?.GetHashCode() ?? 0;
        }

        /// <summary>
        ///     To the cells array.
        /// </summary>
        /// <returns></returns>
        public TextCell[] ToCellsArray()
        {
            return Cells.ToArray();
        }
    }
}