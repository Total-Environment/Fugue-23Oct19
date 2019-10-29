using System;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    ///     Class BetterKeyNotFoundException.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class BetterKeyNotFoundException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BetterKeyNotFoundException" /> class.
        /// </summary>
        /// <param name="key">The key.</param>
        public BetterKeyNotFoundException(string key)
        {
            Key = key;
        }

        /// <summary>
        ///     Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        public string Key { get; set; }
    }
}