using System;
using System.Linq;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    ///     Represents a dependent block.
    /// </summary>
    public class DependentBlock
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DependentBlock" /> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public DependentBlock(string[] data)
        {
            Data = data;
        }

        /// <summary>
        ///     Gets the data.
        /// </summary>
        /// <value>
        ///     The data.
        /// </value>
        public string[] Data { get; }

        /// <summary>
        ///     Validates the specified block.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <returns></returns>
        public bool Validate(string[] block)
        {
            if (block.Length != Data.Length)
                return false;
            return !Data.Where((t, i) => !string.Equals(t, block[i], StringComparison.CurrentCultureIgnoreCase)).Any();
        }

        /// <summary>
        /// Compare two Dependent blocks
        /// </summary>
        /// <param name="dependentBlock">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object dependentBlock)
        {
            if (ReferenceEquals(null, dependentBlock)) return false;
            if (dependentBlock.GetType() != GetType()) return false;
            var convertedDependentBlock = (DependentBlock) dependentBlock;
            var data = convertedDependentBlock.Data.ToList();
            var isEqual = Data.ToList().SequenceEqual(data);
            return isEqual;
        }
    }
}