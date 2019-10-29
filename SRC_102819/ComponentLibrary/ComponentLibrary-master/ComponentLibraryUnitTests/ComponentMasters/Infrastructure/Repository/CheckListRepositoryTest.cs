using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.ComponentMasters.Infrastructure.Repository
{
    public class MaterialCheckListRepositoryTest
    {
        private static CheckList CheckListWithId(string checkListId)
        {
            var checkList = new CheckList();
            checkList.CheckListId = checkListId;
            return checkList;
        }

        private class TestFixture
        {
            private Mock<IMongoRepository<CheckListDao>> _mongoMock;

            public TestFixture WithMongoMock()
            {
                _mongoMock = new Mock<IMongoRepository<CheckListDao>>();
                return this;
            }

            public TestFixture HavingCheckListWithId(string checkListId)
            {
                var checkListDao = new Mock<CheckListDao>();
                checkListDao.Setup(m => m.CheckList).Returns(CheckListWithId(checkListId));
                _mongoMock.Setup(m => m.FindBy("CheckListId", checkListId))
                    .Returns(Task.FromResult(checkListDao.Object));
                return this;
            }

            public CheckListRepository ToSut()
            {
                return new CheckListRepository(_mongoMock.Object);
            }

            public TestFixture WhenAddedReturns(CheckList checkList)
            {
                var dao = new Mock<CheckListDao>();
                dao.Setup(d => d.CheckList).Returns(checkList);
                _mongoMock.Setup(m => m.Add(It.IsAny<CheckListDao>())).Returns(Task.FromResult(dao.Object));
                return this;
            }
        }

        [Fact]
        public void Add_PassedDuplicateEntry_ThrowArgumentException()
        {
            const string checkListId = "MaterialInspection";
            var checkListRepository = new TestFixture().WithMongoMock().HavingCheckListWithId(checkListId).ToSut();
            Func<Task> act = async () => await checkListRepository.Add(CheckListWithId(checkListId));
            act.ShouldThrow<ArgumentException>().WithMessage("Checklist already exists.");
        }

        [Fact]
        public async void Add_Should_AddCheckListToDatabase()
        {
            var checkList = CheckListWithId("MaterailCheckList");
            var actualCheckList = CheckListWithId("ActualCheckList");
            var checkListRepository = new TestFixture().WithMongoMock().WhenAddedReturns(actualCheckList).ToSut();

            var savedCheckList = await checkListRepository.Add(checkList);

            savedCheckList.Should().Be(actualCheckList);
        }

        [Fact]
        public async void GetById_Should_GetCheckList()
        {
            var checkListId = "existing";
            var checkList = CheckListWithId(checkListId);
            var checkListRepository = new TestFixture().WithMongoMock().HavingCheckListWithId(checkListId).ToSut();

            var actualCheckList = await checkListRepository.GetById(checkListId);

            actualCheckList.Should().Be(checkList);
        }

        [Fact]
        public void GetById_Should_ThrowNotFoundException_WhenCheckListIdDoesNotExist()
        {
            var checkListRepository = new TestFixture().WithMongoMock().ToSut();
            Func<Task> act = async () => await checkListRepository.GetById("NonExistent");
            act.ShouldThrow<ResourceNotFoundException>().WithMessage("Checklist not found.");
        }
    }
}