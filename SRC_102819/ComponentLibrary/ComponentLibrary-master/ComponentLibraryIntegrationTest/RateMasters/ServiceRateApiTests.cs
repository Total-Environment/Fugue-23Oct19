using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.RestWebClient;
using TE.ComponentLibrary.TestWebClient;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.IntegrationTests.RateMasters
{
    public class ServiceRateApiTests : IClassFixture<IntegrationTestFixture<ServiceRateDto>>
    {
        public ServiceRateApiTests(IntegrationTestFixture<ServiceRateDto> fixture)
        {
            _fixture = fixture;
            _client = fixture.Client;
        }

        private readonly IntegrationTestFixture<ServiceRateDto> _fixture;

        private readonly IWebClient<ServiceRateDto> _client;

        /// <summary>
        /// Gets the with valid service identifier should return ok status code with service rate.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Get_WithValidServiceId_ShouldReturnOkStatusCodeWithServiceRate()
        {
            _fixture.ResetDatabase();
            var serviceRate = RequestBuilder<ServiceRateDto>.BuildPostRequest("ServiceRate.json");

            await _client.Post(serviceRate, "/service-rates");
            var response =
                _client.Get(
                        "/service-rates?serviceid=MCLT0001&location=Hyderabad&on=2019-07-15T17:30:00.0000000Z&typeOfPurchase=Domestic Interstate")
                    .Result;

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.Id.Should().Be("MCLT0001");
        }

        [Fact]
        public void GetRateHistory_ShouldReturnOKStatusWithListOfServiceRates_WhenGivenValidServiceId()
        {
            const string serviceId = "FDP0001";
            var response = _client.GetPage($"/service-rates/all?serviceId={serviceId}").Result;

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            response.Body.Items.First().AppliedOn.ToLocalTime().Should().Be(DateTime.Today);
            response.Body.Items.Skip(1).First().AppliedOn.ToLocalTime().Should().Be(Convert.ToDateTime("2016-02-12"));
        }

        [Fact]
        public async Task GetRateHistory_ShouldReturnOKStatusWithListOfServiceRates_WhenGivenValidServiceId_FilterByTypeOfPurchase()
        {
            const string serviceId = "FDP0001";
            const string typeOfPurchase = "DOMESTIC INTER-STATE";
            var response = await _client.GetPage($"/service-rates/all?serviceId={serviceId}&typeOfPurchase={typeOfPurchase}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.Items.Count().Should().Be(1);
            response.Body.Items.First().TypeOfPurchase.Should().Be(typeOfPurchase);
            response.Body.Items.Last().TypeOfPurchase.Should().Be(typeOfPurchase);
        }

        [Fact]
        public async Task GetRateHistory_ShouldReturnOKStatusWithListOfServiceRates_WhenGivenValidServiceId_FilterByAppliedOn()
        {
            var appliedOn = new DateTime(2016, 02, 12);
            const string serviceId = "FDP0001";
            var response = await _client.GetPage($"/service-rates/all?serviceId={serviceId}&appliedOn={appliedOn.ToString("O")}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.Items.Count().Should().Be(1);
            response.Body.Items.First().AppliedOn.ToLocalTime().Should().Be(Convert.ToDateTime("2016-02-12"));
        }

        [Fact]
        public async Task GetRateHistory_ShouldReturnOKStatusWithListOfServiceRates_WhenGivenValidServiceId_FilterByLocation()
        {
            const string serviceId = "FDP0001";
            const string location = "Hyderabad";
            var response = await _client.GetPage($"/service-rates/all?serviceId={serviceId}&location={location}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.Items.Count().Should().Be(2);
            response.Body.Items.First().Location.Should().Be("Hyderabad");
        }

        [Fact]
        public void Gets_WithValidServiceId_ShouldReturnOKStatusCodeWithServiceRates()
        {
            const string serviceId = "FDP0001";
            var date = DateTime.UtcNow.ToString("o");
            var response = _client.Gets($"/service-rates/latest?serviceId={serviceId}&on={date}").Result;

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.First().Id.Should().Be("FDP0001");
            response.Body.First().TypeOfPurchase.Should().Be("DOMESTIC INTRA-STATE");
            response.Body.Skip(1).First().TypeOfPurchase.Should().Be("DOMESTIC INTER-STATE");
            response.Body.Skip(2).First().TypeOfPurchase.Should().Be("IMPORT");
        }

        [Fact]
        public async Task Gets_WithValidServiceId_ShouldReturnOkStatusCodeWithServiceRates()
        {
            _fixture.ResetDatabase();
            var serviceRate = RequestBuilder<ServiceRateDto>.BuildPostRequest("ServiceRate.json");
            await _client.Post(serviceRate, "/service-rates");

            var response =
                await _client.Gets(
                    "/service-rates/latest?serviceid=MCLT0001&on=2019-07-15T17:30:00.0000000Z");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.First().Id.Should().Be("MCLT0001");
        }

        [Fact]
        public void Post_WithValidServiceRates_onNextISTDay_ReturnsCreated()
        {
            _fixture.ResetDatabase(); ;
            var firstServiceRate = RequestBuilder<ServiceRateDto>.BuildPostRequest("ServiceRate.json");
            var secondServiceRate = RequestBuilder<ServiceRateDto>.BuildPostRequest("ServiceRate.json");

            secondServiceRate.AppliedOn = secondServiceRate.AppliedOn.AddHours(3);

            _client.Post(firstServiceRate, "/service-rates");
            var response = _client.Post(secondServiceRate, "/service-rates").Result;

            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task Post_WithValidServiceRates_onSameISTDay_ReturnsConflict()
        {
            _fixture.ResetDatabase();
            var firstServiceRate = RequestBuilder<ServiceRateDto>.BuildPostRequest("ServiceRate.json");
            var secondServiceRate = RequestBuilder<ServiceRateDto>.BuildPostRequest("ServiceRate.json");

            secondServiceRate.AppliedOn = secondServiceRate.AppliedOn.AddMinutes(30);

            await _client.Post(firstServiceRate, "/service-rates");
            var response = await _client.Post(secondServiceRate, "/service-rates");

            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public void Post_WithValidServiceRates_Returns201CreatedWithServiceRate()
        {
            _fixture.ResetDatabase();
            var serviceRate = RequestBuilder<ServiceRateDto>.BuildPostRequest("ServiceRate.json");

            var nextISTDay = $"{DateTime.UtcNow.AddDays(2).ToString("yyyy-MM-dd")}T03:00:00+5:30";
            var receivedDate = $"{DateTime.UtcNow.AddDays(2).ToString("yyyy-MM-dd")}T00:00:00+5:30";

            var serviceRateAppliedOn = DateTime.Parse(nextISTDay);
            var serviceRateAppliedOnRecieved = DateTime.Parse(receivedDate);
            serviceRate.AppliedOn = serviceRateAppliedOn;

            var response = _client.Post(serviceRate, "/service-rates").Result;

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Body.Id.Should().NotBeNullOrEmpty();
            response.Body.AppliedOn.Should().Be(TimeZoneInfo.ConvertTimeToUtc(serviceRateAppliedOnRecieved));
        }
    }
}