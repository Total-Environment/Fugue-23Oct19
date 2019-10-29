using System.Collections.Generic;
using System.Linq;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs
{
    /// <summary>
    /// The DTO for Headers
    /// </summary>
    public class HeaderDto
    {
        private List<ColumnDto> _columns;

        public HeaderDto()
        {
            _columns = new List<ColumnDto>();
        }

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        public IEnumerable<ColumnDto> Columns
        {
            get { return _columns; }
            set { _columns = value.ToList(); }
        }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        public void Add(ColumnDto columnDto)
        {
            _columns.Add(columnDto);
        }

        /// <summary>
        /// Gets the domain.
        /// </summary>
        /// <returns></returns>
        public IHeaderData GetDomain()
        {
            return new HeaderData(Name, Key)
            {
                Columns = Columns.Select(c => c.GetDomain())
            };
        }
    }
}