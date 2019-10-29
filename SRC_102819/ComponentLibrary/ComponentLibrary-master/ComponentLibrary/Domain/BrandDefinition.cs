using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    /// The Brand Definition
    /// </summary>
    /// <seealso cref="IBrandDefinition"/>
    public class BrandDefinition : IBrandDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrandDefinition"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="columns">The definitions.</param>
        /// <exception cref="System.ArgumentException">name</exception>
        public BrandDefinition(string name, List<ISimpleColumnDefinition> columns)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            Name = name;
            if (columns == null)
                throw new ArgumentNullException(nameof(columns));
            Columns = columns;
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public List<ISimpleColumnDefinition> Columns { get; }

        /// <inheritdoc />
        public async Task<Brand> Parse(IEnumerable<IDictionary<string, object>> columns)
        {
            var columnDataTasks = Columns.Select(column => column.Parse(columns.FirstOrDefault(d => (string)d["key"] == column.Key)?["value"]));
            var columnData = await Task.WhenAll(columnDataTasks);
            return new Brand(columnData, this);
        }
    }
}