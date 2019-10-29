using System;
using System.Threading.Tasks;
using System.Web.Http.Results;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.ComponentMasters.Infrastructure.Controller
{
    public class CheckListControllerTests
    {
        public class TestFixture
        {
            private Mock<ICheckListRepository> _mockRepository;

            public TestFixture WithMockRepository()
            {
                _mockRepository = new Mock<ICheckListRepository>();
                return this;
            }

            public TestFixture WithCheckList(CheckList checkList)
            {
                _mockRepository.Setup(r => r.GetById(checkList.Id)).Returns(Task.FromResult(checkList));
                return this;
            }

            public CheckListController ToSut()
            {
                return new CheckListController(_mockRepository.Object);
            }

            public TestFixture WithoutCheckListId(string checkListIdThatDoesNotExist)
            {
                _mockRepository.Setup(r => r.GetById(checkListIdThatDoesNotExist))
                    .Throws(new ResourceNotFoundException("checkList"));
                return this;
            }
        }

        [Fact]
        public async void Get_WhenCalledWithExistingCheckListId_Returns200CheckList()
        {
            const string checkListId = "MNS001";
            var checkList = new CheckList {Id = checkListId};
            var controller = new TestFixture().WithMockRepository().WithCheckList(checkList).ToSut();

            var result = await controller.Get(checkListId) as OkNegotiatedContentResult<CheckList>;

            result.Should().NotBeNull();
            result.Content.Should().Be(checkList);
        }

        [Fact]
        public async void Get_WhenCalledWithNonExistingCheckListId_Returns404()
        {
            const string checkListId = "MNS001";
            var checkList = new CheckList {Id = checkListId};
            var controller = new TestFixture().WithMockRepository().WithoutCheckListId(checkListId).ToSut();

            (await controller.Get(checkListId)).Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async void Post_ExistingCheckListData_ReturnsConflictResponse()
        {
            var mockRepository = new Mock<ICheckListRepository>();
            var postedCheckList = new Mock<CheckList>().Object;
            postedCheckList.Id = "Existing";

            mockRepository.Setup(r => r.Add(postedCheckList))
                .Throws(new ArgumentException("CheckList already exists."));
            var controller = new CheckListController(mockRepository.Object);

            var result = await controller.Post(postedCheckList) as ConflictResult;
            result.Should().NotBeNull();
        }

        [Fact]
        public async void Post_InvalidCheckListData_ReturnsBadRequest()
        {
            var mockRepository = new Mock<ICheckListRepository>();
            var postedCheckList = new Mock<CheckList>().Object;

            mockRepository.Verify(r => r.Add(It.IsAny<CheckList>()), Times.Never);
            var controller = new CheckListController(mockRepository.Object);

            controller.ModelState.AddModelError("Name", "Invalid Name");

            var result = await controller.Post(postedCheckList) as InvalidModelStateResult;
            result.Should().NotBeNull();
            result.ModelState.Should().Equal(controller.ModelState);
        }

        [Fact]
        public async void Post_ValidChecklistData_ReturnsCheckList()
        {
            var mockRepository = new Mock<ICheckListRepository>();
            var postedCheckList = new CheckList {Id = "same"};
            var createdCheckList = new CheckList {Id = "same"};

            mockRepository.Setup(r => r.Add(postedCheckList)).Returns(Task.FromResult(createdCheckList));
            var controller = new CheckListController(mockRepository.Object);

            var result = await controller.Post(postedCheckList) as CreatedNegotiatedContentResult<CheckList>;
            result.Should().NotBeNull();
            result.Content.Should().Be(createdCheckList);
        }
    }
}