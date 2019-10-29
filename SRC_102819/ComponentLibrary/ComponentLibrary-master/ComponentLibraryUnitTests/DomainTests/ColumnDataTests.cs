using System;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DomainTests
{
	public class ColumnDataTests
	{
		[Fact]
		public void ColumnData_ShouldBeAssignableToIColumnData()
		{
			var sut = new ColumnData("sattar", "sattar", 1);
			sut.Should().BeAssignableTo<IColumnData>();
		}

		[Fact]
		public void ColumnData_WhenPassedNameAndData_ShouldHaveNameAndData()
		{
			const string name = "sattar";
			object data = 1;
			var column = new ColumnData(name, name, data);
			column.Name.Should().Be(name);
			column.Value.Should().Be(data);
		}

		[Fact]
		public void SetKey_ShouldThrowArgumentException_WhenPassedNull()
		{
			var column = new ColumnData("sattar", "sattar", 1);
			Action act = () => column.Key = null;
			act.ShouldThrow<ArgumentException>().WithMessage("Key is required.");
		}
	}
}