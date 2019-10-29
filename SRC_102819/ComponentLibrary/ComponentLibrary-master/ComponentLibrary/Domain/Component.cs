using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    ///     Component
    /// </summary>
    /// <seealso cref="IComponent" />
    public abstract class Component : DynamicObject, IComponent
    {
        protected List<IHeaderData> _headers;
        private string _id;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Component" /> class.
        /// </summary>
        protected Component()
        {
            SearchKeywords = new List<string>();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Component" /> class.
        /// </summary>
        /// <param name="headers">The headers.</param>
        /// <param name="componentDefinition">The service definition.</param>
        protected Component(IEnumerable<IHeaderData> headers, IComponentDefinition componentDefinition) : this()
        {
            Headers = new List<IHeaderData>(headers);
            ComponentDefinition = componentDefinition;
        }

        /// <inheritdoc />
        public DateTime AmendedAt { get; set; }

        /// <inheritdoc />
        public string AmendedBy { get; set; }

        /// <inheritdoc />
        public IComponentDefinition ComponentDefinition { get; set; }

        /// <inheritdoc />
        public DateTime CreatedAt { get; set; }

        /// <inheritdoc />
        public string CreatedBy { get; set; }

        /// <inheritdoc />
        public string Group { get; set; }

        /// <inheritdoc />
        public IEnumerable<IHeaderData> Headers
        {
            get { return _headers; }
            set { _headers = value.ToList(); }
        }

        /// <inheritdoc />
        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                SearchKeywords.Add(value);
            }
        }

        /// <inheritdoc />
        public List<string> SearchKeywords { get; }

        /// <inheritdoc />
        public IHeaderData this[string key]
        {
            get { return Headers.FirstOrDefault(h => h.Name == key); }
        }

        /// <inheritdoc />
        public void AppendSearchKeywords(List<string> searchKewordList)
        {
            if (searchKewordList != null)
                SearchKeywords.AddRange(searchKewordList);
        }

        /// <inheritdoc />
        public object GetColumnValue(string headerName, string columnName)
        {
            var header = this[headerName];
            if (header == null)
                throw new BetterKeyNotFoundException(headerName);
            var column = header.Columns.FirstOrDefault(c => c.Name == columnName);
            if (column == null)
                throw new BetterKeyNotFoundException(columnName);
            return column.Value;
        }

        /// <inheritdoc />
        public IHeaderData Header(string name)
        {
            return Headers.FirstOrDefault(h => string.Equals(h.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>
        /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result" />.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)
        /// </returns>
        /// <exception cref="ArgumentException"></exception>
        public override bool TryGetMember(GetMemberBinder binder,
            out object result)
        {
            result = _headers.FirstOrDefault(h => h.Key == binder.Name);
            if (result == null)
                throw new ArgumentException($"No header with {binder.Name} is found.");
            return true;
        }

        /// <summary>
        /// Provides the implementation for operations that set member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as setting a value for a property.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member to which the value is being assigned. For example, for the statement sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="value">The value to set to the member. For example, for sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, the <paramref name="value" /> is "Test".</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
        /// </returns>
        /// <exception cref="InvalidOperationException"></exception>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            throw new InvalidOperationException();
        }
    }
}