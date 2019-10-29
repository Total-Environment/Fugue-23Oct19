using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Driver;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.DataAdaptors;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.RepositoryPortTests
{
    public class AssetDefinitionRepositoryTests
    {
        private class Fixture
        {
            private readonly List<Action> _expectations = new List<Action>();

            private readonly Mock<IMongoCollection<AssetDefinitionDao>> _mockCollection =
                new Mock<IMongoCollection<AssetDefinitionDao>>();

            public AssetDefinitionRepository SystemUnderTest()
            {
                return new AssetDefinitionRepository(_mockCollection.Object,
                    new Mock<IDependencyDefinitionRepository>().Object,
                    new Mock<IDataTypeFactory>().Object);
            }

            public void VerifyExpectations()
            {
                _expectations.ForEach(e => e.Invoke());
            }

            public Fixture WithExistingAssetDefinition(AssetDefinition assetDefinition)
            {
                TestHelper.MockCollectionWithExisting(_mockCollection, new AssetDefinitionDao(assetDefinition));
                return this;
            }

            public Fixture Accepting()
            {
                _expectations.Add(
                    () =>
                        _mockCollection.Verify(
                            m =>
                                m.InsertOneAsync(It.IsAny<AssetDefinitionDao>(), It.IsAny<InsertOneOptions>(),
                                    It.IsAny<CancellationToken>()), Times.Once()));
                return this;
            }

            public Fixture FindAsyncReturnsNull()
            {
                TestHelper.MockCollectionWithExistingAndFindReturnEmptyList(_mockCollection, null);
                return this;
            }

            public Fixture Updating(AssetDefinition assetDefinition)
            {
                _expectations.Add(
                    () =>
                        _mockCollection.Verify(
                            m =>
                                m.ReplaceOneAsync(It.IsAny<FilterDefinition<AssetDefinitionDao>>(),
                                    It.IsAny<AssetDefinitionDao>(),
                                    It.IsAny<UpdateOptions>(),
                                    It.IsAny<CancellationToken>()),
                            Times.Once()));
                return this;
            }
        }

        [Fact]
        public async void Add_ShouldSave_WhenAssetDefinitionIsPassed()
        {
            const string code = "code";
            const string name = "name";
            var fixture = new Fixture()
                .FindAsyncReturnsNull()
                .Accepting();

            await fixture.SystemUnderTest().Add(new AssetDefinition(name) { Code = code });

            fixture.VerifyExpectations();
        }

        [Fact]
        public void Add_ShouldThrowDuplicationException_WhenAssetDefinitionWithSameNameExist()
        {
            var groupName = "someName";
            var fixture = new Fixture().WithExistingAssetDefinition(new AssetDefinition(groupName));

            Func<Task> action = async () => await fixture.SystemUnderTest().Add(new AssetDefinition(groupName));

            action.ShouldThrow<DuplicateResourceException>();
        }

        [Fact]
        public void Add_ShouldThrowResourceNotFound_WhenAssetDefinitionWithNameIsNotFound()
        {
            var fixture = new Fixture().FindAsyncReturnsNull();

            Func<Task> action = () => fixture.SystemUnderTest().Update(new AssetDefinition("name") { Code = "code" });

            action.ShouldThrow<ResourceNotFoundException>();
        }

        [Fact]
        public void AssertDefinitionRepository_ShouldImplementComponentRepository()
        {
            var assetDefinitionRepository = new Fixture().SystemUnderTest();

            assetDefinitionRepository.Should().BeAssignableTo<IComponentDefinitionRepository<AssetDefinition>>();
        }

        [Fact]
        public async void Find_ShouldReturnAsset_WhenPassedValidGroupName()
        {
            const string groupName = "name";
            const string code = "code";
            var fixture = new Fixture()
                .WithExistingAssetDefinition(new AssetDefinition(groupName) { Code = code });

            var assetDefinition = await fixture.SystemUnderTest().Find(groupName);

            assetDefinition.Name.Should().Be(groupName);
        }

        [Fact]
        public async void Update_ShouldSave_WhenAssetDefinitionIsPassed()
        {
            const string code = "code";
            const string name = "name";
            var fixture = new Fixture()
                .WithExistingAssetDefinition(new AssetDefinition(name) { Code = code })
                .Updating(new AssetDefinition(name) { Code = code });

            await fixture.SystemUnderTest().Update(new AssetDefinition(name) { Code = code });

            fixture.VerifyExpectations();
        }
    }
}