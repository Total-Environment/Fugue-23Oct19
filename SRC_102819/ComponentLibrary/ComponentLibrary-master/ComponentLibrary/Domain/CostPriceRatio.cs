using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
	/// <summary>
	/// Represents a cost price ration
	/// </summary>
	public class CostPriceRatio
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CostPriceRatio"/> class.
		/// </summary>
		/// <param name="cprCoefficient">The CPR coefficient.</param>
		/// <param name="appliedFrom">The applied from.</param>
		/// <param name="componentType">Type of the component.</param>
		/// <param name="level1">The level1.</param>
		/// <param name="level2">The level2.</param>
		/// <param name="level3">The level3.</param>
		/// <param name="code">The code.</param>
		/// <param name="projectCode">The project code.</param>
		public CostPriceRatio(CPRCoefficient cprCoefficient, DateTime appliedFrom, ComponentType componentType, string level1,
			string level2, string level3, string code, string projectCode)
		{
			this.CprCoefficient = cprCoefficient;
			this.AppliedFrom = appliedFrom;
			this.ComponentType = componentType;
			this.Level1 = level1;
			this.Level2 = level2;
			this.Level3 = level3;
			this.Code = code;
			this.ProjectCode = projectCode;
		}

		/// <summary>
		/// Gets the CPR coefficient.
		/// </summary>
		/// <value>The CPR coefficient.</value>
		public CPRCoefficient CprCoefficient { get; }

		/// <summary>
		/// Gets the applied from.
		/// </summary>
		/// <value>The applied from.</value>
		public DateTime AppliedFrom { get; }

		/// <summary>
		/// Gets the type of the component.
		/// </summary>
		/// <value>The type of the component.</value>
		public ComponentType ComponentType { get; }

		/// <summary>
		/// Gets the level1.
		/// </summary>
		/// <value>The level1.</value>
		public string Level1 { get; }

		/// <summary>
		/// Gets the level2.
		/// </summary>
		/// <value>The level2.</value>
		public string Level2 { get; }

		/// <summary>
		/// Gets the level3.
		/// </summary>
		/// <value>The level3.</value>
		public string Level3 { get; }

		/// <summary>
		/// Gets the code.
		/// </summary>
		/// <value>The code.</value>
		public string Code { get; }

		/// <summary>
		/// Gets the project code.
		/// </summary>
		/// <value>The project code.</value>
		public string ProjectCode { get; }
	}
}