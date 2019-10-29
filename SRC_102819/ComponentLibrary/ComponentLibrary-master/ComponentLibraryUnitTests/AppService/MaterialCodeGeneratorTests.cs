using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.AppService
{
    public class MaterialCodeGeneratorTests
    {
        [Fact]
        public void Genarate_ShouldThrowArgumentException_WhenPrefixIsDifferentWithMaterialCode()
        {
            var material = CreateMaterialWithCode("CLY000100");

            Func<Task> action = async () => await new Fixture().SystemUnderTest().Generate("FTN", material);

            action.ShouldThrow<ArgumentException>().WithMessage("Invalid material code: Wrong prefix");
        }

        [Fact]
        public async void Generate_ShouldCallUpdateCounter_WhenMaterialCodeIsGreaterThanCurrentCode()
        {
            var material = CreateMaterialWithCode("CLY000100");
            var fixture = new Fixture().CounterRepoWithCurrentValue(90).CounterRepoUpdateAccepting(100);

            await fixture.SystemUnderTest().Generate("CLY", material);

            fixture.VerifyExpectations();
        }

        [Fact]
        public async void Generate_ShouldReturnMaterialCode_WhenMaterialIsPassedWithoutCode()
        {
            var material = GetMaterialWithoutCode();
            var fixture = new Fixture().CounterRepoWithNextValue(101);

            var materialCode = await fixture.SystemUnderTest().Generate("CLY", material);

            materialCode.Should().Be("CLY000101");
        }

        [Fact]
        public async void Generate_ShouldReturnMaterialCode_WhenValidMaterialCodeIsPassedWithMaterial()
        {
            var material = CreateMaterialWithCode("CLY000100");

            var materialCode = await new Fixture().SystemUnderTest().Generate("CLY", material);

            materialCode.Should().Be("CLY000100");
        }

        [Fact]
        public void Generate_ShouldThrowArgumentException_WhenLastSixCharacterAreNonNumeric()
        {
            var material = CreateMaterialWithCode("CLY00100a");

            Func<Task> action = async () => await new Fixture().SystemUnderTest().Generate("CLY", material);

            action.ShouldThrow<ArgumentException>()
                .WithMessage("Invalid material code: Last six character should be numeric");
        }

        [Fact]
        public void Generate_ShouldThrowArgumentException_WhenLengthOfMaterialCodeIsNot9()
        {
            var material = CreateMaterialWithCode("CLY00100");

            Func<Task> action = async () => await new Fixture().SystemUnderTest().Generate("CLY", material);

            action.ShouldThrow<ArgumentException>().WithMessage("Invalid material code: Length should be 9");
        }

        private static IMaterial CreateMaterialWithCode(string materialCode)
        {
            return new Material
            {
                Headers = new List<IHeaderData>
                {
                    new HeaderData("General", "general")
                    {
                        Columns = new List<IColumnData>
                        {
                            new ColumnData("Material Code", "material_code", materialCode)
                        }
                    }
                }
            };
        }

        private IMaterial GetMaterialWithoutCode()
        {
            return new Material()
            {
                Headers = new List<IHeaderData>
                {
                    new HeaderData("General", "general")
                    {
                        Columns = new List<IColumnData>
                        {
                            new ColumnData("Material Code", "material_code", null)
                        }
                    }
                }
            };
        }

        private class Fixture
        {
            private const string MaterialIdCounterCollection = "Material";
            private readonly List<Action> _expectations = new List<Action>();
            private readonly Mock<ICounterRepository> _mockCounterRepository = new Mock<ICounterRepository>();

            public Fixture CounterRepoUpdateAccepting(int updateValue)
            {
                _expectations.Add(
                    () => _mockCounterRepository.Verify(m => m.Update(updateValue, MaterialIdCounterCollection), Times.Once));
                return this;
            }

            public Fixture CounterRepoWithCurrentValue(int currentValue)
            {
                _mockCounterRepository.Setup(m => m.CurrentValue(MaterialIdCounterCollection)).ReturnsAsync(currentValue);
                return this;
            }

            public Fixture CounterRepoWithNextValue(int counterValue)
            {
                _mockCounterRepository.Setup(m => m.NextValue(MaterialIdCounterCollection)).ReturnsAsync(counterValue);
                return this;
            }

            public MaterialCodeGenerator SystemUnderTest()
            {
                return new MaterialCodeGenerator(new CounterGenerator(_mockCounterRepository.Object));
            }

            public void VerifyExpectations()
            {
                _expectations.ForEach(e => e.Invoke());
            }
        }
    }
}