using Newtonsoft.Json;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    ///     Represents a text cell.
    /// </summary>
    /// <seealso cref="ICell" />
    [JsonObject(MemberSerialization.OptIn)]
    public class TextCell : ICell
    {
        /// <summary>
        ///     The content
        /// </summary>
        [JsonProperty("value")] public string _content;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TextCell" /> class.
        /// </summary>
        /// <param name="content">The content.</param>
        public TextCell(string content)
        {
            _content = content;
        }

        /// <summary>
        ///     Values this instance.
        /// </summary>
        /// <returns></returns>
        public string Value()
        {
            return _content;
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        protected bool Equals(TextCell other)
        {
            return string.Equals(_content, other._content);
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
            return Equals((TextCell) obj);
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return _content?.GetHashCode() ?? 0;
        }
    }
}