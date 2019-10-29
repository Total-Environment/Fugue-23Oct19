using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    /// Represents a master definition.
    /// </summary>
    /// <seealso cref="IComponentDefinition"/>
    public class ComponentDefinition : IComponentDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentDefinition"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="System.ArgumentException"></exception>
        public ComponentDefinition(string name)
        {
            if (name == null)
                throw new ArgumentException(nameof(name));
            Name = name;
            Headers = new List<IHeaderDefinition>();
        }

        /// <inheritdoc />
        public string Code { get; set; }

        /// <inheritdoc />
        public List<IHeaderDefinition> Headers { get; set; }

        /// <inheritdoc />
        public string Id { get; set; }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public IHeaderDefinition this[string name]
        {
            get { return Headers.FirstOrDefault(h => h.Name == name); }
        }

        /// <inheritdoc />
        public async Task<T> Parse<T>(IDictionary<string, object> data, IBrandDefinition brandDefinition)
            where T : IComponent, new()
        {
            var headers = await Task.WhenAll(Headers.Select(h =>
            {
                if (!data.ContainsKey(h.Name))
                    throw new ArgumentException($"{h.Name} is required.");
                return h.Parse((Dictionary<string, object>)data[h.Name]);
            }));
            var component = new T
            {
                Headers = headers,
                ComponentDefinition = this
            };
            component.AppendSearchKeywords(GenerateKeywords(headers.Select(h => h.Columns).ToList(), brandDefinition));

            return component;
        }

        /// <inheritdoc />
        public IComponent ParseComponent(IComponent component)
        {
            foreach (var componentHeader in component.Headers)
            {
                var header = Headers.FirstOrDefault(h => h.Name == componentHeader.Name);
                if (header == null)
                    throw new ArgumentException(
                        $"Invalid header: no header found with name {componentHeader.Name}.");
                foreach (var column in componentHeader.Columns)
                    if (header.Columns.All(c => c.Name != column.Name))
                        throw new ArgumentException(
                            $"Invalid Column: no column found with name {column.Name} under header {header.Name}.");
            }

            return component;
        }

        private List<string> GenerateKeywords(List<IEnumerable<IColumnData>> headerColumnData,
                    IBrandDefinition brandDefinition)
        {
            var keywordsList = new List<string>();

            foreach (var headerDefinition in Headers)
                foreach (var column in headerDefinition.Columns)
                    if (column.IsSearchable)
                    {
                        IColumnData columnDataObject = null;
                        foreach (var columnData in headerColumnData)
                        {
                            columnDataObject = columnData.FirstOrDefault(c => c.Name == column.Name);
                            if (columnDataObject != null)
                                break;
                        }

                        if (column.DataType is BrandDataType)
                        {
                            var brandColumns = brandDefinition.Columns.ToList();
                            foreach (var brandColumn in brandColumns)
                                if (brandColumn.IsSearchable)
                                {
                                    var brandColumnData =
                                        ((IEnumerable)columnDataObject?.Value)?.Cast<Dictionary<string, object>>();
                                    var brandColumnName = brandColumn.Key.ToLower();
                                    if (brandColumnData == null) continue;
                                    foreach (var brand in brandColumnData)
                                    {
                                        foreach (var brandKey in brand.Keys)
                                        {
                                            if (brandKey.ToLower().Equals(brandColumnName))
                                            {
                                                keywordsList.Add(brand[brandKey]?.ToString());
                                            }
                                        }
                                    }
                                }
                        }
                        else
                        {
                            if (columnDataObject?.Value != null)
                                if (columnDataObject.Value.GetType() == typeof(object[]))
                                {
                                    var columnDataArray = Array.ConvertAll((object[])columnDataObject.Value,
                                        d => d.ToString());
                                    keywordsList.AddRange(columnDataArray);
                                }
                                else if (!columnDataObject.Value.ToString().Equals("-- NA --"))
                                {
                                    keywordsList.Add(columnDataObject.Value.ToString());
                                }
                        }
                    }
            return keywordsList;
        }
    }
}