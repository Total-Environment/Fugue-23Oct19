using System.Collections.Generic;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs
{
    /// <summary>
    /// </summary>
    public class ComponentCoefficientDto
    {
        /// <summary>
        /// Gets or sets the type of the component.
        /// </summary>
        /// <value>The type of the component.</value>
        public ComponentType ComponentType { get; set; }

        /// <summary>
        /// Gets the unit of measure.
        /// </summary>
        /// <value>The unit of measure.</value>
        public string UnitOfMeasure { get; set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the coefficient.
        /// </summary>
        /// <value>The coefficient.</value>
        public double Coefficient { get; set; }

        /// <summary>
        /// Gets or sets the wastage percentages.
        /// </summary>
        /// <value>The wastage percentages.</value>
        public IEnumerable<WastagePercentage> WastagePercentages { get; set; }

        /// <summary>
        /// Gets the total wastage percentage.
        /// </summary>
        /// <value>The total wastage percentage.</value>
        public double TotalWastagePercentage { get; }

        /// <summary>
        /// Gets the total quantity.
        /// </summary>
        /// <value>The total quantity.</value>
        public double TotalQuantity { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentCoefficientDto"/> class.
        /// </summary>
        public ComponentCoefficientDto()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentCoefficientDto"/> class.
        /// </summary>
        /// <param name="componentCoefficient">The component coefficient.</param>
        public ComponentCoefficientDto(ComponentCoefficient componentCoefficient)
        {
            ComponentType = componentCoefficient.ComponentType;
            Code = componentCoefficient.Code;
            Coefficient = componentCoefficient.Coefficient;
            WastagePercentages = componentCoefficient.WastagePercentages;
            UnitOfMeasure = componentCoefficient.UnitOfMeasure;
            Name = componentCoefficient.Name;
            TotalWastagePercentage = componentCoefficient.TotalWastagePercentage;
            TotalQuantity = componentCoefficient.TotalQuantity;
        }

        /// <summary>
        /// To the domain.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public ComponentCoefficient ToDomain()
        {
            return new ComponentCoefficient(Code, Coefficient, WastagePercentages, UnitOfMeasure,
                Name, ComponentType);
        }
    }
}