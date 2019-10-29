using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Driver;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.RepositoryPortTests
{
    public class MaterialDefinitionRepositoryTest
    {
        [Fact]
        public async void Add_ShouldSave_WhenPassedMaterialDefinition()
        {
            var materialDefinition = new MaterialDefinition("sattar");
            materialDefinition.Headers = new List<IHeaderDefinition>() { new HeaderDefinition("name", "key", new List<IColumnDefinition>()) };
            var fixture = new Fixture().WithEmpty().Accepting(materialDefinition);
            await fixture.SystemUnderTest().Add(materialDefinition);
            fixture.VerifyExpectations();
        }

        [Fact]
        public void Add_ShouldThrowDuplicateResourceException_WhenPassedExistingCode()
        {
            var materialDefinition = new MaterialDefinition("sattar") { Code = "SAT" };
            var sut = new Fixture().WithExisting(new MaterialDefinition("abdul") { Code = "SAT" }).SystemUnderTest();
            Func<Task> act = async () => await sut.Add(materialDefinition);
            act.ShouldThrow<DuplicateResourceException>().WithMessage("Code: SAT already exists.");
        }

        [Fact]
        public void Add_ShouldThrowDuplicateResourceException_WhenPassedExistingName()
        {
            var materialDefinition = new MaterialDefinition("sattar");
            var sut = new Fixture().WithExisting(materialDefinition).SystemUnderTest();
            Func<Task> act = async () => await sut.Add(materialDefinition);
            act.ShouldThrow<DuplicateResourceException>().WithMessage("Name: sattar already exists.");
        }

        [Fact]
        public async void Find_ShouldReturnMaterialDefinition_WhenPassedExistingGroup()
        {
            var materialDefinition = new MaterialDefinition("materialName");
            var sut = new Fixture().WithExisting(materialDefinition).SystemUnderTest();
            (await sut.Find("sattar")).Name.Should().Be("materialName");
        }

        [Fact]
        public void Find_ShouldThrowResourceNotFoundException_WhenPassedNonExistingGroup()
        {
            var sut = new Fixture().WithEmpty().SystemUnderTest();
            Func<Task> act = async () => await sut.Find("sattar");
            act.ShouldThrow<ResourceNotFoundException>();
        }

        [Fact]
        public async void Update_ShouldQueryMongoCollection_WhenPassedMaterialDefinition()
        {
            var materialDefinition = new MaterialDefinition("sattar");
            var fixture = new Fixture();

            await fixture.SystemUnderTest().Update(materialDefinition);

            fixture.MongoCollection()
                .Verify(m => m.ReplaceOneAsync(
                    It.IsAny<FilterDefinition<MaterialDefinitionDao>>(),
                    It.Is<MaterialDefinitionDao>(n => n.Name == "sattar"),
                    null,
                    default(CancellationToken)), Times.Once);
        }

        private class Fixture
        {
            private readonly Mock<IDataTypeFactory> _factoryMock = new Mock<IDataTypeFactory>();

            private readonly Mock<IMongoCollection<MaterialDefinitionDao>> _mockCollection =
                new Mock<IMongoCollection<MaterialDefinitionDao>>();

            private readonly List<Action> _verifications = new List<Action>();

            public Fixture Accepting(MaterialDefinition materialDefinition)
            {
                _verifications.Add(
                    () =>
                        _mockCollection.Verify(
                            m =>
                                m.InsertOneAsync(It.Is<MaterialDefinitionDao>(d => d.Headers.Any(h => h.Key != null && h.Name != null)), It.IsAny<InsertOneOptions>(),
                                    It.IsAny<CancellationToken>()), Times.Once()));
                return this;
            }

            public Mock<IMongoCollection<MaterialDefinitionDao>> MongoCollection()
            {
                return _mockCollection;
            }

            public MaterialDefinitionRepository SystemUnderTest()
            {
                return new MaterialDefinitionRepository(_mockCollection.Object, _factoryMock.Object,
                    new Mock<IDependencyDefinitionRepository>().Object);
            }

            public void VerifyExpectations()
            {
                _verifications.ForEach(v => v.Invoke());
            }

            public Fixture WithEmpty()
            {
                TestHelper.MockCollectionWithExisting(_mockCollection, null);
                return this;
            }

            public Fixture WithExisting(IMaterialDefinition materialDefinition)
            {
                TestHelper.MockCollectionWithExisting(_mockCollection, new MaterialDefinitionDao(materialDefinition));
                return this;
            }
        }
    }
}