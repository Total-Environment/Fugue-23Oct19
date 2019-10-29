using System;
using System.Collections.Generic;
using System.Linq;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    //TODO need to remove this class along with material and service definition only component definition is enough.
    /// <summary>
    ///     Definition class for assets
    /// </summary>
    /// <seealso cref="ComponentDefinition" />
    public class AssetDefinition : MaterialDefinition
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AssetDefinition" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public AssetDefinition(string name) : base(name)
        {
        }

        /// <summary>
        ///     Merge MaterialDefinition
        /// </summary>
        /// <param name="definition"></param>
        public IMaterialDefinition Merge(IMaterialDefinition definition)
        {
            if (definition.Name != Name)
                throw new ArgumentException("Cannot merge material definition with different name.");
            if (definition.Code != Code)
                throw new ArgumentException("Cannot merge material definition with different code.");

            return new MaterialDefinition(definition.Name)
            {
                Code = definition.Code,
                Headers = new List<IHeaderDefinition>(definition.Headers).Concat(Headers).ToList()
            };
        }
    }
}