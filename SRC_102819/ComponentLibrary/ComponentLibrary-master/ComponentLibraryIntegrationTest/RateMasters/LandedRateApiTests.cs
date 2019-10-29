using System;
using System.Net;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.RestWebClient;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.IntegrationTests.RateMasters
{
    public class LandedRateApiTest : IClassFixture<IntegrationTestFixture<LandedRateDto>>
    {
        public LandedRateApiTest(IntegrationTestFixture<LandedRateDto> fixture)
        {
            _client = fixture.Client;
        }

        private readonly IWebClient<LandedRateDto> _client;

        [Fact]
        public void GetLandedRate_WithValidMaterialIdWithMaterialIdAndLocationAndTypeOfPurchase_ShouldReturnOKStatusCodeWithRate()
        {
            var date = DateTime.UtcNow.ToString("o");
            var response =
                _client.Get($"/material-landed-rates?materialid=J001&location=Hyderabad&typeOfPurchase=IMPORT&onDate={date}").Result;

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.LandedRate.Value.Should().Be(58.08m);

        }

        [Fact]
        public void GetLandedRate_WithValidMaterialIdWithMaterialIdAndLocationAndDateAndTypeOfPurchase_ShouldReturnOKStatusCodeWithRate()
        {
            var date = DateTime.UtcNow.ToString("o");
            var response =
                _client.Get($"/material-landed-rates?materialid=J001&onDate={date}&location=Hyderabad&typeOfPurchase=IMPORT").Result;

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.LandedRate.Value.Should().Be(58.08m);

        }

        [Fact]
        public void GetLandedRate_WithValidMaterialIdWithMaterialIdAndLocationAndDateAndCurrencyAndTypeOfPurchase_ShouldReturnOKStatusCodeWithRate()
        {
            var date = DateTime.UtcNow.ToString("o");
            var response =
                _client.Get($"/material-landed-rates?materialid=J001&onDate={date}&location=Hyderabad&currency=INR&typeOfPurchase=IMPORT").Result;

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.LandedRate.Value.Should().Be(58.08m);

        }

        [Fact]
        public void GetAverageLandedRate_WithValidMaterialIdWithMaterialIdAndLocationAndDateAndCurrency_ShouldReturnOKStatusCodeWithRate()
        {
            var date = DateTime.UtcNow.ToString("o");
            var response =
                _client.Get($"/material-average-landed-rates?materialid=J001&onDate={date}&location=Hyderabad&currency=INR").Result;

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.LandedRate.Value.Should().Be(63.24m);
            response.Body.ControlBaseRate.Value.Should().Be(45m);
        }

        [Fact]
        public void GetAverageLandedRate_WithValidMaterialIdWithMaterialIdAndLocationAndCurrency_ShouldReturnOKStatusCodeWithRate()
        {
            var date = DateTime.UtcNow.ToString("o");
            var response =
                _client.Get($"/material-average-landed-rates?materialid=J001&onDate={date}&location=Hyderabad&currency=INR").Result;

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.LandedRate.Value.Should().Be(63.24m);
            response.Body.ControlBaseRate.Value.Should().Be(45m);

        }

        [Fact]
        public void GetAverageLandedRate_WithValidMaterialIdWithMaterialIdAndLocation_ShouldReturnOKStatusCodeWithRate()
        {
            var date = DateTime.UtcNow.ToString("o");
            var response =
                _client.Get($"/material-average-landed-rates?materialid=J001&onDate={date}&location=Hyderabad").Result;

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.LandedRate.Value.Should().Be(63.24m);
            response.Body.ControlBaseRate.Value.Should().Be(45m);
        }

    }
}
