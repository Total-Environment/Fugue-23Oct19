using System;
using System.Collections.Generic;
using System.Linq;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto
{
    /// <summary>
    /// The Compressed Format of the Dependency Definition
    /// </summary>
    public class DependencyDefinitionCompressedDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyDefinitionCompressedDto"/> class.
        /// </summary>
        /// <param name="dependencyDefinition">The dependency definition.</param>
        /// <exception cref="ArgumentException">Domain Object is null.</exception>
        public DependencyDefinitionCompressedDto(DependencyDefinition dependencyDefinition)
        {
            if (dependencyDefinition == null)
                throw new ArgumentException("Domain Object is null.");
            Name = dependencyDefinition.Name;
            ColumnList = dependencyDefinition.ColumnList;
            Block = new BlockDto("Dependency");
            dependencyDefinition.Blocks.ToList().ForEach(b => Block.AddChild(b.Data));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyDefinitionCompressedDto"/> class.
        /// </summary>
        public DependencyDefinitionCompressedDto()
        {
        }

        /// <summary>
        /// Gets or sets the block.
        /// </summary>
        /// <value>
        /// The block.
        /// </value>
        public BlockDto Block { get; set; }

        /// <summary>
        /// Gets or sets the column list.
        /// </summary>
        /// <value>
        /// The column list.
        /// </value>
        public List<string> ColumnList { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
    }
}