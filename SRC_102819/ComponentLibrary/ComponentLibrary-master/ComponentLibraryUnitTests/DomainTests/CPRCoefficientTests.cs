using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DomainTests
{
	public class CPRCoefficientTests
	{
		[Fact]
		public void CPRCoefficient_ShouldTakeNameAndValue()
		{
			var columns = new List<IColumnData> { new ColumnData("profit", "profit", 10) };
			var cprCoefficient = new CPRCoefficient(columns);
			cprCoefficient.Columns.ShouldAllBeEquivalentTo(columns);
		}

		[Fact]
		public void CPRCoefficient_ShouldReturnCalculatedCPR_WhenGivenListOfCoefficients()
		{
			var columns = new List<IColumnData>
			{
				new ColumnData("profit", "profit",  new UnitValue(10.4,"%")),
				new ColumnData("Design Fee", "Design Fee",  new UnitValue(10.4,"%"))
			};
			var cprCoefficient = new CPRCoefficient(columns);
			cprCoefficient.CPR.ShouldBeEquivalentTo(1.21);
		}
	}
}