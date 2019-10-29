using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using TE.ComponentLibrary.RestWebClient;
using TE.ComponentLibrary.TestWebClient;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.IntegrationTests
{
    public class ServiceApiTests : IClassFixture<IntegrationTestFixture<Dictionary<string, object>>>
    {
        private readonly IntegrationTestFixture<Dictionary<string, object>> _fixture;
        private readonly IWebClient<Dictionary<string, object>> _client;

        public ServiceApiTests(IntegrationTestFixture<Dictionary<string, object>> fixture)
        {
            _fixture = fixture;
            _client = fixture.Client;
        }

        [Fact]
        public async void Post_WithValidService_Returns201CreatedWithService()
        {
            _fixture.DropCollection("services");
            var serviceDefinition = RequestBuilder<Dictionary<string, object>>.BuildPostRequest("Service.json");

            var response = await _client.Post(serviceDefinition, "/services/old");
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Body.Should().NotBeNull();
        }

        [Fact]
        public async void Post_WithValidServiceWhichDoesNotHaveServiceCode_Returns201CreatedWithService()
        {
            var serviceDefinition = RequestBuilder<Dictionary<string, object>>.BuildPostRequest("ServiceWithoutCode.json");
            var response = await _client.Post(serviceDefinition, "/services/old");
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Body.Should().NotBeNull();
        }

        [Fact]
        public async void Post_WithValidPlanningData_ReturnsServiceWithSamePlanningData()
        {
            _fixture.DropCollection("services");
            var serviceDefinition = RequestBuilder<Dictionary<string, object>>.BuildPostRequest("Service.json");

            var response = await _client.Post(serviceDefinition, "/services/old");

            response.Body["id"].ShouldBeEquivalentTo("TST0001");
            response.Body["group"].ShouldBeEquivalentTo("TestService");
        }

        [Fact]
        public async void Search_ShouldFindValidClayServices_WhenRelevantSearchKeywordsArePassed()
        {
            _fixture.DropCollection("services");
            var service = RequestBuilder<Dictionary<string, object>>.BuildPostRequest("Service.json");
            await _client.Post(service, "/services/old");

            var response = await _client.Get("/services?serviceLevel1=TestService&searchKeyword=Clay Ser&pageNumber=1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            ((JArray)response.Body["items"]).Count.Should().Be(1);
        }

        [Fact]
        public async void Search_ShouldFindValidClayServices_WhenIdPassed()
        {
            _fixture.DropCollection("services");
            var service = RequestBuilder<Dictionary<string, object>>.BuildPostRequest("Service.json");
            await _client.Post(service, "/services/old");

            var response = await _client.Get("/services?serviceLevel1=TestService&searchKeyword=TST0001&pageNumber=1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            ((JArray)response.Body["items"]).Count.Should().Be(1);
        }

        [Fact]
        public async void Search_ShouldFindValidClayServices_WhenRelevantSearchPhrasesArePassedAsKeywords()
        {
            _fixture.DropCollection("services");
            var service = RequestBuilder<Dictionary<string, object>>.BuildPostRequest("AnotherService.json");
            await _client.Post(service, "/services/old");

            var response = await _client.Get("/services?serviceLevel1=TestService&searchKeyword=Cla Ser&pageNumber=1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            ((JArray)response.Body["items"]).Count.Should().Be(1);
        }

        [Fact]
        public async void Count_ShouldFindValidClayServices_WhenRelevantSearchPhrasesArePassedAsKeywords()
        {
            _fixture.DropCollection("services");
            var service = RequestBuilder<Dictionary<string, object>>.BuildPostRequest("AnotherService.json");
            await _client.Post(service, "/services/old");

            var response = await _client.Get("/services?serviceLevel1=TestService&searchKeyword=Cla Ser&pageNumber=1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = response.Body;

            ((long)result["recordCount"]).Should().Be(1);
            ((JArray)result["items"]).Count.Should().Be(1);
        }

        [Fact]
        public async void Count_ShouldAcceptRequest_WhenServiceLevelIsNotSent()
        {
            _fixture.DropCollection("services");
            var service = RequestBuilder<Dictionary<string, object>>.BuildPostRequest("AnotherService.json");
            await _client.Post(service, "/services/old");

            var response = await _client.Get("/services?searchKeyword=Cla Ser&pageNumber=1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = response.Body;

            ((long)result["recordCount"]).Should().Be(1);
            ((JArray)result["items"]).Count.Should().Be(1);
            ((long)result["batchSize"]).Should().Be(2);
            ((long)result["totalPages"]).Should().Be(1);
        }

        [Fact]
        public async void Search_ShouldBeCaseInsensitive_WhenServiceLevelIsPassed()
        {
            _fixture.DropCollection("services");
            var service = RequestBuilder<Dictionary<string, object>>.BuildPostRequest("AnotherService.json");
            await _client.Post(service, "/services/old");

            var response = await _client.Get("/services?serviceLevel1=testService&searchKeyword=Cla Ser&pageNumber=1&sortColumn=service Code&sortOrder=Ascending");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = response.Body;

            ((long)result["recordCount"]).Should().Be(1);
            ((JArray)result["items"]).Count.Should().Be(1);
        }

        [Fact]
        public async void GetByGroupAndColumnName_ShouldFindValidServices()
        {
            _fixture.DropCollection("services");
            var service = RequestBuilder<Dictionary<string, object>>.BuildPostRequest("Service.json");
            await _client.Post(service, "/services/old");

            var response = await _client.Get("/services?group=TestService&columnName=image&pageNumber=1&batchSize=10");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            ((JArray)response.Body["items"]).Count.Should().Be(1);
        }

        [Fact]
        public async void GetByGroupAndColumnNameAndKeyWord_ShouldFindValidServices()
        {
            _fixture.DropCollection("services");
            var service = RequestBuilder<Dictionary<string, object>>.BuildPostRequest("Service.json");
            await _client.Post(service, "/services/old");

            var response = await _client.Get("/services?group=TestService&columnName=image&keyWord=Test&pageNumber=1&batchSize=10");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            ((JArray)response.Body["items"]).Count.Should().Be(1);
        }
    }
}