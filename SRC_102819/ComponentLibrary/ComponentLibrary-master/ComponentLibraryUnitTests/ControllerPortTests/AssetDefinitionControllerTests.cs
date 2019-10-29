using System.Web.Http.Results;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.ControllerPort;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.ControllerPortTests
{
    public class AssetDefinitionControllerTests
    {
        private class Fixture
        {
            private readonly Mock<IComponentDefinitionRepository<AssetDefinition>> _mockedAssetDefinitionRepo
                = new Mock<IComponentDefinitionRepository<AssetDefinition>>();

            private readonly Mock<IComponentDefinitionRepository<IMaterialDefinition>> _mockedMaterialDefinitionRepo
                = new Mock<IComponentDefinitionRepository<IMaterialDefinition>>();

            public AssetDefinitionController SystemUnderTest()
            {
                return new AssetDefinitionController(_mockedAssetDefinitionRepo.Object,
                    _mockedMaterialDefinitionRepo.Object,
                    new Mock<IDataTypeFactory>().Object,
                    new Mock<IDependencyDefinitionRepository>().Object);
            }

            public Fixture HavingAssetDefinition(AssetDefinition assetDefinition)
            {
                _mockedAssetDefinitionRepo.Setup(m => m.Find(assetDefinition.Name))
                    .ReturnsAsync(assetDefinition);
                return this;
            }

            public Fixture NoAssetDefinition()
            {
                _mockedAssetDefinitionRepo.Setup(m => m.Find(It.IsAny<string>()))
                    .ThrowsAsync(new ResourceNotFoundException(""));
                return this;
            }

            public Fixture NoMaterialDefinitionPresent()
            {
                _mockedMaterialDefinitionRepo.Setup(m => m.Find(It.IsAny<string>()))
                    .ThrowsAsync(new ResourceNotFoundException(""));
                return this;
            }

            public Fixture HavingMaterialDefinition(MaterialDefinition materialDefinition)
            {
                _mockedMaterialDefinitionRepo.Setup(m => m.Find(materialDefinition.Name))
                    .ReturnsAsync(materialDefinition);
                return this;
            }
        }

        [Fact]
        public async void Get_ShouldReturnBadRequest_WhenNamePassedIsNullOrWhitespace()
        {
            var fixture = new Fixture();

            var result = await fixture.SystemUnderTest().Get("");

            result.Should().BeOfType<BadRequestErrorMessageResult>("Asset definition name cannot be null or empty.");
        }

        [Fact]
        public async void Get_ShouldReturnNotFound_WhenAssetDefinitionWithNameISnotPresent()
        {
            var fixture = new Fixture().NoAssetDefinition();

            var result = await fixture.SystemUnderTest().Get("name");

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async void Get_ShouldReturnOkNegotiatedContentResult_WhenValidAssetGroupNameIsPassed()
        {
            var fixture = new Fixture()
                .HavingAssetDefinition(new AssetDefinition("assetGroupName"));

            var result = await fixture.SystemUnderTest().Get("assetGroupName");

            result.Should().BeOfType<OkNegotiatedContentResult<ComponentDefinitionDto>>();
        }

        [Fact]
        public async void Post_ShouldReturnBadRequest_WhenNoMaterialDefinitionOfSameNameExist()
        {
            var fixture = new Fixture().NoMaterialDefinitionPresent();

            var result = await fixture.SystemUnderTest().Post(new AssetDefinitionDto { Name = "someName" });

            result.Should().BeOfType<BadRequestErrorMessageResult>();
        }
    }
}