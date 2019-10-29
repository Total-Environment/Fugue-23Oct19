using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DomainTests.DataTypes
{
    public class CheckListDataTypeTests
    {
        private class Fixture
        {
            private readonly Mock<ICheckListRepository> _mockRepository = new Mock<ICheckListRepository>();

            public CheckListDataType SystemUnderTest()
            {
                return new CheckListDataType(_mockRepository.Object);
            }

            public Fixture WithCheckListId(string id)
            {
                _mockRepository.Setup(m => m.GetById(id))
                    .ReturnsAsync(new CheckList { CheckListId = id });
                return this;
            }

            public Fixture WithoutCheckListId(string id)
            {
                _mockRepository.Setup(m => m.GetById(id)).Throws(new ResourceNotFoundException("s"));
                return this;
            }
        }

        [Fact]
        public void It_ShouldBeOfTypeIDataType()
        {
            var dt = new Fixture().SystemUnderTest();
            dt.Should().BeAssignableTo<IDataType>();
        }

        [Fact]
        public async void Parse_ShouldReturnACheckListValue_WhenPassedId()
        {
            const string id = "MNS0001";
            var dt = new Fixture().WithCheckListId(id).SystemUnderTest();
            ((CheckListValue)await dt.Parse(id)).Id.Should().Be(id);
        }

        [Fact]
        public async void Parse_ShouldReturnCheckListValue_WhenPassedADictionaryWithId()
        {
            const string id = "MNS0001";
            var input = new Dictionary<string, object> { { "Id", id } };
            var dt = new Fixture().WithCheckListId(id).SystemUnderTest();
            ((CheckListValue)await dt.Parse(input)).Id.Should().Be(id);
        }

        [Fact]
        public async void Parse_ShouldThrowFormatExceptioN_WhenChecklistIdDoesNotExist()
        {
            const string id = "sattar";
            var dt = new Fixture().WithoutCheckListId(id).SystemUnderTest();
            Func<Task> action = async () => await dt.Parse(id);
            action.ShouldThrow<FormatException>().WithMessage("Checklist with id sattar does not exist.");
        }
    }
}