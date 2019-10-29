using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    ///     Represents the header data.
    /// </summary>
    /// <seealso cref="IHeaderData" />
    public class HeaderData : DynamicObject, IHeaderData
    {
        private List<IColumnData> _columns;
        private string _key;
        private string _name;

        /// <summary>
        ///     Initializes a new instance of the <see cref="HeaderData" /> class.
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="key">The key.</param>
        public HeaderData(string name, string key)
        {
            Name = name;
            Key = key;
            Columns = new List<IColumnData>();
        }

        /// <inheritdoc />
        public IEnumerable<IColumnData> Columns
        {
            get { return _columns; }
            set { _columns = value.ToList(); }
        }

        /// <inheritdoc />
        public string Key
        {
            get { return _key; }
            set
            {
                if (value == null)
                    throw new ArgumentException("key is required for Header.");
                _key = value;
            }
        }

        /// <inheritdoc />
        public string Name
        {
            get { return _name; }
            set
            {
                if (value == null)
                    throw new ArgumentException("name is required for Header.");
                _name = value;
            }
        }

        /// <inheritdoc />
        public object this[string name]
        {
            get { return Columns.FirstOrDefault(c => c.Name == name)?.Value; }
        }

        /// <inheritdoc />
        public void AddColumns(ColumnData columnData)
        {
            _columns.Add(columnData);
        }

        /// <inheritdoc />
        public IColumnData Column(string name)
        {
            return Columns.FirstOrDefault(c => string.Equals(c.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>
        /// Tries the get member.
        /// </summary>
        /// <param name="binder">The binder.</param>
        /// <param name="columnData">The column data.</param>
        /// <returns></returns>
        public override bool TryGetMember(GetMemberBinder binder,
            out object columnData)
        {
            columnData = _columns.FirstOrDefault(h => h.Key == binder.Name) ?? new NullColumnData();
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
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (!(value is IColumnData))
                return false;

            var columnData = _columns.FirstOrDefault(c => c.Key == binder.Name);
            if (columnData == null)
                return false;

            columnData = (IColumnData)value;
            return true;
        }
    }
}