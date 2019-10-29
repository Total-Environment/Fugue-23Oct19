namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    /// </summary>
    public class WastagePercentage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WastagePercentage"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public WastagePercentage(string name, double value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public double Value { get; set; }
    }
}