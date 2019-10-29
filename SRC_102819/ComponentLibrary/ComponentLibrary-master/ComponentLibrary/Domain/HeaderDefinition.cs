using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    /// Represents a header definition.
    /// </summary>
    /// <seealso cref="IHeaderDefinition"/>
    public class HeaderDefinition : IHeaderDefinition
    {
        private IList<IColumnDefinition> _columns;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderDefinition"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="dependency">The dependency.</param>
        public HeaderDefinition(string name, string key, IEnumerable<IColumnDefinition> columns,
            IEnumerable<IDependencyDefinition> dependency = null)
        {
            Name = name;
            Key = key;
            _columns = new List<IColumnDefinition>(columns);
            Dependency = dependency;
        }

        /// <inheritdoc />
        public IEnumerable<IColumnDefinition> Columns
        {
            get { return _columns.AsEnumerable(); }
            set { _columns = new List<IColumnDefinition>(value); }
        }

        /// <inheritdoc />
        public IEnumerable<IDependencyDefinition> Dependency { get; set; }

        /// <inheritdoc />
        public string Key { get; set; }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public void AddColumn(IColumnDefinition columnDefinition)
        {
            _columns.Add(columnDefinition);
        }

        /// <inheritdoc />
        public async Task<IHeaderData> Parse(IDictionary<string, object> data)
        {
            try
            {
                var headerData = new HeaderData(Name, Key)
                {
                    Columns = await Task.WhenAll(Columns.Select(c =>
                    {
                        var columnData = data.ContainsKey(c.Name) ? data[c.Name] : null;
                        return c.Parse(columnData);
                    }))
                };

                if (Dependency != null)
                    foreach (var dependencyDefinition in Dependency)
                    {
                        var stringDictionary = data.ToDictionary(kv => kv.Key, kv => (string)kv.Value);
                        if (!dependencyDefinition.Validate(stringDictionary))
                            throw new FormatException("One of the dependent values is not valid.");
                    }

                return headerData;
            }
            catch (FormatException ex)
            {
                throw new FormatException($"Failed parsing {Name}:{ex.Message}", ex);
            }
        }

        /// <inheritdoc />
        public IColumnDefinition this[string name]
        {
            get { return Columns.FirstOrDefault(h => h.Name == name); }
        }
    }
}