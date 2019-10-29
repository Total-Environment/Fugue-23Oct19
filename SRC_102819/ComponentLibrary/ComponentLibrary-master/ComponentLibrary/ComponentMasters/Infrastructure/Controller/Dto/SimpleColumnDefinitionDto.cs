using System.Linq;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto
{
    /// <summary>
    /// Represents a column definition dto.
    /// </summary>
    public class SimpleColumnDefinitionDto
    {
        /// <summary>
        /// Construct using domain object.
        /// </summary>
        /// <param name="columnDefinition"></param>
        public SimpleColumnDefinitionDto(ISimpleColumnDefinition columnDefinition)
        {
            SetDomain(columnDefinition);
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SimpleColumnDefinitionDto() { }

        /// <summary>
        /// Gets or sets the type of the data.
        /// </summary>
        /// <value>The type of the data.</value>
        public SimpleDataTypeDto DataType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is required.
        /// </summary>
        /// <value><c>true</c> if this instance is required; otherwise, <c>false</c>.</value>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is searchable.
        /// </summary>
        /// <value><c>true</c> if this instance is searchable; otherwise, <c>false</c>.</value>
        public bool IsSearchable { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets the domain.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <returns>Returns the domain object.</returns>
        public async Task<ISimpleColumnDefinition> GetDomain(ISimpleDataTypeFactory factory)
        {
            //May be remove after migration, dont want to set keys in all jsons
            if (Key == null || Key.Contains(' '))
            {
                Key = Name.Replace(' ', '_').ToLower();
            }
            return new SimpleColumnDefinition(Name, Key, await DataType.GetDomain(factory), IsSearchable, IsRequired);
        }

        /// <summary>
        /// Sets the domain.
        /// </summary>
        /// <param name="value">The value.</param>
        private void SetDomain(ISimpleColumnDefinition value)
        {
            Name = value.Name;
            DataType = new SimpleDataTypeDto();
            DataType.SetDomain(value.DataType);
            IsSearchable = value.IsSearchable;
            IsRequired = value.IsRequired;
            Key = value.Key;
        }
    }
}