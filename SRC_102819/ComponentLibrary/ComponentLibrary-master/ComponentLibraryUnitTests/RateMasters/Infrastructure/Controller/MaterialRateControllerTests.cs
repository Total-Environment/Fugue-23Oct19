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
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.RateMasters.Infrastructure.Controller
{
    public class MaterialRateControllerTests
    {
        [Fact]
        public async Task AverageLandedRateGet_ShouldReturnBadRequest_WhenRateMasterThrowsArgumentException()
        {
            var systemUnderTest = new Fixture()
                .HavingMaterialMasterThatThrowsArgumentExceptionForAverageLandedRate()
                .SystemUnderTest();

            var result = await systemUnderTest.GetAverageLandedRate("materialId", "location", DateTime.Today, "INR");

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async Task AverageLandedRateGet_ShouldReturnStatusCodeOK_WhenRateMasterRuturnedAverageLandedAndAverageControlBaseRateWhenDateInFuture()
        {
            var systemUnderTest = new Fixture()
                .HavingMaterialMasterThatReturnsSomeAverageLandedRate()
                .HavingMaterialMasterThatReturnsSomeAverageControlBaseRate()
                .HavingMaterialRepositoryThatReturnsMaterialContainsGeneralHeaderAndUnitOfMeasureColumn("materialId")
                .SystemUnderTest();

            var result = await systemUnderTest.GetAverageLandedRate("materialId", "location", DateTime.Today.AddDays(20), "INR");

            result.Should().BeAssignableTo<OkNegotiatedContentResult<LandedRateDto>>();
        }

        [Fact]
        public async Task AverageLandedRateGet_ShouldReturnStatusCodeOK_WhenRateMasterRuturnedAverageLandedAndAverageControlBaseRateWhenDateIsToday()
        {
            var systemUnderTest = new Fixture()
                .HavingMaterialMasterThatReturnsSomeAverageLandedRate()
                .HavingMaterialMasterThatReturnsSomeAverageControlBaseRate()
                .HavingMaterialRepositoryThatReturnsMaterialContainsGeneralHeaderAndUnitOfMeasureColumn("materialId")
                .SystemUnderTest();

            var result = await systemUnderTest.GetAverageLandedRate("materialId", "location", DateTime.Today, "INR");

            result.Should().BeAssignableTo<OkNegotiatedContentResult<LandedRateDto>>();
        }

        [Fact]
        public async void BulkEdit_ShouldReturnResponseIndicatingCompleteSuccess_WhenRateMasterReturnCreatedRate()
        {
            var systemUnderTest = new Fixture()
                .CreateOfMaterialRateMasterReturnSomeMaterialRate()
                .SystemUnderTest();
            var materialRateDto = new Mock<MaterialRateDto>();
            materialRateDto.Setup(m => m.Domain(It.IsAny<IBank>(), It.IsAny<IMasterDataList>(), It.IsAny<IMasterDataList>())).Returns(new Mock<IMaterialRate>().Object);
            var appliedOn = DateTime.Now.AddDays(1);
            materialRateDto.Setup(m => m.AppliedOn).Returns(appliedOn);

            var result = await systemUnderTest.BulkEdit(new List<MaterialRateDto>() { materialRateDto.Object });

            var content = ((OkNegotiatedContentResult<BuildRateResponse<MaterialRateDto>>)result).Content;
            content.Status.Should().Be("Succeeded");
            var buildRateResponseRecord = content.Records.First();
            buildRateResponseRecord.Status.Should().Be(RateEditStatus.Created.ToString());
            buildRateResponseRecord.Message.Should().Be(string.Empty);
            buildRateResponseRecord.RecordData.Count.Should().Be(1);
            buildRateResponseRecord.RecordData.First().AppliedOn.Should().Be(appliedOn);
        }

        [Fact]
        public async void BulkEdit_ShouldReturnResponseIndicatingFailure_WhenRateCreationFailsForAllRecords()
        {
            var systemUnderTest = new Fixture()
                .CreateOfMaterialRateMasterReturnSomeMaterialRate()
                .SystemUnderTest();
            var materialRateDto1 = new Mock<MaterialRateDto>();
            var materialRateDto2 = new Mock<MaterialRateDto>();
            materialRateDto1.Setup(m => m.Domain(It.IsAny<IBank>(), It.IsAny<IMasterDataList>(), It.IsAny<IMasterDataList>())).Returns(new Mock<IMaterialRate>().Object);
            var appliedOn = DateTime.Now.AddDays(-1);
            materialRateDto1.Setup(m => m.AppliedOn).Returns(appliedOn);
            materialRateDto2.Setup(m => m.AppliedOn).Returns(appliedOn);

            var result = await systemUnderTest.BulkEdit(new List<MaterialRateDto>() { materialRateDto1.Object, materialRateDto2.Object });

            var content = ((OkNegotiatedContentResult<BuildRateResponse<MaterialRateDto>>)result).Content;
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
                .CreateOfMaterialRateMasterReturnSomeMaterialRate()
                .SystemUnderTest();
            var materialRateDto = new Mock<MaterialRateDto>();
            var invalidMaterialRateDto = new Mock<MaterialRateDto>();
            materialRateDto.Setup(m => m.Domain(It.IsAny<IBank>(), It.IsAny<IMasterDataList>(), It.IsAny<IMasterDataList>())).Returns(new Mock<IMaterialRate>().Object);
            var appliedOn = DateTime.Now.AddDays(1);
            materialRateDto.Setup(m => m.AppliedOn).Returns(appliedOn);
            invalidMaterialRateDto.Setup(m => m.AppliedOn).Returns(DateTime.Now.AddDays(-5));
            var result = await systemUnderTest.BulkEdit(new List<MaterialRateDto>() { materialRateDto.Object, invalidMaterialRateDto.Object });

            var content = ((OkNegotiatedContentResult<BuildRateResponse<MaterialRateDto>>)result).Content;
            content.Status.Should().Be("PartiallySucceeded");
            var buildRateResponseSuccessRecord = content.Records.FirstOrDefault();
            var buildRateResponseFailedRecord = content.Records.LastOrDefault();
            buildRateResponseSuccessRecord.Status.Should().Be(RateEditStatus.Created.ToString());
            buildRateResponseSuccessRecord.Message.Should().Be(String.Empty);

            buildRateResponseFailedRecord.Status.Should().Be(RateEditStatus.Error.ToString());
            buildRateResponseFailedRecord.Message.Should().Be("Applied on date cannot be set to past.");
        }

        [Theory]
        [InlineData("2016 - 01 - 01T11: 30:00.0000000")]
        [InlineData("2016 - 01 - 01")]
        public async void Create_ShouldReturnBadRequest_WhenappliedOnFormatIsNonISO(string date)
        {
            var appliedOn = DateTime.Parse(date);
            var systemUnderTest = new Fixture()
                .CreateOfMaterialRateMasterReturnSomeMaterialRate()
                .SystemUnderTest();
            var materialRateDto = new Mock<MaterialRateDto>();
            materialRateDto.Setup(m => m.Domain(It.IsAny<IBank>(), It.IsAny<IMasterDataList>(), It.IsAny<IMasterDataList>())).Returns(new Mock<IMaterialRate>().Object);
            materialRateDto.Setup(m => m.AppliedOn).Returns(appliedOn);

            var result = await systemUnderTest.Create(materialRateDto.Object);

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async void Create_ShouldReturnBadRequest_WhenappliedOnIsPast()
        {
            var appliedOn = DateTime.UtcNow.AddMinutes(-1);
            var systemUnderTest = new Fixture()
                .CreateOfMaterialRateMasterReturnSomeMaterialRate()
                .SystemUnderTest();
            var materialRateDto = new Mock<MaterialRateDto>();
            materialRateDto.Setup(m => m.Domain(It.IsAny<IBank>(), It.IsAny<IMasterDataList>(), It.IsAny<IMasterDataList>())).Returns(new Mock<IMaterialRate>().Object);
            materialRateDto.Setup(m => m.AppliedOn).Returns(appliedOn);

            var result = await systemUnderTest.Create(materialRateDto.Object);

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async void Create_ShouldReturnBadRequest_WhenappliedOnIsToday()
        {
            var appliedOn = DateTime.UtcNow.AddMinutes(1);
            var systemUnderTest = new Fixture()
                .CreateOfMaterialRateMasterReturnSomeMaterialRate()
                .SystemUnderTest();
            var materialRateDto = new Mock<MaterialRateDto>();
            materialRateDto.Setup(m => m.Domain(It.IsAny<IBank>(), It.IsAny<IMasterDataList>(), It.IsAny<IMasterDataList>())).Returns(new Mock<IMaterialRate>().Object);
            materialRateDto.Setup(m => m.AppliedOn).Returns(appliedOn);

            var result = await systemUnderTest.Create(materialRateDto.Object);

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async void Create_ShouldReturnBadRequest_WhenMaterialRateMasterThrowsArgumentException()
        {
            var systemUnderTest = new Fixture()
                .CreateOfMaterialRateMasterThrowArgumentException()
                .SystemUnderTest();

            var result = await systemUnderTest.Create(new Mock<MaterialRateDto>().Object);

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async void Create_ShouldReturnCreated_WhenRateMasterReturnCreatedRate()
        {
            var systemUnderTest = new Fixture()
                .CreateOfMaterialRateMasterReturnSomeMaterialRate()
                .SystemUnderTest();
            var materialRateDto = new Mock<MaterialRateDto>();
            materialRateDto.Setup(m => m.Domain(It.IsAny<IBank>(), It.IsAny<IMasterDataList>(), It.IsAny<IMasterDataList>())).Returns(new Mock<IMaterialRate>().Object);
            materialRateDto.Setup(m => m.AppliedOn).Returns(DateTime.Now.AddDays(1));

            var result = await systemUnderTest.Create(materialRateDto.Object);

            result.Should().BeAssignableTo<CreatedNegotiatedContentResult<MaterialRateDto>>();
        }

        [Fact]
        public async void Get_Should_QueryMaterialRaterMasterToGetMaterialRate()
        {
            var on = DateTime.Today;
            const string materialid = "materialId";
            const string location = "location";
            const string typeOfPurchase = "typeOfPurchase";
            var fixture = new Fixture()
                .GetOfMaterialRateMasterReturnSomeMaterialRate()
                .MaterialMasterAccepting(location, materialid, on, typeOfPurchase);

            await fixture.SystemUnderTest().Get(materialid, location, typeOfPurchase, on);

            fixture.VerifyExpectations();
        }

        [Fact]
        public async Task Get_Should_ReturnResourceNotFoundIfMaterialMasterThrowsResourceNotFoundException()
        {
            var systemUnderTest = new Fixture()
                .HavingMaterialRateMasterThatThrowResourceNotFoundException()
                .SystemUnderTest();

            var result = await systemUnderTest.Get("materialId", "location", "typeOfPurchase", DateTime.Today);

            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async Task Get_Should_ReturnStatusCodeOkIfMaterialIsReturnedAsMaterialRateMaster()
        {
            var systemUnderTest = new Fixture()
                .GetOfMaterialRateMasterReturnSomeMaterialRate()
                .SystemUnderTest();

            var result = await systemUnderTest.Get("materialId", "location", "typeOfPurchase", DateTime.Today);

            result.Should().BeAssignableTo<OkNegotiatedContentResult<MaterialRateDto>>();
        }

        [Fact]
        public async Task Get_ShouldReturnBadRequest_WhenBankThrowsArgumentException()
        {
            var systemUnderTest = new Fixture()
                .GetOfMaterialRateMasterReturnSomeMaterialRates()
                .HavingBankThatThrowsArgumentException()
                .SystemUnderTest();

            var result = await systemUnderTest.Get("materialId", DateTime.Today);

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async Task Get_ShouldReturnBadRequest_WhenMaterialMasterThrowsArgumentException()
        {
            var systemUnderTest = new Fixture()
                .HavingMaterialRateMasterThatThrowArgumentException()
                .SystemUnderTest();

            var result = await systemUnderTest.Get("materialId", "location", "typeOfPurchase", DateTime.Today);

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async Task GetRateHistory_ShouldReturnBadRequest_WhenRateMasterThrowsArugementException()
        {
            var materialRateSearchRequest = new MaterialRateSearchRequest();
            var systemUnderTest = new Fixture()
                .HavingMaterialRateMasterThatThrowArgumentException()
                .SystemUnderTest();

            var result = await systemUnderTest.GetRateHistroy("ALM0001", materialRateSearchRequest);

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async Task GetRateHistory_ShouldReturnNotFound_WhenRateMasterThrowsResourceNotFoundException()
        {
            var materialRateSearchRequest = new MaterialRateSearchRequest();
            var systemUnderTest = new Fixture()
                .HavingMaterialRateMasterThatThrowResourceNotFoundException()
                .SystemUnderTest();

            var result = await systemUnderTest.GetRateHistroy("ALM0001", materialRateSearchRequest);

            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async Task GetRateHistory_ShouldReturnStatusCodeOKListOfMaterialRates_WhenRateMasterReturnsListOfmaterialRates()
        {
            var materialRateSearchRequest = new MaterialRateSearchRequest();
            var systemUnderTest = new Fixture()
                .GetOfMaterialRateMasterReturnSomeHistroyOfMaterialRates()
                .SystemUnderTest();

            var result = await systemUnderTest.GetRateHistroy("materialId", materialRateSearchRequest);

            result.Should().BeAssignableTo<OkNegotiatedContentResult<PaginatedAndSortedListDto<MaterialRateDto>>>();
        }

        [Fact]
        public void Gets_Should_QueryMaterialRaterMasterToGetMaterialRates()
        {
            var on = DateTime.Today;
            const string materialid = "materialId";
            var fixture = new Fixture()
                .GetOfMaterialRateMasterReturnSomeMaterialRates()
                .HavingMaterialMasterThatReturnsSomeControlBaseRate()
                .MaterialMasterAccepting(materialid, on);

            fixture.SystemUnderTest().Get(materialid, DateTime.Today);

            fixture.VerifyExpectations();
        }

        [Fact]
        public async Task Gets_Should_ReturnResourceNotFoundIfMaterialMasterThrowsResourceNotFoundException()
        {
            var systemUnderTest = new Fixture()
                .HavingMaterialRateMasterThatThrowResourceNotFoundException()
                .SystemUnderTest();

            var result = await systemUnderTest.Get("materialId", DateTime.Today);

            result.Should().BeAssignableTo<NotFoundResult>();
        }

//        [Fact]
//        public async Task Gets_Should_ReturnStatusCodeOkIfMaterialIsReturnedAsMaterialRateMaster()
//        {
//            var systemUnderTest = new Fixture()
//                .GetOfMaterialRateMasterReturnSomeMaterialRates()
//                .HavingMaterialMasterThatReturnsSomeControlBaseRate()
//                .SystemUnderTest();
//
//            var result = await systemUnderTest.Get("materialId", DateTime.Today);
//
//            result.Should().BeAssignableTo<OkNegotiatedContentResult<MaterialRateDto[]>>();
//        }

        [Fact]
        public async Task Gets_ShouldReturnBadRequest_WhenMaterialMasterThrowsArgumentException()
        {
            var systemUnderTest = new Fixture()
                .HavingMaterialRateMasterThatThrowArgumentException()
                .SystemUnderTest();

            var result = await systemUnderTest.Get("materialId", DateTime.Today);

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async Task LandedRateGet_ShouldReturnBadRequest_WhenRateMasterThrowsArgumentException()
        {
            var systemUnderTest = new Fixture()
                .HavingMaterialMasterThatThrowsArgumentExceptionForLandedRate()
                .SystemUnderTest();

            var result = await systemUnderTest.GetLandedRate("materialId", "location", "typeOfPurchase", DateTime.Today);

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async Task LandedRateGet_ShouldReturnStatusCodeOK_WhenRateMasterRuturnedLandedAndControlBaseRate()
        {
            var systemUnderTest = new Fixture()
                .HavingMaterialMasterThatReturnsSomeLandedRate()
                .HavingMaterialRepositoryThatReturnsMaterialContainsGeneralHeaderAndUnitOfMeasureColumn("materialId")
                .HavingMaterialMasterThatReturnsSomeControlBaseRate()
                .SystemUnderTest();

            var result = await systemUnderTest.GetLandedRate("materialId", "location", "typeOfPurchase", DateTime.Today);

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

            private readonly Mock<IMaterialRateService> _materialRateMaster = new Mock<IMaterialRateService>();
            private readonly Mock<IMasterDataRepository> _masterDataRepository = new Mock<IMasterDataRepository>();
            private readonly Mock<IMaterialRepository> _materialRepository = new Mock<IMaterialRepository>();

            public Fixture CreateOfMaterialRateMasterReturnSomeMaterialRate()
            {
                var materialRate = MaterialRateStubWithRateThatHasControlBaseRateAndLandedRate();
                _materialRateMaster.Setup(m => m.CreateRate(It.IsAny<IMaterialRate>()))
                    .ReturnsAsync(materialRate.Object);
                return this;
            }

            public Fixture CreateOfMaterialRateMasterThrowArgumentException()
            {
                _materialRateMaster.Setup(m => m.CreateRate(It.IsAny<IMaterialRate>()))
                    .Throws(new ArgumentException());
                return this;
            }

            public Fixture GetOfMaterialRateMasterReturnSomeHistroyOfMaterialRates()
            {
                SetUpMaterialRateMasterRateHistroyToReturn(MaterialRatesStubWithRatesThatHasControlBaseRateAndLandedRateForRateHistory());
                return this;
            }

            public Fixture GetOfMaterialRateMasterReturnSomeMaterialRate()
            {
                SetUpMaterialRateMasterToReturn(MaterialRateStubWithRateThatHasControlBaseRateAndLandedRate().Object);
                return this;
            }

            public Fixture GetOfMaterialRateMasterReturnSomeMaterialRates()
            {
                SetUpMaterialRateMasterToReturn(MaterialRateStubWithRatesThatHasControlBaseRateAndLandedRate());
                return this;
            }

            public Fixture HavingBankThatThrowsArgumentException()
            {
                _bank.Setup(b => b.ConvertTo(It.IsAny<Money>(), It.IsAny<string>())).Throws(new ArgumentException("Exchange rate is invalid"));
                return this;
            }


            public Fixture HavingMaterialRepositoryThatReturnsMaterial(string materialid)
            {
                var material = new Mock<IMaterial>();
                var headers = new Mock<IHeaderData>();
                var column = new Mock<IColumnData>();
                material.Setup(m => m.Headers).Returns(new List<IHeaderData> {headers.Object});
                headers.Setup(h => h.Columns).Returns(new List<IColumnData> {column.Object});
                _materialRepository.Setup(m => m.Find(materialid)).ReturnsAsync(material.Object);
                return this;
            }

            public Fixture HavingMaterialRepositoryThatReturnsMaterialContainsGeneralHeaderAndUnitOfMeasureColumn(string materialid)
            {
                var material = new Mock<IMaterial>();
                var headers = new Mock<IHeaderData>();
                var column = new Mock<IColumnData>();
                headers.Setup(h => h.Key).Returns("general");
                column.Setup(c => c.Key).Returns("unit_of_measure");
                column.Setup(c => c.Value).Returns("Load");
                material.Setup(m => m.Headers).Returns(new List<IHeaderData> { headers.Object });
                headers.Setup(h => h.Columns).Returns(new List<IColumnData> { column.Object });
                _materialRepository.Setup(m => m.Find(materialid)).ReturnsAsync(material.Object);
                return this;
            }

            public Fixture HavingMaterialMasterThatReturnsSomeAverageControlBaseRate()
            {
                _materialRateMaster.Setup(
                        m => m.GetAverageControlBaseRate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                            It.IsAny<string>()))
                    .ReturnsAsync(new Money(10m, "INR", new Mock<IBank>().Object));

                _bank.Setup(b => b.ConvertTo(new Money(10m, "INR", new Mock<IBank>().Object), "INR"))
                    .ReturnsAsync(new Money(10m, "INR", new Mock<IBank>().Object));
                return this;
            }

            public Fixture HavingMaterialMasterThatReturnsSomeAverageLandedRate()
            {
                _materialRateMaster.Setup(
                        m => m.GetAverageLandedRate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                            It.IsAny<string>()))
                    .ReturnsAsync(new Money(15m, "INR", new Mock<IBank>().Object));

                _bank.Setup(b => b.ConvertTo(new Money(15m, "INR", new Mock<IBank>().Object), "INR"))
                    .ReturnsAsync(new Money(15m, "INR", new Mock<IBank>().Object));
                return this;
            }

            public Fixture HavingMaterialMasterThatReturnsSomeControlBaseRate()
            {
                _materialRateMaster.Setup(
                        m => m.GetControlBaseRate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                            It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(new Money(10m, "INR", new Mock<IBank>().Object));

                _bank.Setup(b => b.ConvertTo(new Money(10m, "INR", new Mock<IBank>().Object), "INR"))
                    .ReturnsAsync(new Money(10m, "INR", new Mock<IBank>().Object));

                _bank.Setup(b => b.ConvertTo(new Money(10m, "INR", new Mock<IBank>().Object), "INR", DateTime.Today))
                   .ReturnsAsync(new Money(10m, "INR", new Mock<IBank>().Object));
                return this;
            }

            public Fixture HavingMaterialMasterThatReturnsSomeLandedRate()
            {
                _materialRateMaster.Setup(
                        m => m.GetLandedRate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                            It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(new Money(15m, "INR", new Mock<IBank>().Object));
                _bank.Setup(b => b.ConvertTo(new Money(15m, "INR", new Mock<IBank>().Object), "INR"))
                    .ReturnsAsync(new Money(15m, "INR", new Mock<IBank>().Object));

                return this;
            }

            public Fixture HavingMaterialMasterThatThrowsArgumentExceptionForAverageLandedRate()
            {
                _materialRateMaster.Setup(m => m.GetAverageControlBaseRate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                           It.IsAny<string>())).Throws(new ArgumentException());
                return this;
            }

            public Fixture HavingMaterialMasterThatThrowsArgumentExceptionForLandedRate()
            {
                _materialRateMaster.Setup(m => m.GetLandedRate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                            It.IsAny<string>(), It.IsAny<string>())).Throws(new ArgumentException());
                return this;
            }

            public Fixture HavingMaterialRateMasterThatThrowArgumentException()
            {
                SetupMaterialRateMasterToThrow(new ArgumentException("Location cannot be null."));
                return this;
            }

            public Fixture HavingMaterialRateMasterThatThrowResourceNotFoundException()
            {
                SetupMaterialRateMasterToThrow(new ResourceNotFoundException("MaterialRate"));
                return this;
            }

            public Fixture MaterialMasterAccepting(string location, string materialid, DateTime on,
                string typeOfPurchase)
            {
                _expectations.Add(
                    () =>
                        _materialRateMaster.Verify(
                            m => m.GetRate(materialid, location, @on, typeOfPurchase), Times.Once));
                return this;
            }

            public Fixture MaterialMasterAccepting(string materialid, DateTime on)
            {
                _expectations.Add(
                    () =>
                        _materialRateMaster.Verify(
                            m => m.GetRates(materialid, DateTime.Today), Times.Once));
                return this;
            }

            public MaterialRateController SystemUnderTest()
            {
                return new MaterialRateController(_materialRateMaster.Object, _bank.Object, _masterDataRepository.Object,_materialRepository.Object);
            }

            public void VerifyExpectations()
            {
                _expectations.ForEach(e => e.Invoke());
            }

            private static List<IMaterialRate> MaterialRatesStubWithRatesThatHasControlBaseRateAndLandedRateForRateHistory()
            {
                var materialRate = new Mock<IMaterialRate>();

                materialRate.Setup(r => r.ControlBaseRate).Returns(new Money(10m, "INR", new Mock<IBank>().Object));
                materialRate.Setup(r => r.LandedRate()).ReturnsAsync(new Money(15m, "INR", new Mock<IBank>().Object));

                var materialRates = new List<IMaterialRate> { materialRate.Object };
                return materialRates;
            }

            private static IEnumerable<IMaterialRate> MaterialRateStubWithRatesThatHasControlBaseRateAndLandedRate()
            {
                var materialRates = new List<IMaterialRate>();
                var materialRate = new Mock<IMaterialRate>();
                materialRate.Setup(r => r.ControlBaseRate).Returns(new Money(10m, "INR", new Mock<IBank>().Object));
                materialRate.Setup(r => r.LandedRate()).ReturnsAsync(new Money(15m, "INR", new Mock<IBank>().Object));
                materialRate.Setup(r => r.Id).Returns("materialId");
                materialRate.Setup(r => r.Location).Returns("location");
                materialRate.Setup(r => r.TypeOfPurchase).Returns("typeOfPurchase");
                materialRate.Setup(r => r.AppliedOn).Returns(DateTime.Today);
                materialRates.Add(materialRate.Object);
                return materialRates;
            }

            private static Mock<IMaterialRate> MaterialRateStubWithRateThatHasControlBaseRateAndLandedRate()
            {
                var materialRate = new Mock<IMaterialRate>();
                materialRate.Setup(r => r.ControlBaseRate).Returns(new Money(10m, "INR", new Mock<IBank>().Object));
                materialRate.Setup(r => r.LandedRate()).ReturnsAsync(new Money(15m, "INR", new Mock<IBank>().Object));
                return materialRate;
            }

            private void SetUpMaterialRateMasterRateHistroyToReturn(IEnumerable<IMaterialRate> materialRateDtos)
            {
                var materialRateSearchRequest = new MaterialRateSearchRequest();

                _materialRateMaster.Setup(
                        m =>
                            m.GetRateHistory(It.IsAny<string>(), materialRateSearchRequest))
                    .ReturnsAsync(new PaginatedAndSortedList<IMaterialRate>(materialRateDtos, 1, materialRateDtos.Count(), 20, "AppliedOn", SortOrder.Descending));
            }

            private void SetUpMaterialRateMasterToReturn(IMaterialRate materialRateDto)
            {
                _materialRateMaster.Setup(
                        m =>
                            m.GetRate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                                It.IsAny<string>()))
                    .ReturnsAsync(materialRateDto);
            }

            private void SetUpMaterialRateMasterToReturn(IEnumerable<IMaterialRate> materialRateDtos)
            {
                _materialRateMaster.Setup(
                        m =>
                            m.GetRates(It.IsAny<string>(), It.IsAny<DateTime>()))
                    .ReturnsAsync(materialRateDtos);
            }

            private void SetupMaterialRateMasterToThrow(Exception exception)
            {
                var materialRateSearchRequest = new MaterialRateSearchRequest();
                _materialRateMaster.Setup(
                        m =>
                            m.GetRate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                                It.IsAny<string>()))
                    .Throws(exception);

                _materialRateMaster.Setup(
                        m =>
                            m.GetRates(It.IsAny<string>(), It.IsAny<DateTime>()))
                    .Throws(exception);

                _materialRateMaster.Setup(
                       m =>
                           m.GetRateHistory(It.IsAny<string>(), materialRateSearchRequest))
                   .Throws(exception);
            }
        }
    }
}