using System;
using System.Collections.Generic;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DomainTests
{
    public class MasterDataListTests
    {
        [Fact]
        public void GetValues_ShouldReturnAListOfValues_WhenCalled()
        {
            var masterDataList = new MasterDataList("sattar");
            masterDataList.Values.Should().Equal(new List<MasterDataValue>());
        }

        [Fact]
        public void HasValue_ShouldReturnFalse_WhenCalledWithNonExistentValue()
        {
            var masterDataList = new MasterDataList("abdul");
            masterDataList.HasValueIgnoreCase("sattar").Should().BeFalse();
        }

        [Fact]
        public void HasValue_ShouldReturnTrue_WhenCalledWithExistingValue()
        {
            var masterDataList = new MasterDataList("abdul", new List<MasterDataValue> {new MasterDataValue("sattar")});
            masterDataList.HasValueIgnoreCase("sattar").Should().BeTrue();
        }

        [Fact]
        public void Id_ShouldBeAccessible_WithId()
        {
            var masterDataList = new MasterDataList("Sattar") {Id = "0001"};
            masterDataList.Id.Should().Be("0001");
        }

        [Fact]
        public void MasterDataList_ShouldBeAssignableToIMasterDataList()
        {
            var masterDataList = new MasterDataList("name");
            masterDataList.Should().BeAssignableTo<IMasterDataList>();
        }

        [Fact]
        public void New_ShouldReturnMasterDataList_WhenCalledWithListOfMasterDataValues()
        {
            var masterDataList = new MasterDataList("sattar", new List<MasterDataValue>());
            masterDataList.Values.Should().Equal(new List<MasterDataValue>());
        }

        [Fact]
        public void Parse_ShouldReturnString_WhenCalledWithExistingStringAsObject()
        {
            var masterDataList = new MasterDataList("abdul", new List<MasterDataValue> {new MasterDataValue("sattar")});
            object input = "sattar";
            masterDataList.ParseIgnoreCase(input).Value.Should().Be("sattar");
        }

        [Fact]
        public void Parse_ShouldReturnString_WhenCalledWithExistingValue()
        {
            var masterDataList = new MasterDataList("abdul", new List<MasterDataValue> {new MasterDataValue("sattar")});
            masterDataList.ParseIgnoreCase("sattar").Value.Should().Be("sattar");
        }

        [Fact]
        public void Parse_ShouldThrowFormatException_WhenCalledWithNonExistingString()
        {
            var masterDataList = new MasterDataList("abdul", new List<MasterDataValue> {new MasterDataValue("sattar")});
            Action act = () => masterDataList.ParseIgnoreCase("123");
            act.ShouldThrow<FormatException>();
        }

        [Fact]
        public void Parse_ShouldThrowFormatException_WhenCalledWithNonString()
        {
            var masterDataList = new MasterDataList("abdul");
            Action act = () => masterDataList.ParseIgnoreCase(37);
            act.ShouldThrow<FormatException>();
        }
    }
}