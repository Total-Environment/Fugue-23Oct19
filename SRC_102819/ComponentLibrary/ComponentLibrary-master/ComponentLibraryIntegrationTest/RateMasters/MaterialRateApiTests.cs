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
    public class MaterialRateApiTests : IClassFixture<IntegrationTestFixture<MaterialRateDto>>
    {
        private readonly IWebClient<MaterialRateDto> _client;

        public MaterialRateApiTests(IntegrationTestFixture<MaterialRateDto> fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public void Get_WithValidMaterialId_ShouldReturnOkStatusCodeWithMaterialRate()
        {
            var date = DateTime.UtcNow.ToString("o");
            var response =
                _client.Get(
                        $"/material-rates?materialid=J001&location=Hyderabad&typeOfPurchase=IMPORT&on={date}").Result;

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.Id.Should().Be("J001");
        }

        [Fact]
        public async Task GetRateHistory_ShouldReturnOKStatusWithListOfMaterialRates_WhenGivenValidMaterialId()
        {
            const string materialId = "J002";
            var response = await _client.GetPage($"/material-rates/all?materialId={materialId}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            response.Body.Items.First().AppliedOn.ToLocalTime().Should().Be(Convert.ToDateTime("2016-12-27"));
            response.Body.Items.Skip(1).First().AppliedOn.ToLocalTime().Should().Be(Convert.ToDateTime("2016-12-25"));
        }

        [Fact]
        public async Task GetRateHistory_ShouldReturnOKStatusWithListOfMaterialRates_WhenGivenValidMaterialId_FilterByAppliedOn()
        {
            var appliedOn = new DateTime(2016, 12, 27);
            const string materialId = "J002";
            var response = await _client.GetPage($"/material-rates/all?materialId={materialId}&appliedOn={appliedOn.ToString("O")}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.Items.Count().Should().Be(2);
            response.Body.Items.First().AppliedOn.ToLocalTime().Should().Be(Convert.ToDateTime("2016-12-27"));
        }

        [Fact]
        public async Task GetRateHistory_ShouldReturnOKStatusWithListOfMaterialRates_WhenGivenValidMaterialId_FilterByLocation()
        {
            const string materialId = "J002";
            const string location = "Hyderabad";
            var response = await _client.GetPage($"/material-rates/all?materialId={materialId}&location={location}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.Items.Count().Should().Be(1);
            response.Body.Items.First().Location.Should().Be("Hyderabad");
        }

        [Fact]
        public async Task GetRateHistory_ShouldReturnOKStatusWithListOfMaterialRates_WhenGivenValidMaterialId_FilterByTypeOfPurchase()
        {
            const string materialId = "J002";
            const string typeOfPurchase = "DOMESTIC INTER-STATE";
            var response = await _client.GetPage($"/material-rates/all?materialId={materialId}&typeOfPurchase={typeOfPurchase}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.Items.Count().Should().Be(2);
            response.Body.Items.First().TypeOfPurchase.Should().Be(typeOfPurchase);
            response.Body.Items.Last().TypeOfPurchase.Should().Be(typeOfPurchase);
        }

        [Fact]
        public void Gets_WithValidMaterialId_ShouldReturnOkStatusCodeWithMaterialRates()
        {
            var date = DateTime.UtcNow.ToString("o");
            var response =
                _client.Gets(
                        $"/material-rates/latest?materialid=J001&on={date}").Result;

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.First().Id.Should().Be("J001");
        }

        [Fact]
        public async void Post_WithValidMaterialRates_Returns201CreatedWithMaterialRate()
        {
            var materialRate = RequestBuilder<MaterialRateDto>.BuildPostRequest("MaterialRate.json");

            var nextISTDay = $"{DateTime.UtcNow.AddDays(2).ToString("yyyy-MM-dd")}T03:00:00+5:30";
            var receivedDate = $"{DateTime.UtcNow.AddDays(2).ToString("yyyy-MM-dd")}T00:00:00+5:30";

            var materialRateAppliedOn = DateTime.Parse(nextISTDay);
            var materialRateAppliedOnRecieved = DateTime.Parse(receivedDate);

            materialRate.AppliedOn = materialRateAppliedOn;

            var response = await _client.Post(materialRate, "/material-rates");

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Body.Id.Should().NotBeNullOrEmpty();
            response.Body.AppliedOn.Should().Be(TimeZoneInfo.ConvertTimeToUtc(materialRateAppliedOnRecieved));
        }
    }
}