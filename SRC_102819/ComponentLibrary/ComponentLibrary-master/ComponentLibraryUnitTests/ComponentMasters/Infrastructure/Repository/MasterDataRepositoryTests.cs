using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.ComponentMasters.Infrastructure.Repository
{
    public class MasterDataRepositoryTests
    {
        private class Fixture
        {
            private readonly Mock<IMongoCollection<MasterDataListDao>> _mockCollection =
                new Mock<IMongoCollection<MasterDataListDao>>();

            private readonly List<Action> _verifications = new List<Action>();

            public MasterDataRepository SystemUnderTest()
            {
                return new MasterDataRepository(_mockCollection.Object);
            }

            public Fixture Accepting(MasterDataList masterList)
            {
                TestHelper.MockCollectionWithExisting(_mockCollection, null);
                _verifications.Add(
                    () =>
                        _mockCollection.Verify(
                            m =>
                                m.InsertOneAsync(It.IsAny<MasterDataListDao>(), It.IsAny<InsertOneOptions>(),
                                    It.IsAny<CancellationToken>()), Times.Once()));
                return this;
            }

            public void VerifyExpectations()
            {
                _verifications.ForEach(v => v.Invoke());
            }

            public Fixture WithExisting(string listName)
            {
                TestHelper.MockCollectionWithExisting(_mockCollection, new MasterDataListDao { Name = listName });
                return this;
            }

            public Fixture WithExisting(string listName, MasterDataList dataList)
            {
                TestHelper.MockCollectionWithExisting(_mockCollection, new MasterDataListDao { Name = listName, MasterDataList = dataList });
                return this;
            }

            public Fixture NotAdding(string listName)
            {
                _verifications.Add(
                    () =>
                        _mockCollection.Verify(
                            m => m.InsertOneAsync(It.IsAny<MasterDataListDao>(), null, default(CancellationToken)),
                            Times.Never));
                return this;
            }

            public Fixture Without(string name)
            {
                TestHelper.MockCollectionWithExisting(_mockCollection, null);
                return this;
            }

            public Fixture WithoutId(string id)
            {
                TestHelper.MockCollectionWithExisting(_mockCollection, null);
                return this;
            }

            public Fixture WithExistingId(string id)
            {
                TestHelper.MockCollectionWithExisting(_mockCollection,
                    new MasterDataListDao(new MasterDataList("sattar") { Id = id }));
                return this;
            }

            public Mock<IMongoCollection<MasterDataListDao>>  MongoCollection()
            {
                return _mockCollection;
            }
        }

        [Fact]
        public async void Add_ShouldSaveMasterListAndReturnId_WhenPassed()
        {
            var masterList = new MasterDataList("sattar");
            var fixture = new Fixture().Accepting(masterList);

            var result = await fixture.SystemUnderTest().Add(masterList);
            result.Should().NotBeNullOrEmpty();
            fixture.VerifyExpectations();
        }

        [Fact]
        public void Add_ShouldThrowArgumentException_WhenPassedExistingList()
        {
            var fixture = new Fixture().WithExisting("sattar").NotAdding("sattar");
            Func<Task> act = async () => await fixture.SystemUnderTest().Add(new MasterDataList("sattar"));
            act.ShouldThrow<DuplicateResourceException>().WithMessage("sattar already exists.");
            fixture.VerifyExpectations();
        }

        [Fact]
        public async void Find_ShouldReturnMasterDataList_WhenPassedExistingId()
        {
            var id = ObjectId.GenerateNewId().ToString();
            var sut = new Fixture().WithExistingId(id).SystemUnderTest();
            (await sut.Find(id)).Should().NotBeNull();
        }

        [Fact]
        public void Find_ShouldThrowArgumentException_WhenPassedInvalidId()
        {
            const string id = "sattar";
            var sut = new Fixture().SystemUnderTest();
            Func<Task> act = async () => await sut.Find(id);
            act.ShouldThrow<ArgumentException>().WithMessage("Master Data id is not valid for sattar.");
        }

        [Fact]
        public async void Exists_Should_CheckForValueIndependentOfCase()
        {
            const string masterDataValue = "safety";
            const string masterDataListName = "material_level_2";
            var masterDataList = new MasterDataList("material_level_2",
                new List<MasterDataValue>() {new MasterDataValue("Safety")});
            var sut = new Fixture().WithExisting("material_level_2",masterDataList).SystemUnderTest();
            (await sut.Exists(masterDataListName, masterDataValue)).Should().BeTrue();
        }

        [Fact]
        public void Find_ShouldThrowResourceNotFoundException_WhenPassedNonExistingId()
        {
            const string id = "5822bf16d354cb293420ff63";
            var sut = new Fixture().WithoutId(id).SystemUnderTest();
            Func<Task> act = async () => await sut.Find(id);
            act.ShouldThrow<ResourceNotFoundException>();
        }

        [Fact]
        public void MasterDataListRepository_ShouldBeAssignableToIMasterDataListRepository()
        {
            var sut = new Fixture().SystemUnderTest();
            sut.Should().BeAssignableTo<IMasterDataRepository>();
        }

        [Fact]
        public async void Patch_ShouldQueryMongoCollectionReplace_WhenRecievesInput()
        {
            var masterData = new MasterDataList("masterData");
            var fixture =new Fixture();
            var mongoCollection = fixture.MongoCollection();

            await fixture.SystemUnderTest().Patch(masterData);

            mongoCollection.Verify(m=>m.ReplaceOneAsync(
                It.IsAny<FilterDefinition<MasterDataListDao>>(), 
                It.Is<MasterDataListDao>(md=>md.Name == "masterData"),
                null,
                default(CancellationToken)),Times.Once);
        }
    }
}