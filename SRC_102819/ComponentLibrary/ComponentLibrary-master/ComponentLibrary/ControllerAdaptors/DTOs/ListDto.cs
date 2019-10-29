using System.Collections.Generic;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs
{
    /// <summary>
    /// List Dto.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListDto<T>
    {
        /// <summary>
        /// Gets and Sets List of Items.
        /// </summary>
        public IList<T> Items { get; set; }

        /// <summary>
        /// Gets and Sets Record Count.
        /// </summary>
        public long RecordCount { get; set; }

        /// <summary>
        /// Gets and Sets Total Pages.
        /// </summary>
        public long TotalPages { get; set; }

        /// <summary>
        /// Gets and Sets Batch size.
        /// </summary>
        public long BatchSize { get; set; }

        /// <summary>
        /// Gets and Sets Page Number.
        /// </summary>
        public long PageNumber { get; set; }

        /// <summary>
        /// Gets and Sets Sort Column.
        /// </summary>
        public string SortColumn { get; set; }

        /// <summary>
        /// Gets and Sets Sort Order.
        /// </summary>
        public SortOrder SortOrder { get; set; }

        /// <summary>
        /// Constructor for List dto.
        /// </summary>
        public ListDto()
        {
        }
    }
}