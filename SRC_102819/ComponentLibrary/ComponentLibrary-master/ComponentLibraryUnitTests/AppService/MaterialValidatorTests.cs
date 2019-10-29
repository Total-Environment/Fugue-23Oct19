using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.AppService
{
    public class MaterialValidatorTests
    {
        [Fact]
        public void Validate_ShouldCallDependencyValidator_WhenMaterialDataIsPassed()
        {
            var material = GetValidMaterial();
            var materialDefinition = GetMaterialDefinition();
            Mock<IDependencyValidator> mockDependencyValidator = new Mock<IDependencyValidator>();
            mockDependencyValidator.Setup(
                m =>
                    m.Validate(It.Is<IDependencyDefinition>(d => d.Name == "general"),
                        It.Is<IHeaderData>(h => h.Name == "General"))).Returns(new Tuple<bool, string>(false, "Some message"));

            var result = GetMaterialValidator(mockDependencyValidator.Object).Validate(materialDefinition.Headers, material.Headers);

            result.Item1.Should().Be(false);
            result.Item2.Should().Be("Some message");
        }

        [Fact]
        public void Validate_ShouldReturnTrue_WhenDataKeyMatchesDefinitionKey()
        {
            var material = GetValidMaterial();
            var materialDefinition = GetMaterialDefinition();
            Mock<IDependencyValidator> mockDependencyValidator = new Mock<IDependencyValidator>();
            mockDependencyValidator.Setup(
                m =>
                    m.Validate(It.IsAny<IDependencyDefinition>(), It.IsAny<IHeaderData>())).Returns(new Tuple<bool, string>(true, "Some message"));

            var result = GetMaterialValidator(mockDependencyValidator.Object).Validate(materialDefinition.Headers, material.Headers);

            result.Item1.Should().Be(true);
        }

        [Fact]
        public void Validate_ShouldThrowArgumentException_WhenColumnsAreInvalid()
        {
            var material = GetCreateMaterialWithInvalidColumns();
            var materialDefinition = GetMaterialDefinition();

            var result = GetMaterialValidator().Validate(materialDefinition.Headers, material.Headers);

            result.Item1.Should().Be(false);
            result.Item2.Should().Be("Column(s) not found, keys did not match with definition: material level 2, names");
        }

        [Fact]
        public void Validate_ShouldThrowFormatException_WhenHeaderAreInvalid()
        {
            var material = GetCreateMaterialWithInValidHeaderName();
            var materialDefinition = GetMaterialDefinition();

            var result = GetMaterialValidator().Validate(materialDefinition.Headers, material.Headers);

            result.Item1.Should().Be(false);
            result.Item2.Should().Be("Header(s) not found, keys did not match with definition: Wrong General");
        }

        private IMaterial GetCreateMaterialWithInvalidColumns()
        {
            return new Material
            {
                Group = "Clay",
                Headers = new List<IHeaderData>
                {
                    new HeaderData("General", "general")
                    {
                        Columns = new List<IColumnData>
                        {
                            new ColumnData("Material Level 2", "material level 2", "Primary"),
                            new ColumnData("Names", "names", "brick"),
                            new ColumnData("Date", "date", "2017-03-11T21:00:00.0000000+05:30"),
                            new ColumnData("Image", "image", new[]{"something", "something else"})
                        }
                    }
                }
            };
        }

        private IMaterial GetCreateMaterialWithInValidHeaderName()
        {
            return new Material
            {
                Group = "Clay",
                Headers = new List<IHeaderData>
                {
                    new HeaderData("General", "Wrong General")
                }
            };
        }

        private IMaterialDefinition GetMaterialDefinition()
        {
            return new MaterialDefinition("Clay")
            {
                Headers = new List<IHeaderDefinition>()
                {
                    new HeaderDefinition("General different Name","general", new List<IColumnDefinition>
                    {
                        new ColumnDefinition(
                            "material level 1", "material_level_1",
                            new MasterDataDataType(new MasterDataList("material_level_1", new List<MasterDataValue>
                            {
                                new MasterDataValue("Primary"),
                                new MasterDataValue("Secondry")
                            }))),
                        new ColumnDefinition(
                            "Name Different Name", "name",
                            new StringDataType()),
                        new ColumnDefinition(
                            "Date",  "date",
                            new DateDataType()),
                        new ColumnDefinition(
                            "Image", "image",
                            new ArrayDataType(new StringDataType())),
                    })
                    {
                        Dependency = new List<IDependencyDefinition>()
                        {
                            new DependencyDefinition("general", new List<string>()
                            {
                                 "material level 1"
                            })
                        }
                    }
                }
            };
        }

        private IHeaderColumnDataValidator GetMaterialValidator(IDependencyValidator dependencyValidator = null)
        {
            dependencyValidator = dependencyValidator ?? new Mock<IDependencyValidator>().Object;
            return new HeaderColumnDataValidator(dependencyValidator);
        }

        private IMaterial GetValidMaterial()
        {
            return new Material
            {
                Group = "Clay",
                Headers = new List<IHeaderData>
                {
                    new HeaderData("General", "general")
                    {
                        Columns = new List<IColumnData>
                        {
                            new ColumnData("Material Level 1", "material_level_1", "Primary"),
                            new ColumnData("Name", "name", "brick"),
                            new ColumnData("Date", "date", "2017-03-11T21:00:00.0000000+05:30"),
                            new ColumnData("Image", "image", new[]{"something", "something else"})
                        }
                    }
                }
            };
        }
    }
}