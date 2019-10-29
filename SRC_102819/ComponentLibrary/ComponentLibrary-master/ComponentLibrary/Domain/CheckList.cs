using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    ///     Represents the check list domain object.
    /// </summary>
    [JsonObject]
    public class CheckList
    {
        /// <summary>
        ///     Gets or sets the content.
        /// </summary>
        /// <value>
        ///     The content.
        /// </value>
        public Table Content { get; set; }

        /// <summary>
        ///     Gets or sets the title.
        /// </summary>
        /// <value>
        ///     The title.
        /// </value>
        [Required(ErrorMessage = "Checklist title is mandatory.")]
        public string Title { get; set; }

        /// <summary>
        ///     Gets or sets the check list identifier.
        /// </summary>
        /// <value>
        ///     The check list identifier.
        /// </value>
        [Required(ErrorMessage = "Checklist id is mandatory.")]
        public string CheckListId { get; set; }

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        ///     Gets or sets the template.
        /// </summary>
        /// <value>
        ///     The template.
        /// </value>
        [Required(ErrorMessage = "Template name is mandatory.")]
        public string Template { get; set; }

        /// <summary>
        ///     Gets the data.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Entry> GetData()
        {
            return Content.Content();
        }

        /// <summary>
        ///     Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        protected bool Equals(CheckList other)
        {
            return string.Equals(Id, other.Id);
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
            return Equals((CheckList) obj);
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return Id?.GetHashCode() ?? 0;
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents its value and type.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return CheckListId;
        }
    }
}