using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ControllerPort;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.RateMasters.Infrastructure.Controller
{
    public class ServiceRateControllerTest
    {
        [Fact]
        public async Task AverageLandedRateGet_ShouldReturnBadRequest_WhenRateMasterThrowsArgumentException()
        {
            var systemUnderTest = new Fixture()
                .HavingServiceMasterThatThrowsArgumentExceptionForAverageLandedRate()
                .SystemUnderTest();

            var result = await systemUnderTest.GetAverageLandedRate("serviceId", "location", DateTime.Today, "INR");

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async Task AverageLandedRateGet_ShouldReturnStatusCodeOK_WhenRateMasterRuturnedAverageLandedAndAverageControlBaseRate()
        {
            var systemUnderTest = new Fixture()
                .HavingServiceMasterThatReturnsSomeAverageLandedRate()
                .HavingServiceMasterThatReturnsSomeAverageControlBaseRate()
                .HavingServiceRepositoryTheReturnsSomeServiceContainsGeneralHeaderAndUnitOfMeasureColumn("serviceId")
                .SystemUnderTest();

            var result = await systemUnderTest.GetAverageLandedRate("serviceId", "location", DateTime.Today, "INR");

            result.Should().BeAssignableTo<OkNegotiatedContentResult<LandedRateDto>>();
        }

        [Fact]
        public async void BulkEdit_ShouldReturnResponseIndicatingCompleteSuccess_WhenRateMasterReturnCreatedRate()
        {
            var systemUnderTest = new Fixture()
                .CreateOfServiceRateMasterReturnSomeServiceRate()
                .SystemUnderTest();
            var serviceRateDto = new Mock<ServiceRateDto>();
            serviceRateDto.Setup(m => m.Domain(It.IsAny<IBank>(), It.IsAny<IMasterDataList>(), It.IsAny<IMasterDataList>())).Returns(new Mock<IServiceRate>().Object);
            var appliedOn = DateTime.Now.AddDays(1);
            serviceRateDto.Setup(m => m.AppliedOn).Returns(appliedOn);

            var result = await systemUnderTest.BulkEdit(new List<ServiceRateDto>() { serviceRateDto.Object });

            var content = ((OkNegotiatedContentResult<BuildRateResponse<ServiceRateDto>>)result).Content;
            content.Status.Should().Be("Succeeded");
            var buildRateResponseRecord = content.Records.FirstOrDefault();
            buildRateResponseRecord.Status.Should().Be(RateEditStatus.Created.ToString());
            buildRateResponseRecord.Message.Should().Be(String.Empty);
            buildRateResponseRecord.RecordData.Count.Should().Be(1);
            buildRateResponseRecord.RecordData.First().AppliedOn.Should().Be(appliedOn);
        }

        [Fact]
        public async void BulkEdit_ShouldReturnResponseIndicatingFailure_WhenRateCreationFailsForAllRecords()
        {
            var systemUnderTest = new Fixture()
                .CreateOfServiceRateMasterReturnSomeServiceRate()
                .SystemUnderTest();
            var serviceRateDto1 = new Mock<ServiceRateDto>();
            var serviceRateDto2 = new Mock<ServiceRateDto>();
            serviceRateDto1.Setup(m => m.Domain(It.IsAny<IBank>(), It.IsAny<IMasterDataList>(), It.IsAny<IMasterDataList>())).Returns(new Mock<IServiceRate>().Object);
            var appliedOn = DateTime.Now.AddDays(-1);
            serviceRateDto1.Setup(m => m.AppliedOn).Returns(appliedOn);
            serviceRateDto2.Setup(m => m.AppliedOn).Returns(appliedOn);

            var result = await systemUnderTest.BulkEdit(new List<ServiceRateDto>() { serviceRateDto1.Object, serviceRateDto2.Object });

            var content = ((OkNegotiatedContentResult<BuildRateResponse<ServiceRateDto>>)result).Content;
            content.Status.Should().Be("Failed");
            content.Records.Count.Should().Be(2);

            var buildRateResponseRecord1 = content.Records.FirstOrDefault();
            var buildRateResponseRecord2 = content.Records.LastOrDefault();
            buildRateResponseRecord1.Status.Should().Be(RateEditStatus.Error.ToString());
            buildRateResponseRecord1.Message.Should().Be("Applied on date cannot be set to past.");

            buildRateResponseRecord2.Status.Should().Be(RateEditStatus.Error.ToString());
            buildRateResponseRecord2.Message.Should().Be("Applied on date cannot be set to past.");
        }

        [Fact]
        public async void BulkEdit_ShouldReturnResponseIndicatingPartialSuccess_WhenRateMasterReturnCreatedRateForSomeRecords()
        {
            var systemUnderTest = new Fixture()
                .CreateOfServiceRateMasterReturnSomeServiceRate()
                .SystemUnderTest();
            var materialRateDto = new Mock<ServiceRateDto>();
            var invalidMaterialRateDto = new Mock<ServiceRateDto>();
            materialRateDto.Setup(m => m.Domain(It.IsAny<IBank>(), It.IsAny<IMasterDataList>(), It.IsAny<IMasterDataList>())).Returns(new Mock<IServiceRate>().Object);
            var appliedOn = DateTime.Now.AddDays(1);
            materialRateDto.Setup(m => m.AppliedOn).Returns(appliedOn);
            invalidMaterialRateDto.Setup(m => m.AppliedOn).Returns(DateTime.Now.AddDays(-5));
            var result = await systemUnderTest.BulkEdit(new List<ServiceRateDto>() { materialRateDto.Object, invalidMaterialRateDto.Object });

            var content = ((OkNegotiatedContentResult<BuildRateResponse<ServiceRateDto>>)result).Content;
            content.Status.Should().Be("PartiallySucceeded");
            var buildRateResponseSuccessRecord = content.Records.FirstOrDefault();
            var buildRateResponseFailedRecord = content.Records.LastOrDefault();
            buildRateResponseSuccessRecord.Status.Should().Be(RateEditStatus.Created.ToString());
            buildRateResponseSuccessRecord.Message.Should().Be(String.Empty);

            buildRateResponseFailedRecord.Status.Should().Be(RateEditStatus.Error.ToString());
            buildRateResponseFailedRecord.Message.Should().Be("Applied on date cannot be set to past.");
        }

        [Fact]
        public async Task Create_Should_Accept_When_AppliedOn_Is_Exactly_Midnight_NextDay()
        {
            var INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            var indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            var nextDate = indianTime.AddDays(1).Date;
            var timeRemainingToday = nextDate.Subtract(indianTime);
            var validTomorrow = DateTime.UtcNow.Add(timeRemainingToday);
            var systemUnderTest = new Fixture()
                 .CreateOfServiceRateMasterReturnSomeServiceRate()
                 .SystemUnderTest();
            var serviceRateDto = new Mock<ServiceRateDto>();
            var serviceRate = new Mock<IServiceRate>();
            serviceRate.Setup(s => s.AppliedOn).Returns(validTomorrow);
            serviceRateDto.Setup(m => m.Domain(It.IsAny<IBank>(), It.IsAny<IMasterDataList>(), It.IsAny<IMasterDataList>())).Returns(serviceRate.Object);
            serviceRateDto.Setup(m => m.AppliedOn).Returns(validTomorrow);

            var result = await systemUnderTest.Create(serviceRateDto.Object);
            result.Should().BeAssignableTo<CreatedNegotiatedContentResult<ServiceRateDto>>();
        }

        [Theory]
        [InlineData("2016 - 01 - 01T11: 30:00.0000000")]
        [InlineData("2016 - 01 - 01")]
        public async Task Create_Should_ReturnBadRequest_When_AppliedOn_Format_Is_Non_ISO(string date)
        {
            var appliedOn = DateTime.Parse(date);
            var systemUnderTest = new Fixture().SystemUnderTest();
            var result = await systemUnderTest.Create(new ServiceRateDto() { AppliedOn = appliedOn });
            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async Task Create_Should_ReturnBadRequest_When_AppliedOn_IsPast()
        {
            var currentUTCTime = DateTime.UtcNow;
            DateTime past = currentUTCTime.AddMinutes(-1);
            var systemUnderTest = new Fixture()
                 .CreateOfServiceRateMasterReturnSomeServiceRate()
                 .SystemUnderTest();
            var serviceRateDto = new Mock<ServiceRateDto>();
            var serviceRate = new Mock<IServiceRate>();
            serviceRate.Setup(s => s.AppliedOn).Returns(past);
            serviceRateDto.Setup(m => m.Domain(It.IsAny<IBank>(), It.IsAny<IMasterDataList>(), It.IsAny<IMasterDataList>())).Returns(serviceRate.Object);
            serviceRateDto.Setup(m => m.AppliedOn).Returns(past);
            var result = await systemUnderTest.Create(serviceRateDto.Object);
            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async Task Create_Should_ReturnBadRequest_When_AppliedOn_TodayIST()
        {
            var currentUTCTime = DateTime.UtcNow;
            DateTime futureButNotTomorrow = currentUTCTime.AddMinutes(1);
            var systemUnderTest = new Fixture()
                 .CreateOfServiceRateMasterReturnSomeServiceRate()
                 .SystemUnderTest();
            var serviceRateDto = new Mock<ServiceRateDto>();
            var serviceRate = new Mock<IServiceRate>();
            serviceRate.Setup(s => s.AppliedOn).Returns(futureButNotTomorrow);
            serviceRateDto.Setup(m => m.Domain(It.IsAny<IBank>(), It.IsAny<IMasterDataList>(), It.IsAny<IMasterDataList>())).Returns(serviceRate.Object);
            serviceRateDto.Setup(m => m.AppliedOn).Returns(futureButNotTomorrow);
            var result = await systemUnderTest.Create(serviceRateDto.Object);

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async Task Create_ShouldAcceptAppliedOnAsNextDayInIST()
        {
            var systemUnderTest = new Fixture()
                 .CreateOfServiceRateMasterReturnSomeServiceRate()
                 .SystemUnderTest();
            var serviceRateDto = new Mock<ServiceRateDto>();
            var serviceRate = new Mock<IServiceRate>();
            var dateTime = DateTime.Now.AddDays(1);
            serviceRate.Setup(s => s.AppliedOn).Returns(dateTime);
            serviceRateDto.Setup(m => m.Domain(It.IsAny<IBank>(), It.IsAny<IMasterDataList>(), It.IsAny<IMasterDataList>())).Returns(serviceRate.Object);
            serviceRateDto.Setup(m => m.AppliedOn).Returns(dateTime);

            var result = await systemUnderTest.Create(serviceRateDto.Object);
            result.Should().BeAssignableTo<CreatedNegotiatedContentResult<ServiceRateDto>>();
        }

        [Fact]
        public async void Create_ShouldReturnBadRequest_WhenServiceRateMasterThrowsArgumentException()
        {
            var systemUnderTest = new Fixture()
                .CreateOfServiceRateMasterThrowArgumentException()
                .SystemUnderTest();

            var result = await systemUnderTest.Create(new Mock<ServiceRateDto>().Object);

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async void Create_ShouldReturnCreated_WhenRateMasterReturnCreatedRate()
        {
            var systemUnderTest = new Fixture()
                .CreateOfServiceRateMasterReturnSomeServiceRate()
                .SystemUnderTest();
            var serviceRateDto = new Mock<ServiceRateDto>();
            var serviceRate = new Mock<IServiceRate>();
            var dateTime = DateTime.Now.AddDays(2);
            serviceRate.Setup(s => s.AppliedOn).Returns(dateTime);
            serviceRateDto.Setup(m => m.Domain(It.IsAny<IBank>(), It.IsAny<IMasterDataList>(), It.IsAny<IMasterDataList>())).Returns(serviceRate.Object);
            serviceRateDto.Setup(m => m.AppliedOn).Returns(dateTime);

            var result = await systemUnderTest.Create(serviceRateDto.Object);
            result.Should().BeAssignableTo<CreatedNegotiatedContentResult<ServiceRateDto>>();
        }

        [Fact]
        public async Task Get_Should_AcceptOnDateOnlyWithISOFormat()
        {
            var systemUnderTest = new Fixture()
                 .HavingServiceRateMasterThatThrowResourceNotFoundException()
                 .SystemUnderTest();

            var result = await systemUnderTest.Get("serviceId", "location", "typeOfPurchase", DateTime.Parse("2016-01-01"));

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
        }

        [Fact]
        public void Get_Should_QueryServiceRaterMasterToGetServiceRate()
        {
            var on = DateTime.Today;
            const string serviceid = "serviceId";
            const string location = "location";
            const string typeOfPurchase = "typeOfPurchase";
            var fixture = new Fixture()
                .GetOfServiceRateMasterReturnSomeServiceRate()
                .ServiceMasterAccepting(location, serviceid, on, typeOfPurchase);

            fixture.SystemUnderTest().Get(serviceid, location, typeOfPurchase, on);

            fixture.VerifyExpectations();
        }

        [Fact]
        public async Task Get_Should_ReturnResourceNotFoundIfServiceMasterThrowsResourceNotFoundException()
        {
            var systemUnderTest = new Fixture()
                .HavingServiceRateMasterThatThrowResourceNotFoundException()
                .SystemUnderTest();

            var result = await systemUnderTest.Get("serviceId", "location", "typeOfPurchase", DateTime.Today);

            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async Task Get_Should_ReturnStatusCodeOkIfServiceIsReturnedAsServiceRateMaster()
        {
            var systemUnderTest = new Fixture()
                .GetOfServiceRateMasterReturnSomeServiceRate()
                .SystemUnderTest();

            var result = await systemUnderTest.Get("serviceId", "location", "typeOfPurchase", DateTime.Today);

            result.Should().BeAssignableTo<OkNegotiatedContentResult<ServiceRateDto>>();
        }

        [Fact]
        public async Task Get_ShouldReturnBadRequest_WhenBankThrowsArgumentException()
        {
            var systemUnderTest = new Fixture()
                .GetOfServiceRateMasterReturnSomeServiceRates()
                .HavingBankThatThrowsArgumentException()
                .SystemUnderTest();

            var result = await systemUnderTest.Get("serviceId", DateTime.Today);

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async Task Get_ShouldReturnBadRequest_WhenServiceMasterThrowsArgumentException()
        {
            var systemUnderTest = new Fixture()
                .HavingServiceRateMasterThatThrowArgumentException()
                .SystemUnderTest();

            var result = await systemUnderTest.Get("serviceId", "location", "typeOfPurchase", DateTime.Today);

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async Task GetRateHistory_ShouldReturnBadRequest_WhenServiceRateMasterThrowsArugementException()
        {
            var serviceRateSearchRequest = new ServiceRateSearchRequest();
            var systemUnderTest = new Fixture()
                .HavingServiceRateMasterThatThrowArgumentException()
                .SystemUnderTest();

            var result = await systemUnderTest.GetRateHistory("FDP0001", serviceRateSearchRequest);

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async Task GetRateHistory_ShouldReturnNotFound_WhenServiceRateMasterThrowsResourceNotFoundException()
        {
            var serviceRateSearchRequest = new ServiceRateSearchRequest();
            var systemUnderTest = new Fixture()
                .HavingServiceRateMasterThatThrowResourceNotFoundException()
                .SystemUnderTest();

            var result = await systemUnderTest.GetRateHistory("FDP0001", serviceRateSearchRequest);

            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async Task GetRateHistory_ShouldreturnStatusCodeOKWithListOfServiceRates_WhenServiceMasterReturnsListOfServiceRates()
        {
            var serviceRateSearchRequest = new ServiceRateSearchRequest();
            var systemUnderTest = new Fixture()
                .GetOfServiceRateMasterReturnSomeHistoryOfServiceRates()
                .SystemUnderTest();

            var result = await systemUnderTest.GetRateHistory("serviceId", serviceRateSearchRequest);
            result.Should().BeAssignableTo<OkNegotiatedContentResult<PaginatedAndSortedListDto<ServiceRateDto>>>();
        }

        [Fact]
        public void Gets_Should_QueryServiceRaterMasterToGetServiceRates()
        {
            var on = DateTime.Today;
            const string serviceid = "serviceId";
            var fixture = new Fixture()
                .GetOfServiceRateMasterReturnSomeServiceRates()
                .HavingServiceMasterThatReturnsSomeControlBaseRate()
                .ServiceMasterAccepting(serviceid, on);

            fixture.SystemUnderTest().Get(serviceid, on);

            fixture.VerifyExpectations();
        }

        [Fact]
        public async Task Gets_Should_ReturnResourceNotFoundIfServiceMasterThrowsResourceNotFoundException()
        {
            var systemUnderTest = new Fixture()
                .HavingServiceRateMasterThatThrowResourceNotFoundException()
                .SystemUnderTest();

            var result = await systemUnderTest.Get("serviceId", DateTime.Today);

            result.Should().BeAssignableTo<NotFoundResult>();
        }

//        [Fact]
//        public async Task Gets_Should_ReturnStatusCodeOkIfServiceIsReturnedAsServiceRateMaster()
//        {
//            var systemUnderTest = new Fixture()
//                .GetOfServiceRateMasterReturnSomeServiceRates()
//                .HavingServiceMasterThatReturnsSomeControlBaseRate()
//                .SystemUnderTest();
//
//            var result = await systemUnderTest.Get("serviceId", DateTime.Today);
//
//            result.Should().BeAssignableTo<OkNegotiatedContentResult<ServiceRateDto[]>>();
//        }

        [Fact]
        public async Task Gets_ShouldReturnBadRequest_WhenServiceMasterThrowsArgumentException()
        {
            var systemUnderTest = new Fixture()
                .HavingServiceRateMasterThatThrowArgumentException()
                .SystemUnderTest();

            var result = await systemUnderTest.Get("serviceId", DateTime.Today);

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async void LandedRateGet_ShouldReturnBadRequest_WhenRateMasterThrowsArgumentException()
        {
            var systemUnderTest = new Fixture()
                .HavingServiceMasterThatThrowsArgumentExceptionForLandedRate()
                .SystemUnderTest();

            var result = await systemUnderTest.GetLandedRate("serviceId", "location", "typeOfPurchase", DateTime.Today);

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async void LandedRateGet_ShouldReturnStatusCodeOK_WhenRateMasterRuturnedLandedAndControlBaseRateWithoutProcurementRateThreshold()
        {
            var systemUnderTest = new Fixture()
                .HavingServiceMasterThatReturnsSomeLandedRate()
                .HavingServiceMasterThatReturnsSomeControlBaseRate()
                .HavingServiceRepositoryThatReturnsSomeServiceWithInvalidProcurementRateThreshold()
                .SystemUnderTest();

            var result = await systemUnderTest.GetLandedRate("serviceId", "location", "typeOfPurchase", DateTime.Today);

            result.Should().BeAssignableTo<OkNegotiatedContentResult<LandedRateDto>>();
        }

        [Fact]
        public async void LandedRateGet_ShouldReturnStatusCodeOK_WhenRateMasterRuturnedLandedAndControlBaseRateWithProcurementRateThreshold()
        {
            var systemUnderTest = new Fixture()
                .HavingServiceMasterThatReturnsSomeLandedRate()
                .HavingServiceMasterThatReturnsSomeControlBaseRate()
                .HavingServiceRepositoryThatReturnsSomeServiceWithValidProcurementRateThreshold()
                .SystemUnderTest();

            var result = await systemUnderTest.GetLandedRate("serviceId", "location", "typeOfPurchase", DateTime.Today);

            result.Should().BeAssignableTo<OkNegotiatedContentResult<LandedRateDto>>();
        }

        [Fact]
        public void Should_ImplementApiController()
        {
            var systemUnderTest = new Fixture().SystemUnderTest();

            systemUnderTest.Should().BeAssignableTo<ApiController>();
        }

        private class Fixture
        {
            private readonly Mock<IBank> _bank = new Mock<IBank>();
            private readonly List<Action> _expectations = new List<Action>();

            private readonly Mock<IServiceRateService> _serviceRateMaster = new Mock<IServiceRateService>();
            private readonly Mock<IServiceRepository> _serviceRepository = new Mock<IServiceRepository>();
            private readonly Mock<IMasterDataRepository> _masterDataRepository = new Mock<IMasterDataRepository>();

            public Fixture CreateOfServiceRateMasterReturnSomeServiceRate()
            {
                var serviceRate = ServiceRateStubWithRateThatHasControlBaseRateAndLandedRate().Object;
                _serviceRateMaster.Setup(m => m.CreateRate(It.IsAny<IServiceRate>()))
                    .ReturnsAsync(serviceRate);
                return this;
            }

            public Fixture CreateOfServiceRateMasterThrowArgumentException()
            {
                _serviceRateMaster.Setup(m => m.CreateRate(It.IsAny<IServiceRate>()))
                    .Throws(new ArgumentException());
                return this;
            }

            public Fixture GetOfServiceRateMasterReturnSomeHistoryOfServiceRates()
            {
                SetUpServiceRateMasterRateHistroyToReturn(ServiceRatesStubWithRatesThatHasControlBaseRateAndLandedRateForRateHistory());
                return this;
            }

            public Fixture GetOfServiceRateMasterReturnSomeServiceRate()
            {
                SetUpServiceRateMasterToReturn(ServiceRateStubWithRateThatHasControlBaseRateAndLandedRate().Object);
                return this;
            }

            public Fixture GetOfServiceRateMasterReturnSomeServiceRates()
            {
                SetUpServiceRateMasterToReturn(ServiceRateStubWithRatesThatHasControlBaseRateAndLandedRate());
                return this;
            }

            public Fixture HavingBankThatThrowsArgumentException()
            {
                _bank.Setup(b => b.ConvertTo(It.IsAny<Money>(), It.IsAny<string>())).Throws(new ArgumentException());
                return this;
            }

            public Fixture HavingServiceMasterThatReturnsSomeAverageControlBaseRate()
            {
                _serviceRateMaster.Setup(
                        m => m.GetAverageControlBaseRate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                            It.IsAny<string>()))
                    .ReturnsAsync(new Money(10m, "INR", new Mock<IBank>().Object));
                _bank.Setup(b => b.ConvertTo(new Money(10m, "INR", new Mock<IBank>().Object), "INR"))
                    .ReturnsAsync(new Money(10m, "INR", new Mock<IBank>().Object));
                return this;
            }

            public Fixture HavingServiceMasterThatReturnsSomeAverageLandedRate()
            {
                _serviceRateMaster.Setup(
                        m => m.GetAverageLandedRate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                            It.IsAny<string>()))
                    .ReturnsAsync(new Money(15m, "INR", new Mock<IBank>().Object));
                _bank.Setup(b => b.ConvertTo(new Money(15m, "INR", new Mock<IBank>().Object), "INR"))
                    .ReturnsAsync(new Money(15m, "INR", new Mock<IBank>().Object));
                return this;
            }

            public Fixture HavingServiceMasterThatReturnsSomeControlBaseRate()
            {
                _serviceRateMaster.Setup(
                        m => m.GetControlBaseRate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                            It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(new Money(10m, "INR", new Mock<IBank>().Object));
                _bank.Setup(b => b.ConvertTo(new Money(10m, "INR", new Mock<IBank>().Object), "INR"))
                .ReturnsAsync(new Money(10m, "INR", new Mock<IBank>().Object));
                return this;
            }

            public Fixture HavingServiceMasterThatReturnsSomeLandedRate()
            {
                _serviceRateMaster.Setup(
                        m => m.GetLandedRate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                            It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(new Money(15m, "INR", new Mock<IBank>().Object));
                _bank.Setup(b => b.ConvertTo(new Money(15m, "INR", new Mock<IBank>().Object), "INR"))
                .ReturnsAsync(new Money(15m, "INR", new Mock<IBank>().Object));
                return this;
            }

            public Fixture HavingServiceMasterThatThrowsArgumentExceptionForAverageLandedRate()
            {
                _serviceRateMaster.Setup(m => m.GetAverageControlBaseRate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                           It.IsAny<string>())).Throws(new ArgumentException());
                return this;
            }

            public Fixture HavingServiceMasterThatThrowsArgumentExceptionForLandedRate()
            {
                _serviceRateMaster.Setup(m => m.GetLandedRate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                            It.IsAny<string>(), It.IsAny<string>())).Throws(new ArgumentException());
                return this;
            }

            public Fixture HavingServiceRateMasterThatThrowArgumentException()
            {
                SetupServiceRateMasterToThrow(new ArgumentException("Location cannot be null."));
                return this;
            }

            public Fixture HavingServiceRateMasterThatThrowResourceNotFoundException()
            {
                SetupServiceRateMasterToThrow(new ResourceNotFoundException("ServiceRate"));
                return this;
            }

            public Fixture HavingServiceRepositoryThatReturnsSomeServiceWithInvalidProcurementRateThreshold()
            {
                var serviceDefinition = new ServiceDefinition("Masonry & Plaster");
                var headerData = new List<IHeaderData>
                {
                    new HeaderData("Classification","Classification")
                    {
                        Columns = new List<IColumnData>
                        {
                            new ColumnData("Service Level 1","Service Level 1",  "Masonry & Plaster")
                        }
                    },
                    new HeaderData("Purchase","Purchase")
                    {
                        Columns = new List<IColumnData>
                        {
                            new ColumnData("Procurement Rate Threshold %","Procurement Rate Threshold %", null)
                        }
                    }
                };
                var service = new Service(headerData, serviceDefinition) { Id = "J001" };

                _serviceRepository.Setup(
                        m => m.Find(It.IsAny<string>()))
                    .ReturnsAsync(service);

                return this;
            }

            public Fixture HavingServiceRepositoryThatReturnsSomeServiceWithValidProcurementRateThreshold()
            {
                var serviceDefinition = new ServiceDefinition("Masonry & Plaster");
                var headerData = new List<IHeaderData>
                {
                    new HeaderData("Classification","Classification")
                    {
                        Columns = new List<IColumnData>
                        {
                            new ColumnData("Service Level 1","Service Level 1", "Masonry & Plaster")
                        }
                    },
                    new HeaderData("Purchase","Purchase")
                    {
                        Columns = new List<IColumnData>
                        {
                            new ColumnData("Procurement Rate Threshold %","Procurement Rate Threshold %", 10)
                        }
                    }
                };
                var service = new Service(headerData, serviceDefinition) { Id = "J001" };

                _serviceRepository.Setup(
                        m => m.Find(It.IsAny<string>()))
                    .ReturnsAsync(service);

                return this;
            }

            public Fixture ServiceMasterAccepting(string location, string serviceid, DateTime on,
                string typeOfPurchase)
            {
                _expectations.Add(
                    () =>
                        _serviceRateMaster.Verify(
                            m => m.GetRate(serviceid, location, @on, typeOfPurchase), Times.Once));
                return this;
            }

            public Fixture ServiceMasterAccepting(string serviceid, DateTime on)
            {
                _expectations.Add(
                    () =>
                        _serviceRateMaster.Verify(
                            m => m.GetRates(serviceid, DateTime.Today), Times.Once));
                return this;
            }

            public void SetUpServiceRateMasterRateHistroyToReturn(IEnumerable<IServiceRate> serviceRates)
            {
                var serviceRateSearchRequest = new ServiceRateSearchRequest();
                _serviceRateMaster.Setup(
                        m =>
                            m.GetRateHistory(It.IsAny<string>(), serviceRateSearchRequest))
                    .ReturnsAsync(new PaginatedAndSortedList<IServiceRate>(serviceRates, 1, serviceRates.Count(), 20, "AppliedOn", SortOrder.Descending));
            }

            public ServiceRateController SystemUnderTest()
            {
                return new ServiceRateController(_serviceRateMaster.Object, _bank.Object, _serviceRepository.Object, _masterDataRepository.Object);
            }

            public void VerifyExpectations()
            {
                _expectations.ForEach(e => e.Invoke());
            }

            private static List<IServiceRate> ServiceRatesStubWithRatesThatHasControlBaseRateAndLandedRateForRateHistory()
            {
                var serviceRate = new Mock<IServiceRate>();

                serviceRate.Setup(r => r.ControlBaseRate).Returns(new Money(10m, "INR", new Mock<IBank>().Object));
                serviceRate.Setup(r => r.LandedRate()).ReturnsAsync(new Money(15m, "INR", new Mock<IBank>().Object));

                var serviceRates = new List<IServiceRate> { serviceRate.Object };
                return serviceRates;
            }

            private static IEnumerable<IServiceRate> ServiceRateStubWithRatesThatHasControlBaseRateAndLandedRate()
            {
                var serviceRates = new List<IServiceRate>();
                var serviceRate = new Mock<IServiceRate>();
                serviceRate.Setup(r => r.ControlBaseRate).Returns(new Money(10m, "INR", new Mock<IBank>().Object));
                serviceRate.Setup(r => r.LandedRate()).ReturnsAsync(new Money(15m, "INR", new Mock<IBank>().Object));
                serviceRate.Setup(r => r.Id).Returns("serviceId");
                serviceRate.Setup(r => r.Location).Returns("location");
                serviceRate.Setup(r => r.TypeOfPurchase).Returns("typeOfPurchase");
                var dateTime = DateTime.Today.AddDays(2);
                serviceRate.Setup(r => r.AppliedOn).Returns(dateTime);
                serviceRates.Add(serviceRate.Object);
                return serviceRates;
            }

            private static Mock<IServiceRate> ServiceRateStubWithRateThatHasControlBaseRateAndLandedRate()
            {
                var serviceRate = new Mock<IServiceRate>();
                serviceRate.Setup(r => r.ControlBaseRate).Returns(new Money(10m, "INR", new Mock<IBank>().Object));
                serviceRate.Setup(r => r.LandedRate()).ReturnsAsync(new Money(15m, "INR", new Mock<IBank>().Object));
                return serviceRate;
            }

            private void SetUpServiceRateMasterToReturn(IServiceRate serviceRateDto)
            {
                _serviceRateMaster.Setup(
                        m =>
                            m.GetRate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                                It.IsAny<string>()))
                    .ReturnsAsync(serviceRateDto);
            }

            private void SetUpServiceRateMasterToReturn(IEnumerable<IServiceRate> serviceRateDtos)
            {
                _serviceRateMaster.Setup(
                        m =>
                            m.GetRates(It.IsAny<string>(), It.IsAny<DateTime>()))
                    .ReturnsAsync(serviceRateDtos);
            }

            private void SetupServiceRateMasterToThrow(Exception exception)
            {
                var serviceRateSearchRequest = new ServiceRateSearchRequest();
                _serviceRateMaster.Setup(
                        m =>
                            m.GetRate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                                It.IsAny<string>()))
                    .Throws(exception);

                _serviceRateMaster.Setup(
                        m =>
                            m.GetRates(It.IsAny<string>(), It.IsAny<DateTime>()))
                    .Throws(exception);
                _serviceRateMaster.Setup(
                       m =>
                           m.GetRateHistory(It.IsAny<string>(), serviceRateSearchRequest))
                   .Throws(exception);
            }

            public Fixture HavingServiceRepositoryTheReturnsSomeServiceContainsGeneralHeaderAndUnitOfMeasureColumn(string serviceid)
            {
                var service = new Mock<IService>();
                var headers = new Mock<IHeaderData>();
                var column = new Mock<IColumnData>();
                headers.Setup(h => h.Key).Returns("general");
                column.Setup(c => c.Key).Returns("unit_of_measure");
                column.Setup(c => c.Value).Returns("Load");
                service.Setup(m => m.Headers).Returns(new List<IHeaderData> { headers.Object });
                headers.Setup(h => h.Columns).Returns(new List<IColumnData> { column.Object });
                _serviceRepository.Setup(m => m.Find(serviceid)).ReturnsAsync(service.Object);
                return this;
            }
        }
    }
}