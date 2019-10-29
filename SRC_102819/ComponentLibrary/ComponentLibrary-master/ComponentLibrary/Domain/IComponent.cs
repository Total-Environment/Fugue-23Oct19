using System;
using System.Collections.Generic;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    ///     Represents an interface for material data
    /// </summary>
    public interface IComponent
    {
        /// <summary>
        ///     Gets the <see cref="IHeaderData" /> with the specified key.
        /// </summary>
        /// <value>
        ///     The <see cref="IHeaderData" />.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        IHeaderData this[string key] { get; }

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        string Id { get; set; }

        /// <summary>
        ///     Gets the headers.
        /// </summary>
        /// <value>
        ///     The headers.
        /// </value>
        IEnumerable<IHeaderData> Headers { get; set; }

        /// <summary>
        ///     Gets or sets the search keywords.
        /// </summary>
        /// <value>
        ///     The search keywords.
        /// </value>
        List<string> SearchKeywords { get; }

        /// <summary>
        ///     Gets or sets the component definition.
        /// </summary>
        /// <value>
        ///     The component definition.
        /// </value>
        IComponentDefinition ComponentDefinition { get; set; }

        
        /// <summary>
        /// Gets or sets the group
        /// </summary>
        string Group { get; set; }

        /// <summary>
        ///     Gets or sets the created at.
        /// </summary>
        /// <value>
        ///     The created at.
        /// </value>
        DateTime CreatedAt { get; set; }

        /// <summary>
        ///     Gets or sets the amended at.
        /// </summary>
        /// <value>
        ///     The amended at.
        /// </value>
        DateTime AmendedAt { get; set; }

        /// <summary>
        ///     Gets or sets the created by.
        /// </summary>
        /// <value>
        ///     The created by.
        /// </value>
        string CreatedBy { get; set; }

        /// <summary>
        ///     Gets or sets the amended by.
        /// </summary>
        /// <value>
        ///     The amended by.
        /// </value>
        string AmendedBy { get; set; }

        /// <summary>
        ///     Appends the search keywords.
        /// </summary>
        /// <param name="searchKewordList">The search keword list.</param>
        void AppendSearchKeywords(List<string> searchKewordList);
    }
}