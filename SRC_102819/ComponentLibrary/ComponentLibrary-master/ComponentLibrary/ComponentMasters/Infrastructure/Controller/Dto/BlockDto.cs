using System;
using System.Collections.Generic;
using System.Linq;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto
{
    /// <summary>
    /// Structure used for dependency definitions
    /// </summary>
    /// <seealso cref="System.IComparable" />
    public class BlockDto : IComparable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockDto"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public BlockDto(string name)
        {
            Name = name;
            Children = new List<BlockDto>();
        }

        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        public List<BlockDto> Children { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Adds given list of blocks as the child.
        /// </summary>
        /// <param name="block">The block.</param>
        public void AddChild(string[] block)
        {
            if (!block.Any()) return;
            var child = Children.FirstOrDefault(c => c.Name.Equals(block[0]));
            if (child != null)
            {
                child.AddChild(block.Skip(1).ToArray());
            }
            else
            {
                var newBlock = new BlockDto(block[0] ?? "null");
                newBlock.AddChild(block.Skip(1).ToArray());
                Children.Add(newBlock);
                Children.Sort();
            }
        }

        int IComparable.CompareTo(object obj)
        {
            if (obj is BlockDto)
            {
                var compared = obj as BlockDto;
                return String.Compare(Name, compared.Name, StringComparison.Ordinal);
            }
            throw new InvalidOperationException("Cannot compare.");
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is BlockDto)) return false;
            var that = (BlockDto)obj;
            return that.Name.Equals(Name);
        }
    }
}