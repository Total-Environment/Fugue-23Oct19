using System;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository
{
    /// <summary>
    /// Exception thrown when a resource is not found
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class ResourceNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceNotFoundException"/> class.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        public ResourceNotFoundException(string resourceName) : base($"{resourceName} not found.")
        {
        }
    }
}