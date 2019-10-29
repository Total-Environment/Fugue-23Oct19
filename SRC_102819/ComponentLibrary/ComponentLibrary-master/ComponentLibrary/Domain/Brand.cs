using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    /// </summary>
    /// <seealso cref="System.Dynamic.DynamicObject"/>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.Domain.IBrand"/>
    public class Brand : DynamicObject, IBrand
    {
        private List<IColumnData> _columns;

        /// <inheritdoc />
        public IEnumerable<IColumnData> Columns
        {
            get { return _columns; }
            set { _columns = value.ToList(); }
        }

        /// <inheritdoc />
        public IBrandDefinition BrandDefinition { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Brand"/> class.
        /// </summary>
        public Brand()
        {
            Columns = new List<IColumnData> { new ColumnData("Brand Code", "brand_code", null) };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Brand"/> class.
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <param name="brandDefinition">The brand definition.</param>
        public Brand(IEnumerable<IColumnData> columns, IBrandDefinition brandDefinition)
        {
            Columns = new List<IColumnData>(columns);
            BrandDefinition = brandDefinition;
        }

        /// <summary>
        /// Provides the implementation for operations that set member values. Classes derived from
        /// the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to
        /// specify dynamic behavior for operations such as setting a value for a property.
        /// </summary>
        /// <param name="binder">
        /// Provides information about the object that called the dynamic operation. The binder.Name
        /// property provides the name of the member to which the value is being assigned. For
        /// example, for the statement sampleObject.SampleProperty = "Test", where sampleObject is an
        /// instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/>
        /// class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies
        /// whether the member name is case-sensitive.
        /// </param>
        /// <param name="value">
        /// The value to set to the member. For example, for sampleObject.SampleProperty = "Test",
        /// where sampleObject is an instance of the class derived from the <see
        /// cref="T:System.Dynamic.DynamicObject"/> class, the <paramref name="value"/> is "Test".
        /// </param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the
        /// run-time binder of the language determines the behavior. (In most cases, a
        /// language-specific run-time exception is thrown.)
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

        /// <inheritdoc />
        public object this[string key]
        {
            get { return Columns.FirstOrDefault(c => c.Key == key)?.Value; }
            set
            {
                var column = Columns.FirstOrDefault(c => c.Key == key);
                column.Value = value;
            }
        }

        /// <inheritdoc />
        public IColumnData Column(string key)
        {
            return Columns.FirstOrDefault(c => string.Equals(c.Key, key, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}