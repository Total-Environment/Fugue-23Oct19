using System;
using System.Collections.Generic;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DomainTests
{
	public class CostPriceRatioTests
	{
		[Fact]
		public void CostPriceRatio_ShouldTakeListOfCPRCoefficientsAndAppliedOnAndComponentTypeAndComponentCodeAndProjectCode()
		{
			var cprCoefficient = new CPRCoefficient(new List<IColumnData>());
			var date = DateTime.Today;
			var cpr = new CostPriceRatio(cprCoefficient, date, ComponentType.Material, string.Empty, string.Empty, string.Empty, "ALM00001", "POARR1");
			cpr.CprCoefficient.ShouldBeEquivalentTo(cprCoefficient);
			cpr.AppliedFrom.ShouldBeEquivalentTo(date);
			cpr.Code.ShouldBeEquivalentTo("ALM00001");
			cpr.ComponentType.ShouldBeEquivalentTo(ComponentType.Material);
			cpr.ProjectCode.ShouldBeEquivalentTo("POARR1");
		}

		[Fact]
		public void CostPriceRatio_ShouldTakeListOfCPRCoefficientsAndAppliedOnAndComponentTypeAndComponentCode()
		{
			var cprCoefficient = new CPRCoefficient(new List<IColumnData> { new ColumnData("profit", "profit", 10.4) });
			var date = DateTime.Today;
			var cpr = new CostPriceRatio(cprCoefficient, date, ComponentType.Material, string.Empty, string.Empty, string.Empty, "ALM00001", string.Empty);
			cpr.CprCoefficient.ShouldBeEquivalentTo(cprCoefficient);
			cpr.AppliedFrom.ShouldBeEquivalentTo(date);
			cpr.Code.ShouldBeEquivalentTo("ALM00001");
			cpr.ComponentType.ShouldBeEquivalentTo(ComponentType.Material);
		}

		[Fact]
		public void CostPriceRatio_ShouldTakeListOfCPRCoefficientsAndAppliedOnAndComponentTypeAndThreeLevels()
		{
			var cprCoefficient = new CPRCoefficient(new List<IColumnData> { new ColumnData("profit", "profit", 10.4) });
			var date = DateTime.Today;
			var cpr = new CostPriceRatio(cprCoefficient, date, ComponentType.Material, "level 1", "level 2", null, String.Empty, String.Empty);
			cpr.CprCoefficient.ShouldBeEquivalentTo(cprCoefficient);
			cpr.AppliedFrom.ShouldBeEquivalentTo(date);
			cpr.ComponentType.ShouldBeEquivalentTo(ComponentType.Material);
			cpr.Level1.ShouldBeEquivalentTo("level 1");
			cpr.Level2.ShouldBeEquivalentTo("level 2");
			cpr.Level3.Should().BeNull();
		}
	}
}