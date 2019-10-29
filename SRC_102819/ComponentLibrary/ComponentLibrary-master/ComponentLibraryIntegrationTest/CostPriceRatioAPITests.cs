using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.RestWebClient;
using TE.ComponentLibrary.TestWebClient;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.IntegrationTests
{
	public class CostPriceRatioAPITests : IClassFixture<IntegrationTestFixture<CostPriceRatioDto>>
	{
		private readonly IWebClient<CostPriceRatioDto> _client;

		public CostPriceRatioAPITests(IntegrationTestFixture<CostPriceRatioDto> fixture)
		{
			_client = fixture.Client;
		}

		[Fact]
		public async Task CreateCostPriceRatio_ShouldCreateCostPriceRatio()
		{
			//_fixture.DropCollection("costPriceRatios");
			var costPriceRatioDto = RequestBuilder<CostPriceRatioDto>.BuildPostRequest("CostPriceRatioDto.json");
			costPriceRatioDto.AppliedFrom = DateTime.Today.AddDays(1);
			var response = await _client.Post(costPriceRatioDto, "/costpriceratios");
			response.StatusCode.Should().Be(HttpStatusCode.Created);
			response.Body.Should().NotBeNull();
		}

		[Fact]
		public async Task GetCostPriceRatio_ShouldReturnCorrectCostPriceRatio_Scenario1()
		{
			//var response = await _client.Get($"/costpriceratios?appliedOn=2017-06-30 10:00:00.000Z&componentType=Material&level1=Primary&level2=Aluminium and Copper&level3=Aluminium&code=ALM000001&projectCode=0053");
			var dateTimeString = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'");
			var response = await _client.Get($"/costpriceratios?appliedOn={dateTimeString}&componentType=Material&level1=Primary&level2=AluminiumAndCopper&level3=Aluminium&code=ALM000001&projectCode=0053");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Body.CprCoefficient.CPR.ShouldBeEquivalentTo(1.4);
		}

		[Fact]
		public async Task GetCostPriceRatio_ShouldReturnCorrectCostPriceRatio_Scenario2()
		{
			var dateTimeString = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'");
			var response = await _client.Get($"/costpriceratios?appliedOn={dateTimeString}&componentType=Material&level1=Primary&level2=AluminiumAndCopper&level3=Aluminium&code=ALM000001&projectCode=0054");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Body.CprCoefficient.CPR.ShouldBeEquivalentTo(1.3);
		}

		[Fact]
		public async Task GetCostPriceRatio_ShouldReturnCorrectCostPriceRatio_Scenario3()
		{
			var dateTimeString = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'");
			var response = await _client.Get($"/costpriceratios?appliedOn={dateTimeString}&componentType=Material&level1=Primary&level2=AluminiumAndCopper&level3=Aluminium&code=ALM000001");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Body.CprCoefficient.CPR.ShouldBeEquivalentTo(1.3);
		}

		[Fact]
		public async Task GetCostPriceRatio_ShouldReturnCorrectCostPriceRatio_Scenario4()
		{
			var dateTimeString = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'");
			var response = await _client.Get($"/costpriceratios?appliedOn={dateTimeString}&componentType=Material&level1=Primary&level2=AluminiumAndCopper&level3=Aluminium&code=ALM000002");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Body.CprCoefficient.CPR.ShouldBeEquivalentTo(1.2);
		}

		[Fact]
		public async Task GetCostPriceRatio_ShouldReturnCorrectCostPriceRatio_Scenario5()
		{
			var dateTimeString = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'");
			var response = await _client.Get($"/costpriceratios?appliedOn={dateTimeString}&componentType=Material&level1=Primary&level2=AluminiumAndCopper&level3=Copper&code=ALM000002");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Body.CprCoefficient.CPR.ShouldBeEquivalentTo(1.1);
		}

		[Fact]
		public async Task GetCostPriceRatio_ShouldReturnCorrectCostPriceRatio_Scenario6()
		{
			var dateTimeString = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'");
			var response = await _client.Get($"/costpriceratios?appliedOn={dateTimeString}&componentType=SFG&level1=FLOORING|DADO|PAVIOUR&level2=Flooring&level3=NaturalStone&code=FLR000001&projectCode=0053");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Body.CprCoefficient.CPR.ShouldBeEquivalentTo(1.4);
		}

		[Fact]
		public async Task GetCostPriceRatio_ShouldReturnCorrectCostPriceRatio_Scenario7()
		{
			var dateTimeString = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'");
			var response = await _client.Get($"/costpriceratios?appliedOn={dateTimeString}&componentType=SFG&level1=FLOORING|DADO|PAVIOUR&level2=Flooring&level3=NaturalStone&code=FLR000001&projectCode=0054");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Body.CprCoefficient.CPR.ShouldBeEquivalentTo(1.3);
		}

		[Fact]
		public async Task GetCostPriceRatio_ShouldReturnCorrectCostPriceRatio_Scenario8()
		{
			var dateTimeString = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'");
			var response = await _client.Get($"/costpriceratios?appliedOn={dateTimeString}&componentType=SFG&level1=FLOORING|DADO|PAVIOUR&level2=Flooring&level3=NaturalStone&code=FLR000001");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Body.CprCoefficient.CPR.ShouldBeEquivalentTo(1.3);
		}

		[Fact]
		public async Task GetCostPriceRatio_ShouldReturnCorrectCostPriceRatio_Scenario9()
		{
			var dateTimeString = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'");
			var response = await _client.Get($"/costpriceratios?appliedOn={dateTimeString}&componentType=SFG&level1=FLOORING|DADO|PAVIOUR&level2=Flooring&level3=NaturalStone&code=FLR000002");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Body.CprCoefficient.CPR.ShouldBeEquivalentTo(1.2);
		}

		[Fact]
		public async Task GetCostPriceRatio_ShouldReturnCorrectCostPriceRatio_Scenario10()
		{
			var dateTimeString = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'");
			var response = await _client.Get($"/costpriceratios?appliedOn={dateTimeString}&componentType=SFG&level1=FLOORING|DADO|PAVIOUR&level2=Flooring1&level3=NaturalStone1&code=FLR000002");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Body.CprCoefficient.CPR.ShouldBeEquivalentTo(1.1);
		}

		[Fact]
		public async Task GetCostPriceRatio_ShouldReturnCorrectCostPriceRatio_Scenario11()
		{
			var dateTimeString = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'");
			var response = await _client.Get($"/costpriceratios?appliedOn={dateTimeString}&componentType=Service&level1=FLOORING|DADO|PAVIOUR&level2=Flooring&level3=NaturalStone&code=FLR000001&projectCode=0053");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Body.CprCoefficient.CPR.ShouldBeEquivalentTo(1.4);
		}

		[Fact]
		public async Task GetCostPriceRatio_ShouldReturnCorrectCostPriceRatio_Scenario12()
		{
			var dateTimeString = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'");
			var response = await _client.Get($"/costpriceratios?appliedOn={dateTimeString}&componentType=Service&level1=FLOORING|DADO|PAVIOUR&level2=Flooring&level3=NaturalStone&code=FLR000001&projectCode=0054");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Body.CprCoefficient.CPR.ShouldBeEquivalentTo(1.3);
		}

		[Fact]
		public async Task GetCostPriceRatio_ShouldReturnCorrectCostPriceRatio_Scenario13()
		{
			var dateTimeString = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'");
			var response = await _client.Get($"/costpriceratios?appliedOn={dateTimeString}&componentType=Service&level1=FLOORING|DADO|PAVIOUR&level2=Flooring&level3=NaturalStone&code=FLR000001");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Body.CprCoefficient.CPR.ShouldBeEquivalentTo(1.3);
		}

		[Fact]
		public async Task GetCostPriceRatio_ShouldReturnCorrectCostPriceRatio_Scenario14()
		{
			var dateTimeString = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'");
			var response = await _client.Get($"/costpriceratios?appliedOn={dateTimeString}&componentType=Service&level1=FLOORING|DADO|PAVIOUR&level2=Flooring&level3=NaturalStone&code=FLR000002");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Body.CprCoefficient.CPR.ShouldBeEquivalentTo(1.2);
		}

		[Fact]
		public async Task GetCostPriceRatio_ShouldReturnCorrectCostPriceRatio_Scenario15()
		{
			var dateTimeString = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'");
			var response = await _client.Get($"/costpriceratios?appliedOn={dateTimeString}&componentType=Service&level1=FLOORING|DADO|PAVIOUR&level2=Flooring1&level3=NaturalStone1&code=FLR000002");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Body.CprCoefficient.CPR.ShouldBeEquivalentTo(1.1);
		}

		[Fact]
		public async Task GetCostPriceRatio_ShouldReturnCorrectCostPriceRatio_Scenario16()
		{
			var dateTimeString = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'");
			var response = await _client.Get($"/costpriceratios?appliedOn={dateTimeString}&componentType=Package&level1=HVAC&level2=VRV&level3=&code=PHV0001&projectCode=0053");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Body.CprCoefficient.CPR.ShouldBeEquivalentTo(1.4);
		}

		[Fact]
		public async Task GetCostPriceRatio_ShouldReturnCorrectCostPriceRatio_Scenario17()
		{
			var dateTimeString = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'");
			var response = await _client.Get($"/costpriceratios?appliedOn={dateTimeString}&componentType=Package&level1=HVAC&level2=VRV&level3=&code=PHV0001&projectCode=0054");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Body.CprCoefficient.CPR.ShouldBeEquivalentTo(1.3);
		}

		[Fact]
		public async Task GetCostPriceRatio_ShouldReturnCorrectCostPriceRatio_Scenario18()
		{
			var dateTimeString = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'");
			var response = await _client.Get($"/costpriceratios?appliedOn={dateTimeString}&componentType=Package&level1=HVAC&level2=VRV&level3=&code=PHV0001");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Body.CprCoefficient.CPR.ShouldBeEquivalentTo(1.3);
		}

		[Fact]
		public async Task GetCostPriceRatio_ShouldReturnCorrectCostPriceRatio_Scenario19()
		{
			var dateTimeString = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'");
			var response = await _client.Get($"/costpriceratios?appliedOn={dateTimeString}&componentType=Package&level1=HVAC&level2=VRV&level3=&code=PHV0002");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Body.CprCoefficient.CPR.ShouldBeEquivalentTo(1.2);
		}

		[Fact]
		public async Task GetCostPriceRatio_ShouldReturnCorrectCostPriceRatio_Scenario20()
		{
			var dateTimeString = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'");
			var response = await _client.Get($"/costpriceratios?appliedOn={dateTimeString}&componentType=Package&level1=HVAC&level2=VRV1&level3=&code=PHV0002");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Body.CprCoefficient.CPR.ShouldBeEquivalentTo(1.1);
		}
	}
}