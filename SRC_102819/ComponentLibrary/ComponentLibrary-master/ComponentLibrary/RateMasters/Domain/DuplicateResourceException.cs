using System;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain
{
    /// <summary>
    /// Exceptions thrown for Duplicate Resources
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class DuplicateResourceException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateResourceException"/> class.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        public DuplicateResourceException(string msg):base (msg)
        {
            
        }
    }
}