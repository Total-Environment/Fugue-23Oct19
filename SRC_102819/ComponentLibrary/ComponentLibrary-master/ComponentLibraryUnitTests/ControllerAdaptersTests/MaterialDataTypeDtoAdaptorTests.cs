using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.ControllerAdaptersTests
{
    public class MaterialDataTypeDtoAdaptorTests
    {
        private const string Brick = "brick";
        private const string Clay = "Clay";
        private const string Date = "Date";
        private const string General = "General";
        private const string Image = "Image";
        private const string MaterialLevel1 = "material level 1";
        private const string Name = "name";
        private const string Primary = "Primary";
        private const string SomeDate = "2017-03-11T21:00:00.0000000+05:30";
        private const string SomeThing = "something";
        private const string Material_Level_1 = "material_level_1";
        private const string Secondary = "Secondary";
        private const string SomeThingElse = "something else";

        [Fact]
        public void FromMaterial_ShouldReturnMaterialDataTypeDto_WhenMaterialIsPassed()
        {
            var materialWithDefinition = GetMaterialWithDefinition();

            var result = MaterialDataTypeDtoAdaptor.FromMaterial(materialWithDefinition);

            result.Headers.First().Columns.First().DataType.Name.Should().Be("MasterData");
        }

        private IMaterial GetMaterialWithDefinition()
        {
            return new Material()
            {
                Group = Clay,
                Headers = new List<IHeaderData>
                {
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
                },
                ComponentDefinition = GetMaterialDefinition()
            };
        }

        private IMaterialDefinition GetMaterialDefinition()
        {
            return new MaterialDefinition(Clay)
            {
                Headers = new List<IHeaderDefinition>()
                {
                    new HeaderDefinition(General,General, new List<IColumnDefinition>
                    {
                        new ColumnDefinition(
                            MaterialLevel1,MaterialLevel1,
                            new MasterDataDataType(new MasterDataList(Material_Level_1, new List<MasterDataValue>
                            {
                                new MasterDataValue(Primary),
                                new MasterDataValue(Secondary)
                            }))),
                        new ColumnDefinition(
                            Name,Name,
                            new StringDataType(),true),
                        new ColumnDefinition(
                            Date,Date,
                            new DateDataType()),
                        new ColumnDefinition(
                            Image,Image,
                            new ArrayDataType(new StringDataType())),
                    })
                }
            };
        }
    }
}