using System.Collections.Generic;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    ///     Class Material.
    /// </summary>
    /// <seealso cref="IComponent" />
    public class Material : Component, IMaterial
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Material" /> class.
        /// </summary>
        public Material()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Material" /> class.
        /// </summary>
        /// <param name="headers">The headers.</param>
        /// <param name="materialDefinition">The material definition.</param>
        public Material(IEnumerable<IHeaderData> headers, IMaterialDefinition materialDefinition)
            : base(headers, materialDefinition)
        {
        }
    }
}