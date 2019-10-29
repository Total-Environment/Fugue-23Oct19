using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Driver;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.DataAdaptors;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.RepositoryPortTests
{
    public class BrandDefinitionRepositoryTests
    {
        [Fact]
        public async Task FindBy_ShouldReturnBrand_WhenPassedValidBrandName()
        {
            Cache.Clear();
            var dataTypeFactoryMock = new Mock<ISimpleDataTypeFactory>();
            dataTypeFactoryMock.Setup(d => d.Construct(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(new IntDataType());
            var mongoCollectionMock = new Mock<IMongoCollection<BrandDefinitionDao>>();
            var brandRepository = new BrandDefinitionRepository(mongoCollectionMock.Object, dataTypeFactoryMock.Object);
            IBrandDefinition brandDefinition = new BrandDefinition("Generic Brands", new List<ISimpleColumnDefinition>
            {
                new SimpleColumnDefinition("Maximum No in Days","Maximum No in Days", new IntDataType())
            });
            var brandDefinitionDao = new BrandDefinitionDao(brandDefinition);
            TestHelper.MockCollectionWithExisting(mongoCollectionMock, brandDefinitionDao);
            var brand = await brandRepository.FindBy("Generic Brands");
            brand.Name.Should().Be("Generic Brands");
        }

        [Fact]
        public void FindBy_ShouldThrowResourceNotFoundException_WhenBrandDefinitionIsNull()
        {
            Cache.Clear();
            var dataTypeFactoryMock = new Mock<ISimpleDataTypeFactory>();
            dataTypeFactoryMock.Setup(d => d.Construct(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(new IntDataType());
            var mongoCollectionMock = new Mock<IMongoCollection<BrandDefinitionDao>>();
            var brandRepository = new BrandDefinitionRepository(mongoCollectionMock.Object, dataTypeFactoryMock.Object);
            TestHelper.MockCollectionWithExisting(mongoCollectionMock, null);
            Func<Task<IBrandDefinition>> result = async () => await brandRepository.FindBy("Generic Brands");
            result.ShouldThrow<ResourceNotFoundException>("Brand Definition not found.");
        }
    }
}