using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Extensions;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using Xunit;
using MoneyDto = TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller.Dto.MoneyDto;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.RateMasters.Infrastructure.Controller
{
    public class RentalRateControllerTests
    {
        #region GET methods

        [Fact]
        public async void GetRentalRate_When_AppliedFromIsNotInISOFormat_ShouldReturnBadRequest()
        {
            var controller = CreateFixture();
            var appliedFrom = DateTime.UtcNow.InIst();

            var result = await controller.Get("1", appliedFrom, "Daily");

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
            var response = result as BadRequestErrorMessageResult;
            response.Message.Should().Be("Date should be in ISO format.");
        }

        [Fact]
        public async Task GetRentalRate_When_AppliedFromIsNull_ShouldReturnBadRequest()
        {
            var controller = CreateFixture();

            var result = await controller.Get("1", default(DateTime), "Daily");

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
            var response = result as BadRequestErrorMessageResult;
            response.Message.Should().Be("Applied From is null.");
        }

        [Fact]
        public async void GetRentalRate_When_MaterialIdIsNull_ShouldReturnBadRequest()
        {
            var controller = CreateFixture();
            var appliedFrom = DateTime.UtcNow;

            var result = await controller.Get(null, appliedFrom, "Daily");

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
            var response = result as BadRequestErrorMessageResult;
            response.Message.Should().Be("Material Id is null.");
        }

        [Fact]
        public async Task GetRentalRate_When_MaterialIsNotAsset_ShouldReturnBadRequest()
        {
            var materialRepository = MaterialRepositoryWithoutAsset();

            var controller = CreateFixture(materialRepository);

            var result = await controller.Get("1", DateTime.Now, "Daily");

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
            var response = result as BadRequestErrorMessageResult;
            response.Message.Should()
                .Be("This material is not defined as an Asset and hence can not have Rental rates.");
        }

        [Fact]
        public async Task GetRentalRate_When_NoMatachingElementIsFoundAndMaterialIsAsset_ShouldReturnNotFoundResult()
        {
            var materialRepository = MaterialRepositoryWithAsset();

            var rentalRateRepository = new Mock<IRentalRateRepository>();
            rentalRateRepository.Setup(r => r.Get(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Throws(new ResourceNotFoundException("Rental rate not found."));
            var controller = CreateFixture(rentalRateRepository, materialRepository);

            var result = await controller.Get("1", DateTime.Now, "Daily");

            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async Task GetRentalRate_When_UnitOfMeasureIsNull_ShouldReturnBadRequest()
        {
            var controller = CreateFixture();

            var result = await controller.Get("1", DateTime.UtcNow, null);

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
            var response = result as BadRequestErrorMessageResult;
            response.Message.Should().Be("Unit of Measure is null.");
        }

        [Fact]
        public async Task GetRentalRate_WithExistingMaterialData_WhichIsAsAsset_ShouldReturnRentalValue()
        {
            var materialRepository = MaterialRepositoryWithAsset();

            var rentalRateRepository = new Mock<IRentalRateRepository>();
            rentalRateRepository.Setup(r => r.Get(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new RentalRate("1", "Daily", new Money(20, "INR"), DateTime.Now));
            var controller = CreateFixture(rentalRateRepository, materialRepository);

            var result = await controller.Get("1", DateTime.Now, "Daily");

            result.Should().BeAssignableTo<OkNegotiatedContentResult<RentalRateDto>>();
            var response = result as OkNegotiatedContentResult<RentalRateDto>;
            response.Content.RentalRateValue.ToString().Should().Be("Value: 20, Currency: INR");
        }

        #endregion GET methods

        #region GET Latest methods

        [Fact]
        public async Task GetLatesRentalRate_WithExistingMaterialData_WhichIsAsAsset_ShouldReturnLatestRentalValue_WhenUnitOfMeasureIsNotSpecified()
        {
            var materialRepository = MaterialRepositoryWithAsset();

            var rentalRateRepository = new Mock<IRentalRateRepository>();
            rentalRateRepository.Setup(r => r.GetLatest(It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<IRentalRate>() { new RentalRate("1", "Daily", new Money(20, "INR"), DateTime.Now) });
            var controller = CreateFixture(rentalRateRepository, materialRepository);

            var result = await controller.GetActive("1", DateTime.Now);

            result.Should().BeAssignableTo<OkNegotiatedContentResult<IEnumerable<RentalRateDto>>>();
            var response = result as OkNegotiatedContentResult<IEnumerable<RentalRateDto>>;
            response.Content.FirstOrDefault().RentalRateValue.ToString().Should().Be("Value: 20, Currency: INR");
        }

        [Fact]
        public async Task GetLatestRate_When_AppliedFromIsNull_ShouldReturnBadRequest()
        {
            var controller = CreateFixture();

            var result = await controller.GetActive("1", default(DateTime));

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
            var response = result as BadRequestErrorMessageResult;
            response.Message.Should().Be("Applied From is null.");
        }

        [Fact]
        public async void GetLatestRate_When_MaterialIdIsNull_ShouldReturnBadRequest()
        {
            var controller = CreateFixture();
            var appliedFrom = DateTime.UtcNow;

            var result = await controller.GetActive(null, appliedFrom);

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
            var response = result as BadRequestErrorMessageResult;
            response.Message.Should().Be("Material Id is null.");
        }

        public async void GetLatestRentalRate_When_AppliedFromIsNotInISOFormat_ShouldReturnBadRequest()
        {
            var controller = CreateFixture();
            var appliedFrom = DateTime.UtcNow.InIst();

            var result = await controller.GetActive("1", appliedFrom);

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
            var response = result as BadRequestErrorMessageResult;
            response.Message.Should().Be("Date should be in ISO format.");
        }

        [Fact]
        public async Task GetLatestRentalRate_When_MaterialIsNotAsset_ShouldReturnBadRequest()
        {
            var materialRepository = MaterialRepositoryWithoutAsset();

            var controller = CreateFixture(materialRepository);

            var result = await controller.GetActive("1", DateTime.Now);

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
            var response = result as BadRequestErrorMessageResult;
            response.Message.Should()
                .Be("This material is not defined as an Asset and hence can not have Rental rates.");
        }

        [Fact]
        public async Task GetLatestRentalRate_When_NoMatachingElementIsFoundAndMaterialIsAsset_ShouldReturnNotFoundResult()
        {
            var materialRepository = MaterialRepositoryWithAsset();

            var rentalRateRepository = new Mock<IRentalRateRepository>();
            rentalRateRepository.Setup(r => r.GetLatest(It.IsAny<string>(), It.IsAny<DateTime>()))
                .Throws(new ResourceNotFoundException("Rental rate not found."));
            var controller = CreateFixture(rentalRateRepository, materialRepository);

            var result = await controller.GetActive("1", DateTime.Now);

            result.Should().BeAssignableTo<NotFoundResult>();
        }

        #endregion GET Latest methods

        #region GetAll Methods

        [Fact]
        public async void GetAllRates_When_MaterialIdIsNull_ShouldReturnBadRequest()
        {
            var controller = CreateFixture();
            var appliedFrom = DateTime.UtcNow;

            var result = await controller.GetAll(null);

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
            var response = result as BadRequestErrorMessageResult;
            response.Message.Should().Be("Material Id is null.");
        }

        [Fact]
        public async Task GetAllRates_When_MaterialIsNotAsset_ShouldReturnBadRequest()
        {
            var materialRepository = MaterialRepositoryWithoutAsset();

            var controller = CreateFixture(materialRepository);

            var result = await controller.GetAll("1");

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
            var response = result as BadRequestErrorMessageResult;
            response.Message.Should()
                .Be("This material is not defined as an Asset and hence can not have Rental rates.");
        }

        [Fact]
        public async Task GetAllRates_When_NoMatachingElementIsFoundAndMaterialIsAsset_ShouldReturnNotFoundResult()
        {
            var materialRepository = MaterialRepositoryWithAsset();

            var rentalRateRepository = new Mock<IRentalRateRepository>();
            rentalRateRepository.Setup(r => r.GetAll(It.IsAny<string>(), "Daily", null, 1, "AppliedFrom", SortOrder.Descending))
                .Throws(new ResourceNotFoundException("Rental rate not found."));
            var controller = CreateFixture(rentalRateRepository, materialRepository);

            var result = await controller.GetAll("1", "Daily");

            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async Task GetAllRates_WithExistingMaterialData_WhichIsAsAsset_ShouldReturnRentalValue()
        {
            var materialRepository = MaterialRepositoryWithAsset();

            var rentalRateRepository = new Mock<IRentalRateRepository>();
            rentalRateRepository.Setup(r => r.GetAll(It.IsAny<string>(), "Daily", null, 1, "AppliedFrom", SortOrder.Descending))
                .ReturnsAsync(new PaginatedAndSortedList<IRentalRate>(
                    new List<IRentalRate> { new RentalRate("1", "Daily", new Money(20, "INR"), DateTime.Now) },
                    1,
                    1,
                    20,
                    "AppliedFrom",
                    SortOrder.Descending
                ));
            var controller = CreateFixture(rentalRateRepository, materialRepository);

            var result = await controller.GetAll("1", "Daily");

            result.Should().BeAssignableTo<OkNegotiatedContentResult<PaginatedAndSortedListDto<RentalRateDto>>>();
            var response = result as OkNegotiatedContentResult<PaginatedAndSortedListDto<RentalRateDto>>;
            response.Content.Items.FirstOrDefault().RentalRateValue.ToString().Should().Be("Value: 20, Currency: INR");
        }

        #endregion GetAll Methods

        #region POST methods

        [Fact]
        public void PostRentalRate_Should_SaveRentalRateToRentalRateRepository()
        {
            //Arrange
            var rentalRateRepository = new Mock<IRentalRateRepository>();
            var controller = CreateFixture(rentalRateRepository);
            var rentalRateDto = CreateRentalRateDto("");

            //Act
            controller.CreateRentalRate("1", rentalRateDto);

            //Assert
            var rentalRate = rentalRateDto.GetDomain();
            rentalRateRepository.Verify(rr => rr.Add(rentalRate), Times.Once);
        }

        [Fact]
        public async Task PostRentalrate_Should_ThrowBadRequest_WhenAppliedFromAPastDate()
        {
            //Arrange
            var controller = CreateFixture();
            var rentalRateDto = CreateRentalRateDto("");
            rentalRateDto.AppliedFrom = DateTime.Now.AddDays(-1);

            //Act
            var result = await controller.CreateRentalRate("1", rentalRateDto);

            //Assert
            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
            var response = result as BadRequestErrorMessageResult;
            response.Message.Should().Be("New version of rental rate can only be created for future dates.");
        }

        [Fact]
        public async Task PostRentalrate_Should_ThrowBadRequest_WhenAppliedFromIsTodayInIST()
        {
            //Arrange
            var controller = CreateFixture();
            var rentalRateDto = CreateRentalRateDto("");
            rentalRateDto.AppliedFrom = DateTime.UtcNow;

            //Act
            var result = await controller.CreateRentalRate("1", rentalRateDto);

            //Assert
            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
            var response = result as BadRequestErrorMessageResult;
            response.Message.Should().Be("New version of rental rate can only be created for future dates.");
        }

        [Theory]
        [InlineData("uom")]
        [InlineData("value")]
        [InlineData("appliedFrom")]
        public async Task
            PostRentalrate_Should_ThrowBadRequest_WhenEitherOfUomOrValueWithCurrencyOrApplicableFromDateIsMissing(
                string missingElement)
        {
            //Arrange
            var controller = CreateFixture();
            var rentalRateDto = CreateRentalRateDto(missingElement);

            //Act
            var result = await controller.CreateRentalRate("1", rentalRateDto);

            //Assert
            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
            var response = result as BadRequestErrorMessageResult;
            response.Message.Should().Be("One of input values is null.");
        }

        [Fact]
        public async Task PostRentalRate_Should_ThrowBadRequest_WhenUnitOfMEasureIsNotAMasterDataValue()
        {
            var masterDataRepository = new Mock<IMasterDataRepository>();
            masterDataRepository.Setup(m => m.Exists("UnitOfMeasure", "Daily")).Returns(Task.FromResult(false));
            var controller = CreateFixture(masterDataRepository);
            var result = await controller.CreateRentalRate("1", CreateRentalRateDto(""));

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
            var response = result as BadRequestErrorMessageResult;
            response.Message.Should().Be("Please specify a valid UoM for Rental");
        }

        [Fact]
        public async Task PostRentalrate_Should_ThrowBadRequest_WhrenMaterialCannotBeUsedAsAsset()
        {
            //Arrange
            var materialRepository = MaterialRepositoryWithoutAsset();
            var controller = CreateFixture(materialRepository);
            var rentalRateDto = CreateRentalRateDto("");

            //Act
            var result = await controller.CreateRentalRate("1", rentalRateDto);

            //Assert
            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
            var response = result as BadRequestErrorMessageResult;
            response.Message.Should()
                .Be("This material is not defined as an Asset and hence can not have Rental rates.");
        }

        [Fact]
        public async Task PostRentalrate_Should_ThrowBadRequest_WhrenMaterialIdDoesNotExistInSystem()
        {
            //Arrange
            var materialRepository = new Mock<IMaterialRepository>();
            var resultTask = Task.FromResult<IMaterial>(null);
            materialRepository.Setup(m => m.Find("1")).Returns(resultTask);
            var controller = CreateFixture(materialRepository);
            var rentalRateDto = CreateRentalRateDto("");

            //Act
            var result = await controller.CreateRentalRate("1", rentalRateDto);

            //Assert
            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
            var response = result as BadRequestErrorMessageResult;
            response.Message.Should().Be("MaterialID not found.");
        }

        [Fact]
        public async Task PostRentalRate_ShouldAccept_UnitOfMeasureRentalRateAndApplicableFrom()
        {
            var controller = CreateFixture();
            var result = await controller.CreateRentalRate("1", CreateRentalRateDto(""));
            result.Should().BeAssignableTo<CreatedNegotiatedContentResult<RentalRateDto>>();
        }

        #endregion POST methods

        private static RentalRateDto CreateRentalRateDto(string missingElement)
        {
            var materialId = "1";
            var unitOfMesure = "Daily";
            var rentalRateValue = new MoneyDto(new Money(2, "INR"));
            var appliedFrom = DateTime.UtcNow.AddDays(1);
            switch (missingElement)
            {
                case "materialID":
                    materialId = null;
                    break;

                case "uom":
                    unitOfMesure = null;
                    break;

                case "value":
                    rentalRateValue = null;
                    break;

                case "appliedFrom":
                    appliedFrom = default(DateTime);
                    break;
            }

            return new RentalRateDto(materialId, unitOfMesure, rentalRateValue, appliedFrom);
        }

        private static Mock<IMasterDataRepository> MasterDataRepositoryWithAsset()
        {
            var masterDataRepository = new Mock<IMasterDataRepository>();
            masterDataRepository.Setup(m => m.Exists(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true));
            return masterDataRepository;
        }

        private static Mock<IMaterialRepository> MaterialRepositoryWithAMaterial()
        {
            var materialRepository = new Mock<IMaterialRepository>();
            IMaterial material = new Material
            {
                Headers = new List<IHeaderData>
                {
                    new HeaderData("General","General")
                    {
                        Name = "General",
                        Columns = new List<IColumnData>
                        {
                            new ColumnData("Can be Used as an Asset","Can be Used as an Asset",  true)
                        }
                    }
                }
            };
            var resultTask = Task.FromResult(material);
            materialRepository.Setup(m => m.Find("1")).Returns(resultTask);
            return materialRepository;
        }

        private static Mock<IMaterialRepository> MaterialRepositoryWithAsset()
        {
            var materialRepository = new Mock<IMaterialRepository>();
            IMaterial material = new Material
            {
                Headers = new List<IHeaderData>
                {
                    new HeaderData("General","General")
                    {
                        Name = "General",
                        Columns = new List<IColumnData>
                        {
                            new ColumnData("Can be Used as an Asset","Can be Used as an Asset", true)
                        }
                    }
                }
            };
            var resultTask = Task.FromResult(material);
            materialRepository.Setup(m => m.Find("1")).Returns(resultTask);
            return materialRepository;
        }

        private static Mock<IMaterialRepository> MaterialRepositoryWithoutAsset()
        {
            var materialRepository = new Mock<IMaterialRepository>();
            var material = new Mock<IMaterial>().Object;
            material.Headers = new List<IHeaderData>
            {
                new HeaderData("General","General")
                {
                    Name = "General",
                    Columns = new List<IColumnData>
                    {
                        new ColumnData("Can be Used as an Asset","Can be Used as an Asset", false)
                    }
                }
            };
            var resultTask = Task.FromResult(material);
            materialRepository.Setup(m => m.Find("1")).Returns(resultTask);
            return materialRepository;
        }

        private RentalRateController CreateFixture(Mock<IMasterDataRepository> masterDataRepository)
        {
            var rentalRateRepository = new Mock<IRentalRateRepository>();
            var materialRepository = MaterialRepositoryWithAMaterial();
            return CreateFixture(rentalRateRepository, materialRepository, masterDataRepository);
        }

        private RentalRateController CreateFixture()
        {
            var rentalRateRepository = new Mock<IRentalRateRepository>();
            var masterDataRepository = MasterDataRepositoryWithAsset();
            var materialRepository = MaterialRepositoryWithAMaterial();
            return CreateFixture(rentalRateRepository, materialRepository, masterDataRepository);
        }

        private RentalRateController CreateFixture(Mock<IRentalRateRepository> rentalRateRepository)
        {
            var masterDataRepository = MasterDataRepositoryWithAsset();
            var materialRepository = MaterialRepositoryWithAMaterial();
            return CreateFixture(rentalRateRepository, materialRepository, masterDataRepository);
        }

        private RentalRateController CreateFixture(Mock<IMaterialRepository> materialRepository)
        {
            var rentalRateRepository = new Mock<IRentalRateRepository>();
            var masterDataRepository = MasterDataRepositoryWithAsset();

            return CreateFixture(rentalRateRepository, materialRepository, masterDataRepository);
        }

        private RentalRateController CreateFixture(Mock<IRentalRateRepository> rentalRateRepository,
            Mock<IMaterialRepository> materialRepository, Mock<IMasterDataRepository> masterDataRepository)
        {
            var controller = new RentalRateController(rentalRateRepository.Object, materialRepository.Object,
                masterDataRepository.Object);
            return controller;
        }

        private RentalRateController CreateFixture(Mock<IRentalRateRepository> rentalRateRepository,
            Mock<IMaterialRepository> materialRepository)
        {
            var masterDataRepository = MasterDataRepositoryWithAsset();
            return CreateFixture(rentalRateRepository, materialRepository, masterDataRepository);
        }
    }
}