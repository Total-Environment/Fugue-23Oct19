using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.ControllerPort;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.ControllerPortTests
{
    public class BrandDefinitionControllerTests
    {
        [Fact]
        public async void Post_ShouldCreateABrandDefinition_GiveResponse201Created()
        {
            var masterDataList = new MasterDataList("Status");
            var brandDefinition = new BrandDefinition("Generic Brands", new List<ISimpleColumnDefinition>
            {
                new SimpleColumnDefinition("Brand/Series","Brand/Series", new StringDataType()),
                new SimpleColumnDefinition("3D Model","3D Model", new StaticFileDataType()),
                new SimpleColumnDefinition("Status", "Status", new MasterDataDataType(masterDataList)),
                new SimpleColumnDefinition("Warranty Period in Years", "Warranty Period in Years", new IntDataType())
            });
            var brandDefinitionRepository = new Mock<IBrandDefinitionRepository>();
            brandDefinitionRepository.Setup(b => b.FindBy("Generic Brand")).ReturnsAsync(brandDefinition);
            var dataTypeFactory = new Mock<ISimpleDataTypeFactory>();
            dataTypeFactory.Setup(d => d.Construct("String", It.IsAny<object>())).ReturnsAsync(new StringDataType());
            dataTypeFactory.Setup(d => d.Construct("StaticFile", It.IsAny<object>()))
                .ReturnsAsync(new StaticFileDataType());
            dataTypeFactory.Setup(d => d.Construct("MasterData", It.IsAny<object>()))
                .ReturnsAsync(new MasterDataDataType(masterDataList));
            dataTypeFactory.Setup(d => d.Construct("Int", It.IsAny<object>())).ReturnsAsync(new IntDataType());
            var brandDefinitionController = new BrandDefinitionController(brandDefinitionRepository.Object,
                dataTypeFactory.Object);
            var brandDefinitionDto = new BrandDefinitionDto("Generic Brand", new List<SimpleColumnDefinitionDto>
            {
                new SimpleColumnDefinitionDto
                {
                    Name = "Brand/Series",
                    DataType = new SimpleDataTypeDto
                    {
                        Name = "String",
                        SubType = ""
                    }
                },
                new SimpleColumnDefinitionDto
                {
                    Name = "3D Model",
                    DataType = new SimpleDataTypeDto
                    {
                        Name = "StaticFile",
                        SubType = ""
                    }
                },
                new SimpleColumnDefinitionDto
                {
                    Name = "Status",
                    DataType = new SimpleDataTypeDto
                    {
                        Name = "MasterData",
                        SubType = "status"
                    }
                },
                new SimpleColumnDefinitionDto
                {
                    Name = "Warranty Period in Years",
                    DataType = new SimpleDataTypeDto
                    {
                        Name = "Int",
                        SubType = ""
                    }
                }
            });

            var response = await brandDefinitionController.Add(brandDefinitionDto);
            response.Should().BeOfType<CreatedNegotiatedContentResult<BrandDefinitionDto>>();
            var castResponse = response as CreatedNegotiatedContentResult<BrandDefinitionDto>;
            castResponse.Content.Columns.Count.Should().Be(4);
        }

        [Fact]
        public async Task Post_shouldThrowDuplicateResouceException_WhenBrandDefinitionWithSameNameExists()
        {
            var brandDefinitionRepository = new Mock<IBrandDefinitionRepository>();
            brandDefinitionRepository.Setup(b => b.Add(It.IsAny<BrandDefinition>()))
                .Throws(new DuplicateResourceException("Already Exists"));

            var dataTypeFactory = new Mock<ISimpleDataTypeFactory>();

            var brandDefinitionController = new BrandDefinitionController(brandDefinitionRepository.Object,
                dataTypeFactory.Object);
            var brandDefinitionDto = new BrandDefinitionDto("Generic Brand", new List<SimpleColumnDefinitionDto>());

            var response = await brandDefinitionController.Add(brandDefinitionDto);
            response.Should().BeOfType<ConflictResult>();
        }

        [Fact]
        public async Task Post_ShouldthrowResourceNotFound_WhenBrandDefinitionIsNotPresent()
        {
            var brandDefinitionRepository = new Mock<IBrandDefinitionRepository>();
            brandDefinitionRepository.Setup(b => b.FindBy("Generic Brand"))
                .Throws(new ResourceNotFoundException("Not Found"));

            var dataTypeFactory = new Mock<ISimpleDataTypeFactory>();

            var brandDefinitionController = new BrandDefinitionController(brandDefinitionRepository.Object,
                dataTypeFactory.Object);
            var brandDefinitionDto = new BrandDefinitionDto("Generic Brand", new List<SimpleColumnDefinitionDto>());

            var response = await brandDefinitionController.Add(brandDefinitionDto);
            response.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Get_ShouldGetABrandDefinition_ReturnBrandDefinition()
        {
            var masterDataList = new MasterDataList("Status");
            var brandDefinition = new BrandDefinition("Generic Brands", new List<ISimpleColumnDefinition>
            {
                new SimpleColumnDefinition("Brand/Series","Brand/Series", new StringDataType()),
                new SimpleColumnDefinition("3D Model","3D Model", new StaticFileDataType()),
                new SimpleColumnDefinition("Status", "Status", new MasterDataDataType(masterDataList)),
                new SimpleColumnDefinition("Warranty Period in Years", "Warranty Period in Years", new IntDataType())
            });
            var brandDefinitionRepository = new Mock<IBrandDefinitionRepository>();
            brandDefinitionRepository.Setup(b => b.FindBy("Generic Brand")).ReturnsAsync(brandDefinition);
            var dataTypeFactory = new Mock<ISimpleDataTypeFactory>();
            dataTypeFactory.Setup(d => d.Construct("String", It.IsAny<object>())).ReturnsAsync(new StringDataType());
            dataTypeFactory.Setup(d => d.Construct("StaticFile", It.IsAny<object>()))
                .ReturnsAsync(new StaticFileDataType());
            dataTypeFactory.Setup(d => d.Construct("MasterData", It.IsAny<object>()))
                .ReturnsAsync(new MasterDataDataType(masterDataList));
            dataTypeFactory.Setup(d => d.Construct("Int", It.IsAny<object>())).ReturnsAsync(new IntDataType());
            var brandDefinitionController = new BrandDefinitionController(brandDefinitionRepository.Object,
                dataTypeFactory.Object);

            var response = await brandDefinitionController.Get();
            response.Should().BeOfType<OkNegotiatedContentResult<BrandDefinitionDto>>();
            var castResponse = response as OkNegotiatedContentResult<BrandDefinitionDto>;
            castResponse.Content.Columns.Count.Should().Be(4);
        }

        [Fact]
        public async Task Get_IfBrandDefinitionDoesNotExist_ReturnNotFoundResult()
        {
            var brandDefinitionRepository = new Mock<IBrandDefinitionRepository>();
            brandDefinitionRepository.Setup(b => b.FindBy("Generic Brand")).Throws(new ResourceNotFoundException(""));
            var dataTypeFactory = new Mock<ISimpleDataTypeFactory>();
            var brandDefinitionController = new BrandDefinitionController(brandDefinitionRepository.Object,
                dataTypeFactory.Object);

            var response = await brandDefinitionController.Get();
            response.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Get_IfAnyErrorIsThrown_ReturnBadRequestResult()
        {
            var brandDefinitionRepository = new Mock<IBrandDefinitionRepository>();
            brandDefinitionRepository.Setup(b => b.FindBy("Generic Brand")).Throws(new DuplicateResourceException(""));
            var dataTypeFactory = new Mock<ISimpleDataTypeFactory>();
            var brandDefinitionController = new BrandDefinitionController(brandDefinitionRepository.Object,
                dataTypeFactory.Object);

            var response = await brandDefinitionController.Get();
            response.Should().BeOfType<BadRequestResult>();
        }
    }
}