using System;
using System.Collections.Generic;
using System.Web.Http.Results;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.ComponentMasters.Infrastructure.Controller
{
    public class MasterDataListControllerTests
    {
        private class Fixture
        {
            private readonly Mock<IMasterDataRepository> _mockRepository = new Mock<IMasterDataRepository>();
            private readonly List<Action> _verifications = new List<Action>();

            public Fixture Having(string listName)
            {
                _mockRepository.Setup(m => m.Find(listName)).ReturnsAsync(new MasterDataList(listName));
                return this;
            }

            public MasterDataController SystemUnderTest()
            {
                return new MasterDataController(_mockRepository.Object);
            }

            public Fixture NotHaving(string id)
            {
                _mockRepository.Setup(m => m.Find(id)).Throws(new ResourceNotFoundException("MasterDataList"));
                return this;
            }

            public Fixture Accepting(MasterDataList masterDataList)
            {
                _mockRepository.Setup(m => m.Add(masterDataList)).ReturnsAsync("001");
                _verifications.Add(() => _mockRepository.Verify(m => m.Add(It.Is<MasterDataList>(md=>md.Name == masterDataList.Name)), Times.Once));
                _mockRepository.Setup(m => m.Find("001")).ReturnsAsync(masterDataList);
                return this;
            }

            public void VerifyExpectations()
            {
                foreach (var verification in _verifications)
                    verification.Invoke();
            }

            public Fixture WithInvalidId(string id)
            {
                _mockRepository.Setup(m => m.Find(id)).Throws(new ArgumentException("id is not valid"));
                return this;
            }

            public Fixture MasterDataRepoFindThrowingResourceException()
            {
                _mockRepository.Setup(m => m.Find(It.IsAny<string>())).ThrowsAsync(new ResourceNotFoundException(""));
                return this;
            }

            public Mock<IMasterDataRepository> MasterDataRepo()
            {
                return _mockRepository;
            }

            public Fixture MAsterDataRepoFindReturning(MasterDataList input)
            {
                _mockRepository.Setup(m => m.Find(It.IsAny<string>())).ReturnsAsync(input);
                return this;
            }
        }

        [Fact]
        public async void Get_ShouldReturn200OK_WhenCalledWithExistingName()
        {
            const string id = "5822bf16d354cb293420ff63";
            var sut = new Fixture().Having(id).SystemUnderTest();
            var result = await sut.Get(id);
            result.Should().BeOfType<OkNegotiatedContentResult<MasterDataListDto>>();
            var parsedResult = (OkNegotiatedContentResult<MasterDataListDto>) result;
            parsedResult.Content.Name.Should().Be(id);
        }

        [Fact]
        public async void Get_ShouldReturn400BadRequest_WhenCalledWithInvalidId()
        {
            const string id = "001";
            var sut = new Fixture().WithInvalidId(id).SystemUnderTest();
            var result = await sut.Get(id);
            result.Should().BeOfType<BadRequestErrorMessageResult>();
            var parsedResult = (BadRequestErrorMessageResult) result;
            parsedResult.Message.Should().Be("id is not valid");
        }

        [Fact]
        public async void Get_ShouldReturn404NotFound_WhenCalledWithNonExistentName()
        {
            const string id = "5822bf16d354cb293420ff63";
            var sut = new Fixture().NotHaving(id).SystemUnderTest();
            var result = await sut.Get(id);
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async void Patch_ShouldQueryFindForMasterDataRepo_WhenMethodIsCalled()
        {
            const string masterDataName = "name";
            var input = new MasterDataList(masterDataName) {Id = "Code"};
            var fixture = new Fixture().MAsterDataRepoFindReturning(input);

            await fixture.SystemUnderTest().Patch(input);

            fixture.MasterDataRepo().Verify(m => m.Find("Code"), Times.Once);
        }

        [Fact]
        public async void Patch_ShouldReturnBadRequest_WhenMasterDataValueAlreadyExist()
        {
            var input = new MasterDataList(
                "SomeName",
                new List<MasterDataValue>
                {
                    new MasterDataValue("Value1")
                });
            var fixture = new Fixture().MAsterDataRepoFindReturning(input);

            var patch = await fixture.SystemUnderTest().Patch(input);

            patch.Should().BeAssignableTo<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async void Patch_ShouldReturnNotFound_WhenFindOfMasterDataRepositoryThrowsResourceNoyFound()
        {
            var input = new MasterDataList("name");
            var fixture = new Fixture().MasterDataRepoFindThrowingResourceException();

            var result = await fixture.SystemUnderTest().Patch(input);

            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async void Patch_ShouldQueryMasterData_WhenInputIsPassed()
        {
            var existingMasterData = new MasterDataList("name") {Id = "code"};
            var fixture = new Fixture().MAsterDataRepoFindReturning(existingMasterData);
            var input = new MasterDataList(
                "name",
                new List<MasterDataValue>
                {
                    new MasterDataValue("Value1")
                });
            var repo = fixture.MasterDataRepo();

            await fixture.SystemUnderTest().Patch(input);

            repo.Verify(r=>r.Patch(It.Is<MasterDataList>(m=>m.Name=="name")));
        }

        [Fact]
        public async void Post_ShouldReturn201Created_WhenCalledWithMasterDataList()
        {
            var masterDataListDto = MasterDataListDtoStub("Sattar");
            var masterDataList = new MasterDataList("Sattar");
            var fixture = new Fixture().Accepting(masterDataList);

            var result = await fixture.SystemUnderTest().Post(masterDataListDto);

            result.Should().BeOfType<CreatedNegotiatedContentResult<MasterDataListDto>>();
            var parsedResult = (CreatedNegotiatedContentResult<MasterDataListDto>) result;
            parsedResult.Content.Name.Should().Be("Sattar");
            fixture.VerifyExpectations();
        }

        private static MasterDataListDto MasterDataListDtoStub(string name)
        {
            return new MasterDataListDto { Name = name, Values = new List<string> { "value"} };
        }
    }
}