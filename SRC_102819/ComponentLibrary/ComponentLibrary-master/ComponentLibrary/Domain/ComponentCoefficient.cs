using System.Collections.Generic;
using System.Linq;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    /// </summary>
    public class ComponentCoefficient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentCoefficient"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="coefficient">The coefficient.</param>
        /// <param name="wastagePercentages">The wastagePercentages.</param>
        /// <param name="unitOfMeasure">The unit of measure.</param>
        /// <param name="name">The name.</param>
        /// <param name="componentType"></param>
        public ComponentCoefficient(string code, double coefficient, IEnumerable<WastagePercentage> wastagePercentages,
            string unitOfMeasure, string name, ComponentType componentType)
        {
            Code = code;
            Coefficient = coefficient;
            WastagePercentages = wastagePercentages;
            UnitOfMeasure = unitOfMeasure;
            Name = name;
            ComponentType = componentType;
        }

        /// <summary>
        /// Gets the type of the component.
        /// </summary>
        /// <value>The type of the component.</value>
        public ComponentType ComponentType { get; }

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
        /// Gets the code.
        /// </summary>
        /// <value>The code.</value>
        public string Code { get; }

        /// <summary>
        /// Gets the coefficient.
        /// </summary>
        /// <value>The coefficient.</value>
        public double Coefficient { get; }

        /// <summary>
        /// Gets the wastagePercentages.
        /// </summary>
        /// <value>The wastagePercentages.</value>
        public IEnumerable<WastagePercentage> WastagePercentages { get; }

        /// <summary>
        /// Gets the total wastage percentage.
        /// </summary>
        /// <value>The total wastage percentage.</value>
        public double TotalWastagePercentage
        {
            get { return WastagePercentages.Sum(w => w.Value); }
        }

        /// <summary>
        /// Gets the total quantity.
        /// </summary>
        /// <value>The total quantity.</value>
        public double TotalQuantity => Coefficient + (Coefficient * TotalWastagePercentage / 100);
    }
}