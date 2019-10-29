using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.AppService
{
    public class MaterialBuilderTests
    {
        private const string approved_brands = "approved_brands";
        private const string ApprovedBrands = "Approved Brands";
        private const string BrandCode = "brand_code";
        private const string Brick = "brick";
        private const string Clay = "Clay";
        private const string Date = "Date";
        private const string General = "General";
        private const string Image = "Image";
        private const string manufacturers_name = "manufacturers_name";
        private const string ManufacturersName = "manufacturer's name";
        private const string Material_Level_1 = "material_level_1";
        private const string MaterialCannotBeNull = "Material cannot be null.";
        private const string MaterialCode = "CLY00010";
        private const string MaterialDefinitionCannotBeNull = "Material definition cannot be null.";
        private const string MaterialLevel1 = "material level 1";
        private const string Name = "name";
        private const string Primary = "Primary";
        private const string Purchase = "Purchase";
        private const string PurchaseKey = "purchase";
        private const string Secondry = "Secondry";
        private const string Some = "some";
        private const string SomeDate = "2017-03-11T21:00:00.0000000+05:30";
        private const string SomeThing = "something";
        private const string SomeThingElse = "something else";

        [Fact]
        public async void Build_ShouldBuildAValidMaterial_WhenMaterialAndDefinitionAsPassed()
        {
            var material = GetCreateMaterial();
            var materialDefinition = GetMaterialDefinition();

            var result = await GetMaterialBuilder().BuildAsync(material, materialDefinition);

            var columns = result.Headers.First(h => h.Name == General).Columns;
            columns.First(c => c.Name == MaterialLevel1).Value.Should().BeOfType<string>();
            columns.First(c => c.Name == Name).Value.Should().BeOfType<string>();
            columns.First(c => c.Name == Date).Value.Should().BeOfType<DateTime>();
            columns.First(c => c.Name == Image).Value.Should().BeOfType<object[]>();
            ((object[])columns.First(c => c.Name == Image).Value)[0].Should().BeOfType<string>();
        }

        [Fact]
        public async void Build_ShouldGenerateKeywordsForArray_WhenArrayIsPassedAsColumnData()
        {
            var data = new[] { "something", "something else", "data" };
            var materialWithArray = GetMaterialWithArrayAsColumnData(data);
            var materialDefinitionWithArray = GetMaterialDefinitionWithArrayColumnDefinition();

            var result = await GetMaterialBuilder().BuildAsync(materialWithArray, materialDefinitionWithArray);

            result.SearchKeywords.Should().Equal("something", "something else", "data", MaterialCode);
        }

        [Fact]
        public async void Build_ShouldGenerateKeywordsForBrand_WhenBrandIsPassedAsColumnData()
        {
            var materialWithBrands = GetMaterialWithBrandData();
            var materialDefinitionWithBrands = GetMaterialDefinitionWithBrands();

            var result = await GetMaterialBuilder().BuildAsync(materialWithBrands, materialDefinitionWithBrands);

            result.SearchKeywords.Should().Equal("brand 1", "brand 2", MaterialCode);
        }

        [Fact]
        public async void Build_ShouldReturnMaterialWithMaterialCode()
        {
            const string materialcode = "materialCode";
            var fixture = new Fixture().GenerateOfMaterialCodeGeneratorReturns(materialcode);

            var result = await fixture.Sut().BuildAsync(GetCreateMaterial(), GetMaterialDefinition());

            result.Id.Should().Be(materialcode);
        }

        [Fact]
        public async Task Build_ShouldSetSearchKeywords_BasedOnSearchableColumns()
        {
            var material = GetCreateMaterial();
            var materialDefinition = GetMaterialDefinition();

            var result = await GetMaterialBuilder().BuildAsync(material, materialDefinition);

            result.SearchKeywords.Contains("brick").Should().Be(true);
        }

        [Fact]
        public void Build_ShouldShouldThrowArgumentException_WhenDataIsNotValid()
        {
            var material = new Material
            {
                Headers = new List<IHeaderData>
                {
                    new HeaderData(General, General)
                    {
                        Columns = new List<IColumnData>
                        {
                            new ColumnData("Short", "Short", "fddfhuiveriwufir")
                        }
                    }
                }
            };
            var materialDefinition = new MaterialDefinition(Some)
            {
                Headers = new List<IHeaderDefinition>
                {
                    new HeaderDefinition(General, General,
                        new List<IColumnDefinition> {new ColumnDefinition("Short", "Short", new BooleanDataType())})
                }
            };

            Func<Task> action = async () => await GetMaterialBuilder().BuildAsync(material, materialDefinition);

            action.ShouldThrow<FormatException>();
        }

        [Fact]
        public void Build_ShouldThrowArgumentException_WhenMaterialDefinitionPassedIsNull()
        {
            var materialBuilder = GetMaterialBuilder();

            Func<Task> action = async () => await materialBuilder.BuildAsync(new Material(), null);

            action.ShouldThrow<ArgumentException>().WithMessage(MaterialDefinitionCannotBeNull);
        }

        [Fact]
        public void Build_ShouldThrowArgumentException_WhenMaterialPassedIsNotValidMaterial()
        {
            const string invalidMaterialErrorMessage = "errorMessage";
            var valueTuple = new Tuple<bool, string>(false, invalidMaterialErrorMessage);
            var fixture =
                new Fixture().ValidateOfMaterialValidatorReturns(valueTuple);

            Func<Task> action = async () => await fixture.Sut().BuildAsync(GetCreateMaterial(), GetMaterialDefinition());

            action.ShouldThrow<ArgumentException>().WithMessage(invalidMaterialErrorMessage);
        }

        [Fact]
        public void Build_ShouldThrowArgumentException_WhenMaterialPassedIsNull()
        {
            var materialBuilder = GetMaterialBuilder();

            Func<Task> action = async () => await materialBuilder.BuildAsync(null, new MaterialDefinition(Clay));

            action.ShouldThrow<ArgumentException>().WithMessage(MaterialCannotBeNull);
        }

        private static MaterialBuilder GetMaterialBuilder()
        {
            var mock = new Mock<IMaterialCodeGenerator>();
            mock.Setup(m => m.Generate(It.IsAny<string>(), It.IsAny<IMaterial>())).ReturnsAsync(MaterialCode);
            return new MaterialBuilder(new Mock<IHeaderColumnDataValidator>().Object, mock.Object);
        }

        private IMaterial GetCreateMaterial()
        {
            return new Material
            {
                Group = Clay,
                Headers = new List<IHeaderData>
                {
                    new HeaderData("Classification", "classification")
                    {
                        Columns = new List<IColumnData>
                        {
                            new ColumnData("Material Level 2", "material_level_2", Primary),
                        }
                    },
                    new HeaderData(General, General)
                    {
                        Columns = new List<IColumnData>
                        {
                            new ColumnData(MaterialLevel1, MaterialLevel1, Primary),
                            new ColumnData(Name, Name, Brick),
                            new ColumnData(Date, Date, SomeDate),
                            new ColumnData(Image, Image, new[] {SomeThing, SomeThingElse})
                        }
                    }
                }
            };
        }

        private IMaterialDefinition GetMaterialDefinition()
        {
            return new MaterialDefinition(Clay)
            {
                Headers = new List<IHeaderDefinition>()
                {
                    new HeaderDefinition("Classification","classification", new List<IColumnDefinition>
                    {
                        new ColumnDefinition(
                            "Material Level 2","material_level_2",
                            new ConstantDataType(Primary))
                    }),
                    new HeaderDefinition(General,General, new List<IColumnDefinition>
                    {
                        new ColumnDefinition(
                            MaterialLevel1, MaterialLevel1,
                            new MasterDataDataType(new MasterDataList(Material_Level_1, new List<MasterDataValue>
                            {
                                new MasterDataValue(Primary),
                                new MasterDataValue(Secondry)
                            }))),
                        new ColumnDefinition(
                            Name, Name,
                            new StringDataType(), true),
                        new ColumnDefinition(
                            Date, Date,
                            new DateDataType()),
                        new ColumnDefinition(
                            Image, Image,
                            new ArrayDataType(new StringDataType())),
                    })
                }
            };
        }

        private IMaterialDefinition GetMaterialDefinitionWithArrayColumnDefinition()
        {
            return new MaterialDefinition(Clay)
            {
                Headers = new List<IHeaderDefinition>()
                {
                    new HeaderDefinition("Classification","classification", new List<IColumnDefinition>
                    {
                        new ColumnDefinition(
                            "Material Level 2","material_level_2",
                            new ConstantDataType(Primary))
                    }),
                    new HeaderDefinition(General,General, new List<IColumnDefinition>
                    {
                        new ColumnDefinition(
                            MaterialLevel1,
                            Material_Level_1,
                            new ArrayDataType(new StringDataType()),
                            true)
                    })
                }
            };
        }

        private IMaterialDefinition GetMaterialDefinitionWithBrands()
        {
            var brandDefinition = new BrandDefinition("Generic Brand", new List<ISimpleColumnDefinition>
            {
                new SimpleColumnDefinition(Name, "name", new StringDataType(), true),
                new SimpleColumnDefinition(BrandCode, "brand_code", new AutogeneratedDataType("Brand Code")),
                new SimpleColumnDefinition(ManufacturersName, manufacturers_name, new StringDataType())
            });
            return new MaterialDefinition(Clay)
            {
                Headers = new List<IHeaderDefinition>()
                {
                    new HeaderDefinition("Classification", "classification", new List<IColumnDefinition>
                    {
                        new ColumnDefinition(
                            "Material Level 2", "material_level_2",
                            new ConstantDataType(Primary))
                    }),
                    new HeaderDefinition(Purchase, PurchaseKey, new List<IColumnDefinition>
                    {
                        new ColumnDefinition(
                            ApprovedBrands,
                            approved_brands,
                            new ArrayDataType(new BrandDataType(brandDefinition, "BCL",
                                new Mock<IBrandCodeGenerator>().Object,
                                new Mock<ICounterRepository>().Object)),
                            true)
                    })
                }
            };
        }

        private IMaterial GetMaterialWithArrayAsColumnData(string[] data)
        {
            return new Material()
            {
                Group = Clay,
                Headers = new List<IHeaderData>
                {
                    new HeaderData("Classification", "classification")
                    {
                        Columns = new List<IColumnData>
                        {
                            new ColumnData("Material Level 2", "material_level_2", Primary),
                        }
                    },
                    new HeaderData(General, General)
                    {
                        Columns = new List<IColumnData>
                        {
                            new ColumnData(MaterialLevel1, Material_Level_1, data),
                        }
                    }
                }
            };
        }

        private IMaterial
            GetMaterialWithBrandData()
        {
            return new Material
            {
                Group = Clay,
                Headers = new List<IHeaderData>
                {
                    new HeaderData("Classification", "classification")
                    {
                        Columns = new List<IColumnData>
                        {
                            new ColumnData("Material Level 2", "material_level_2", Primary),
                        }
                    },
                    new HeaderData(Purchase, PurchaseKey)
                    {
                        Columns = new List<IColumnData>
                        {
                            new ColumnData(ApprovedBrands, approved_brands, new List<Dictionary<string, object>>
                            {
                                new Dictionary<string, object>
                                {
                                    {
                                        "columns", new List<object>
                                        {
                                            new Dictionary<string, object>
                                            {
                                                {"key", "brand_code"},
                                                {"name", "Brand Code"},
                                                {"value", "BCL000010"}
                                            },
                                            new Dictionary<string, object>
                                            {
                                                {"key", "name"},
                                                {"name", "Name"},
                                                {"value", "brand 1"}
                                            },
                                            new Dictionary<string, object>
                                            {
                                                {"key", "manufacturer's name"},
                                                {"name", "Manufacturer's Name"},
                                                {"value", "name 1"}
                                            }
                                        }
                                    },
                                },
                                new Dictionary<string, object>
                                {
                                    {
                                        "columns", new List<object>
                                        {
                                            new Dictionary<string, object>
                                            {
                                                {"key", "brand_code"},
                                                {"name", "Brand Code"},
                                                {"value", "BCL000002"}
                                            },
                                            new Dictionary<string, object>
                                            {
                                                {"key", "name"},
                                                {"name", "Name"},
                                                {"value", "brand 2"}
                                            },
                                            new Dictionary<string, object>
                                            {
                                                {"key", "manufacturer's name"},
                                                {"name", "Manufacturer's Name"},
                                                {"value", "name 2"}
                                            }
                                        }
                                    },
                                },
                            })
                        }
                    }
                }
            };
        }

        private class Fixture
        {
            private readonly Mock<IMaterialCodeGenerator> _mockMaterialCodeGenerator = new Mock<IMaterialCodeGenerator>();
            private readonly Mock<IHeaderColumnDataValidator> _mockMaterialValidator = new Mock<IHeaderColumnDataValidator>();

            public Fixture GenerateOfMaterialCodeGeneratorReturns(string materialcode)
            {
                _mockMaterialCodeGenerator.Setup(m => m.Generate(It.IsAny<string>(), It.IsAny<IMaterial>()))
                    .ReturnsAsync(materialcode);
                return this;
            }

            public MaterialBuilder Sut()
            {
                return new MaterialBuilder(_mockMaterialValidator.Object, _mockMaterialCodeGenerator.Object);
            }

            public Fixture ValidateOfMaterialValidatorReturns(Tuple<bool, string> valueTuple)
            {
                _mockMaterialValidator.Setup(m => m.Validate(It.IsAny<IEnumerable<IHeaderDefinition>>(), It.IsAny<IEnumerable<IHeaderData>>()))
                    .Returns(valueTuple);
                return this;
            }
        }
    }
}