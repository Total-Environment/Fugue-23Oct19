namespace TE.ComponentLibrary.ComponentLibrary.Extensions
{
    /// <summary>
    ///     Extension methods for strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        ///     Determines whether this instance is empty.
        /// </summary>
        /// <param name="anyString">Any string.</param>
        /// <returns>
        ///     <c>true</c> if the specified any string is empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEmpty(this string anyString)
        {
            return string.IsNullOrWhiteSpace(anyString);
        }

        /// <summary>
        ///     Determines whether string has value.
        /// </summary>
        /// <param name="anyString">Any string.</param>
        /// <returns><c>true</c> if any specified string is not empty; otherwise, <c>false</c>.</returns>
        public static bool IsNotEmpty(this string anyString)
        {
            return !string.IsNullOrWhiteSpace(anyString);
        }
    }
}