namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    ///     Represents a master definition.
    /// </summary>
    /// <seealso cref="IMaterialDefinition" />
    public class MaterialDefinition : ComponentDefinition, IMaterialDefinition
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MaterialDefinition" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="System.ArgumentException"></exception>
        public MaterialDefinition(string name) : base(name)
        {
        }
    }
}