using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DomainTests.DataTypes
{
    public class MoneyDataTypeTests
    {
        [Theory]
        [InlineData("Amount")]
        [InlineData("Currency")]
        public void Parse_ShouldThrowFormatException_WhenNotPassedRequiredKeys(string key)
        {
            var dict = new Dictionary<string, object> { { "Amount", 1000m }, { "Currency", "INR" } };
            dict.Remove(key);
            Func<Task> act = async () => await new MoneyDataType().Parse(dict);
            act.ShouldThrow<FormatException>().WithMessage($"{key} is required.");
        }

        [Theory]
        [InlineData("Amount")]
        public void Parse_ShouldThrowFormatException_WhenAmountIsNotValid(object amount)
        {
            var dict = new Dictionary<string, object> { { "Amount", amount }, { "Currency", "INR" } };
            Func<Task> act = async () => await new MoneyDataType().Parse(dict);
            act.ShouldThrow<FormatException>();
        }

        [Fact]
        public void It_ShouldBeOfTypeIDataType()
        {
            var dt = new MoneyDataType();
            dt.Should().BeAssignableTo<IDataType>();
        }

        [Fact]
        public async void Parse_ShouldReturnMoney_WhenPassedADictionary()
        {
            var dt = new MoneyDataType();
            var result = await dt.Parse(new Dictionary<string, object> { { "Amount", 1000m }, { "Currency", "INR" } });
            result.Should().Be(new MoneyValue(1000, "INR"));
        }

        [Fact]
        public void Parse_ShouldThrowFormatException_WhenPassedANonDictionary()
        {
            var dt = new MoneyDataType();
            Func<Task> act = async () => await dt.Parse(1);
            act.ShouldThrow<FormatException>().WithMessage("Expected an object of type Money. Got 1");
        }
    }
}