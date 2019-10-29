using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto
{
    /// <summary>
    /// Represents a dependent block dto.
    /// </summary>
    public class DependentBlockDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependentBlockDto"/> class.
        /// </summary>
        /// <param name="dependentBlock">The dependent block.</param>
        public DependentBlockDto(DependentBlock dependentBlock)
        {
            Data = dependentBlock.Data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependentBlockDto"/> class.
        /// </summary>
        public DependentBlockDto()
        {
            
        }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public string[] Data { get; set; }

        /// <summary>
        /// Domains this instance.
        /// </summary>
        /// <returns></returns>
        public DependentBlock Domain()
        {   
            return new DependentBlock(Data);
        }
    }
}