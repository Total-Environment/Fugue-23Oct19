using System;
using System.Linq;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <summary>
	/// </summary>
	public class CostPriceRatioFilter : ICostPriceRatioFilter
	{
		/// <summary>
		/// </summary>
		/// <param name="costPriceRatioList"></param>
		/// <param name="projectCode"></param>
		/// <returns></returns>
		public CostPriceRatioList Filter(CostPriceRatioList costPriceRatioList, string projectCode)
		{
			var l1FilteredCostPriceRatioList = costPriceRatioList.costPriceRatios
				.Where(m => m.Level2 == null && m.Code == null && m.ProjectCode == null)
				.GroupBy(m => new { m.Level1 })
				.Select(m => m.OrderByDescending(n => n.AppliedFrom).First()).ToList();

			var l1L2FilteredCostPriceRatioList = costPriceRatioList.costPriceRatios
				.Where(m => m.Level2 != null && m.Code == null && m.ProjectCode == null)
				.GroupBy(m => new { m.Level1, m.Level2 })
				.Select(m => m.OrderByDescending(n => n.AppliedFrom).First()).ToList();

			var l1L2CodeFilteredCostPriceRatioList = costPriceRatioList.costPriceRatios
				.Where(m => m.Level2 != null && m.Code != null && m.ProjectCode == null)
				.GroupBy(m => new { m.Level1, m.Level2, m.Code })
				.Select(m => m.OrderByDescending(n => n.AppliedFrom).First()).ToList();

			var concatList = l1FilteredCostPriceRatioList.Concat(l1L2FilteredCostPriceRatioList).Concat(l1L2CodeFilteredCostPriceRatioList).ToList();

			if (projectCode != null)
			{
				var l1L2CodeProjectCodeFilteredCostPriceRatioList = costPriceRatioList.costPriceRatios
					.Where(m => m.ProjectCode == projectCode)
					.GroupBy(m => new { m.Level1, m.Level2, m.Code, m.ProjectCode })
					.Select(m => m.OrderByDescending(n => n.AppliedFrom).First()).ToList();

				concatList = concatList.Concat(l1L2CodeProjectCodeFilteredCostPriceRatioList).ToList();
			}

			return new CostPriceRatioList(concatList);
		}
	}
}