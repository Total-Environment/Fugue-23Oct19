namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    ///     Represents a master definition.
    /// </summary>
    /// <seealso cref="IComponentDefinition" />
    public class ServiceDefinition : ComponentDefinition, IServiceDefinition
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ServiceDefinition" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="System.ArgumentException"></exception>
        public ServiceDefinition(string name) : base(name)
        {
        }
    }
}