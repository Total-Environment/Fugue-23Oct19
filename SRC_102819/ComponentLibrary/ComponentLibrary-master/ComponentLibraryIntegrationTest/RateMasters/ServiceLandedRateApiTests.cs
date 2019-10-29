using System;
using System.Net;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.RestWebClient;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.IntegrationTests.RateMasters
{
    public class ServiceLandedRateApiTest : IClassFixture<IntegrationTestFixture<LandedRateDto>>
    {
        public ServiceLandedRateApiTest(IntegrationTestFixture<LandedRateDto> fixture)
        {
            _client = fixture.Client;
        }

        private readonly IWebClient<LandedRateDto> _client;


        [Fact]
        public async void GetServiceLandedRate_WithValidServiceIdWithServiceIdAndLocationAndDateAndTypeOfPurchase_ShouldReturnOKStatusCodeWithRate()
        {
            var date = DateTime.UtcNow.ToString("o");
            var response =
                await _client.Get($"/service-landed-rates?serviceid=J001&onDate={date}&location=Hyderabad&typeOfPurchase=IMPORT");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.LandedRate.Value.Should().Be(56m);

        }

        [Fact]
        public void GetServiceLandedRate_WithValidServiceIdWithServiceIdAndLocationAndDateAndCurrencyAndTypeOfPurchase_ShouldReturnOKStatusCodeWithRate()
        {
            var date = DateTime.UtcNow.ToString("o");
            var response =
                _client.Get($"/service-landed-rates?serviceid=J001&onDate={date}&location=Hyderabad&currency=INR&typeOfPurchase=IMPORT").Result;

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.LandedRate.Value.Should().Be(56m);

        }

        [Fact]
        public void GetServiceAverageLandedRate_WithValidServiceIdWithServiceIdAndLocationAndDateAndCurrency_ShouldReturnOKStatusCodeWithRate()
        {
            var date = DateTime.UtcNow.ToString("o");
            var response =
                _client.Get($"/service-average-landed-rates?serviceid=J001&onDate={date}&location=Hyderabad&currency=INR").Result;

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.LandedRate.Value.Should().Be(63m);
            response.Body.ControlBaseRate.Value.Should().Be(45m);
        }

    }
}
