using System;
using System.Collections.Generic;
using System.Linq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
	/// <summary>
	/// </summary>
	public class CostPriceRatioDao : Entity
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CostPriceRatioDao"/> class.
		/// </summary>
		/// <param name="costPriceRatio">The cost price ratio.</param>
		public CostPriceRatioDao(CostPriceRatio costPriceRatio)
		{
			AppliedFrom = costPriceRatio.AppliedFrom;
			ComponentType = costPriceRatio.ComponentType;
			Level1 = costPriceRatio.Level1;
			Level2 = costPriceRatio.Level2;
			Level3 = costPriceRatio.Level3;
			Code = costPriceRatio.Code;
			ProjectCode = costPriceRatio.ProjectCode;
			CprCoefficient = costPriceRatio.CprCoefficient;
		}

		/// <summary>
		/// Gets or sets the applied from.
		/// </summary>
		/// <value>The applied from.</value>
		public DateTime AppliedFrom { get; set; }

		/// <summary>
		/// Gets or sets the type of the component.
		/// </summary>
		/// <value>The type of the component.</value>
		public ComponentType ComponentType { get; set; }

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
		/// Gets or sets the CPR coefficient.
		/// </summary>
		/// <value>The CPR coefficient.</value>
		public CPRCoefficient CprCoefficient { get; set; }

		/// <summary>
		/// To the cost price ratio.
		/// </summary>
		/// <param name="costPriceRatioDefinition">The cost price ratio definition.</param>
		/// <returns></returns>
		public CostPriceRatio ToCostPriceRatio(CostPriceRatioDefinition costPriceRatioDefinition)
		{
			var columnDatas = new List<IColumnData>();
			foreach (var simpleColumnDefinition in costPriceRatioDefinition.Columns)
			{
				var columnData = new ColumnData(simpleColumnDefinition.Name, simpleColumnDefinition.Key, null);
				if (CprCoefficient.Columns.Any(c => c.Key == simpleColumnDefinition.Key))
				{
					columnData.Value = CprCoefficient.Columns.First(c => c.Key == simpleColumnDefinition.Key).Value;
				}
				columnDatas.Add(columnData);
			}
			var costPriceRatio = new CostPriceRatio(new CPRCoefficient(columnDatas), this.AppliedFrom, this.ComponentType,
				this.Level1, this.Level2, this.Level3, this.Code, this.ProjectCode);
			return costPriceRatio;
		}
	}
}