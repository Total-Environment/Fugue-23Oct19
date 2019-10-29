using System;
using System.Collections.Generic;
using System.Linq;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    /// Class Service.
    /// </summary>
    /// <seealso cref="IService"/>
    public class Service : Component, IService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Service"/> class.
        /// </summary>
        public Service()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Service"/> class.
        /// </summary>
        /// <param name="headers">The headers.</param>
        /// <param name="componentDefinition">The service definition.</param>
        public Service(IEnumerable<IHeaderData> headers, ServiceDefinition componentDefinition)
            : base(headers, componentDefinition)
        {
        }

        //Todo: Remove it when the service refactoring is done.
        /// <inheritdoc />
        public void UpdateColumn(string columnName, Dictionary<string, object> value)
        {
            IHeaderData header;
            var headers = HeaderWithColumnName(columnName, out header);
            IColumnData column;
            var columns = ColumnWithName(columnName, header, out column);
            var headerDefinition = HeaderDefinitionWithColumnName(columnName);
            var columnDefinition = ColumnDefinitionWithName(columnName, headerDefinition);
            var parsedValue = columnDefinition.Parse(value).Result;
            column.Value = parsedValue.Value;
            header.Columns = columns;
            Headers = headers;
        }

        private static IColumnDefinition ColumnDefinitionWithName(string columnName, IHeaderDefinition headerDefinition)
        {
            var columnDefinition =
                headerDefinition.Columns.FirstOrDefault(
                    c => string.Equals(c.Name, columnName, StringComparison.CurrentCultureIgnoreCase));
            if (columnDefinition == null)
                throw new ArgumentException($"Definition donnot have column {columnName}.");
            return columnDefinition;
        }

        private List<IColumnData> ColumnWithName(string columnName, IHeaderData header, out IColumnData column)
        {
            var columns = new List<IColumnData>(header.Columns);
            column =
                columns.FirstOrDefault(c => string.Equals(c.Name, columnName, StringComparison.CurrentCultureIgnoreCase));
            if (column == null)
                throw new ArgumentException($"Service {Id} does not have column {columnName}.");
            return columns;
        }

        private IHeaderDefinition HeaderDefinitionWithColumnName(string columnName)
        {
            var headerDefinition = ComponentDefinition.Headers.FirstOrDefault(
                h => h.Columns.Any(c => string.Equals(c.Name, columnName, StringComparison.CurrentCultureIgnoreCase)));
            if (headerDefinition == null)
                throw new ArgumentException($"Definition donnot have column {columnName}.");
            return headerDefinition;
        }

        private List<IHeaderData> HeaderWithColumnName(string columnName, out IHeaderData header)
        {
            var headers = new List<IHeaderData>(Headers);
            header =
                headers.FirstOrDefault(
                    h =>
                        h != null &&
                        h.Columns.Any(
                            c =>
                                c != null &&
                                string.Equals(c.Name, columnName, StringComparison.CurrentCultureIgnoreCase)));
            if (header == null)
                throw new ArgumentException($"Service {Id} does not have column {columnName}.");
            return headers;
        }
    }
}