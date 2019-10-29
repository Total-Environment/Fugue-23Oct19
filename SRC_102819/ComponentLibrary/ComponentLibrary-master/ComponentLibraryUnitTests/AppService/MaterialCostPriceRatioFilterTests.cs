using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.AppService
{
	public class MaterialCostPriceRatioFilterTests
	{
		/// <summary>
		/// </summary>
		[Fact]
		public void Filter_ShouldReturnFilteresList_WhenInputListIsPassed()
		{
			var costPriceRatioList = new CostPriceRatioList(

				new List<CostPriceRatio>
				{
					new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()),
						new DateTime(2017,05,20), ComponentType.Material, "x", "y", null, "ALM0001",
						null),
					new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()),
						new DateTime(2017,05,20), ComponentType.Material, "x", "y", "a", "ALM0002",
						null),
					new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()),
						new DateTime(2017,05,15), ComponentType.Material, "x", "y", null, "ALM0001",
						null),
					new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()),
						new DateTime(2017,05,20), ComponentType.Material, "x", "y", "a", "ALM0002",
						null),
					new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()),
						new DateTime(2017,05,20), ComponentType.Material, "x", "y", "b", "ALM0002",
						null),
					new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()),
						new DateTime(2017,05,20), ComponentType.Material, "a", "b", null, "ALM0002",
						null),
				});

			var materialCostPriceRatioFilter = new MaterialCostPriceRatioFilter();
			var result = materialCostPriceRatioFilter.Filter(costPriceRatioList, null);
			result.costPriceRatios.Count.ShouldBeEquivalentTo(2);
		}

		[Fact]
		public void Filter_ShouldReturnFilteresList_WhenInputListAndProjectIdIsPassed()
		{
			var costPriceRatioList = new CostPriceRatioList(

				new List<CostPriceRatio>
				{
					new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()),
						new DateTime(2017,05,20), ComponentType.Material, "x", "y", null, "ALM0001",
						null),
					new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()),
						new DateTime(2017,05,20), ComponentType.Material, "x", "y", "a", "ALM0002",
						null),
					new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()),
						new DateTime(2017,05,15), ComponentType.Material, "x", "y", null, "ALM0001",
						null),
					new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()),
						new DateTime(2017,05,20), ComponentType.Material, "x", "y", "a", "ALM0002",
						null),
					new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()),
						new DateTime(2017,05,20), ComponentType.Material, "x", "y", null, "ALM0002",
						"p1"),
					new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()),
						new DateTime(2017,05,19), ComponentType.Material, "x", "y", null, "ALM0002",
						"p1"),
					new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()),
						new DateTime(2017,05,20), ComponentType.Material, "x", "y", "a", "ALM0002",
						"p1"),
					new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()),
						new DateTime(2017,05,18), ComponentType.Material, "x", "y", "a", "ALM0002",
						"p1")
				});

			var materialCostPriceRatioFilter = new MaterialCostPriceRatioFilter();
			var result = materialCostPriceRatioFilter.Filter(costPriceRatioList, "p1");
			result.costPriceRatios.Count.ShouldBeEquivalentTo(3);
		}
	}
}