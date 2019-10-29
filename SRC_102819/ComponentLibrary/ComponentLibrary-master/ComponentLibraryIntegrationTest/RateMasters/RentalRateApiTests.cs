using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.RestWebClient;
using TE.ComponentLibrary.TestWebClient;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.IntegrationTests.RateMasters
{
    public class RentalRateApiTests : IClassFixture<IntegrationTestFixture<RentalRateDto>>
    {
        private readonly IWebClient<RentalRateDto> _client;
        private readonly IntegrationTestFixture<RentalRateDto> _fixture;

        public RentalRateApiTests(IntegrationTestFixture<RentalRateDto> fixture)
        {
            _fixture = fixture;
            _client = fixture.Client;
        }

        [Fact]
        public async Task Get_With_Id_AppliedFrom_UnitOfMeasure_ShoudReturnRentalRate()
        {
            await SetupMaterial("MCH000099");

            var rentalrate = RequestBuilder<RentalRateDto>.BuildPostRequest("RentalRate.json");
            var rentalrateAppliedFrom = DateTime.UtcNow.AddDays(1);
            rentalrate.AppliedFrom = rentalrateAppliedFrom;
            await _client.Post(rentalrate, "materials/MCH000099/rental-rates");

            var response = _client.Get($"/materials/MCH000099/rental-rates?appliedFrom={rentalrateAppliedFrom.ToString("o")}&unitOfMeasure={rentalrate.UnitOfMeasure}").Result;
            response.Body.MaterialId.Should().Be("MCH000099");
        }

        [Fact]
        public async Task GetAll_With_Id_ShoudReturnRentalRate()
        {
            await SetupMaterial("MCH000100");

            var rentalrate = RequestBuilder<RentalRateDto>.BuildPostRequest("RentalRate.json");
            var rentalrateAppliedFrom = DateTime.UtcNow.AddDays(1);
            rentalrate.AppliedFrom = rentalrateAppliedFrom;
            await _client.Post(rentalrate, "materials/MCH000100/rental-rates");

            var response = await _client.GetPage("/materials/MCH000100/rental-rates/all");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.TotalRecords.Should().Be(1);
            response.Body.Items.First().MaterialId.Should().Be("MCH000100");
        }

        [Fact]
        public async Task GetLatest_With_Id_AppliedFrom_ShoudReturnRentalRate()
        {
            await SetupMaterial("MCH000101");

            var rentalrate = RequestBuilder<RentalRateDto>.BuildPostRequest("RentalRate.json");
            var rentalrateAppliedFrom = DateTime.UtcNow.AddDays(1);
            rentalrate.AppliedFrom = rentalrateAppliedFrom;
            await _client.Post(rentalrate, "materials/MCH000101/rental-rates");

            var response = _client.Gets($"/materials/MCH000101/rental-rates/active?appliedFrom={rentalrateAppliedFrom:o}").Result;

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.Count().Should().Be(1);
            response.Body.FirstOrDefault().MaterialId.Should().Be("MCH000101");
        }

        [Fact]
        public async Task Post_ShoudCreateRentalRate()
        {
            await SetupMaterial("MCH000102");

            var rentalrate = RequestBuilder<RentalRateDto>.BuildPostRequest("RentalRate.json");
            rentalrate.AppliedFrom = DateTime.UtcNow.AddDays(1);
            var response = _client.Post(rentalrate, "materials/MCH000102/rental-rates").Result;

            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        private async Task SetupMaterial(string materialCode)
        {
            _fixture.DropCollection("materials");
            var materialData = RequestBuilder<MaterialDataDto>.BuildPostRequest("Asset.json");
            foreach (var header in materialData.Headers)
            {
                foreach (var headerColumn in header.Columns)
                {
                    if (headerColumn.Key == "material_code")
                    {
                        headerColumn.Value = materialCode;
                    }
                }
            }

            var materialClient = new RestClient<MaterialDataDto>(IntegrationTestServer.GetInstance().GetClient());
            await materialClient.Post(materialData, "/materials");
        }
    }
}