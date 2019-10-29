using System.Collections.Generic;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
    /// <summary>
    /// </summary>
    public class ComponentCoefficientDao
    {
        /// <summary>
        /// Gets the type of the component.
        /// </summary>
        /// <value>The type of the component.</value>
        public ComponentType ComponentType { get; set; }

        /// <summary>
        /// Gets the code.
        /// </summary>
        /// <value>The code.</value>
        public string Code { get; set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets the coefficient.
        /// </summary>
        /// <value>The coefficient.</value>
        public double Coefficient { get; set; }

        /// <summary>
        /// Gets the unit of measure.
        /// </summary>
        /// <value>The unit of measure.</value>
        public string UnitOfMeasure { get; set; }

        /// <summary>
        /// Gets the wastages.
        /// </summary>
        /// <value>The wastages.</value>
        public IEnumerable<WastagePercentage> Wastages { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentCoefficientDao"/> class.
        /// </summary>
        /// <param name="componentCoefficient">The component coefficient.</param>
        public ComponentCoefficientDao(ComponentCoefficient componentCoefficient)
        {
            UnitOfMeasure = componentCoefficient.UnitOfMeasure;
            ComponentType = componentCoefficient.ComponentType;
            Name = componentCoefficient.Name;
            Code = componentCoefficient.Code;
            Coefficient = componentCoefficient.Coefficient;
            Wastages = componentCoefficient.WastagePercentages;
        }

        /// <summary>
        /// To the component coeficient.
        /// </summary>
        /// <returns></returns>
        public ComponentCoefficient ToComponentCoeficient()
        {
            return new ComponentCoefficient(Code, Coefficient, Wastages, UnitOfMeasure, Name, ComponentType);
        }
    }
}