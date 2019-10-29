using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs
{
	/// <summary>
	/// </summary>
	public class CostPriceRatioDto
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CostPriceRatioDto"/> class.
		/// </summary>
		public CostPriceRatioDto()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CostPriceRatioDto"/> class.
		/// </summary>
		/// <param name="costPriceRatio">The cost price ratio.</param>
		public CostPriceRatioDto(CostPriceRatio costPriceRatio)
		{
			CprCoefficient = new CPRCoefficientDto(costPriceRatio.CprCoefficient);
			ComponentType = costPriceRatio.ComponentType.ToString();
			Code = costPriceRatio.Code;
			ProjectCode = costPriceRatio.ProjectCode;
			AppliedFrom = costPriceRatio.AppliedFrom;
			Level1 = costPriceRatio.Level1;
			Level2 = costPriceRatio.Level2;
			Level3 = costPriceRatio.Level3;
		}

		/// <summary>
		/// Gets or sets the CPR coefficient.
		/// </summary>
		/// <value>The CPR coefficient.</value>
		public CPRCoefficientDto CprCoefficient { get; set; }

		/// <summary>
		/// Gets or sets the applied from.
		/// </summary>
		/// <value>The applied from.</value>
		public DateTime AppliedFrom { get; set; }

		/// <summary>
		/// Gets or sets the type of the component.
		/// </summary>
		/// <value>The type of the component.</value>
		public string ComponentType { get; set; }

		/// <summary>
		/// Gets or sets the code.
		/// </summary>
		/// <value>The code.</value>
		public string Code { get; set; }

		/// <summary>
		/// Gets or sets the project code.
		/// </summary>
		/// <value>The project code.</value>
		public string ProjectCode { get; set; }

		/// <summary>
		/// Gets or sets the level1.
		/// </summary>
		/// <value>The level1.</value>
		public string Level1 { get; set; }

		/// <summary>
		/// Gets or sets the level2.
		/// </summary>
		/// <value>The level2.</value>
		public string Level2 { get; set; }

		/// <summary>
		/// Gets or sets the level3.
		/// </summary>
		/// <value>The level3.</value>
		public string Level3 { get; set; }

		/// <summary>
		/// To the domain.
		/// </summary>
		/// <returns></returns>
		public CostPriceRatio ToDomain()
		{
			ComponentType componentType;
			if (Enum.TryParse(ComponentType, true, out componentType))
			{
				return new CostPriceRatio(CprCoefficient.ToDomain(), AppliedFrom, componentType, Level1,
					Level2, Level3, Code, ProjectCode);
			}
			else
			{
				throw new ArgumentException("Invalid component type.");
			}
		}
	}
}