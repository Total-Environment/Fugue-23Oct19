using System;
using System.Collections.Generic;
using System.Net;
using FluentAssertions;
using Newtonsoft.Json;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.RestWebClient;
using TE.ComponentLibrary.TestWebClient;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.IntegrationTests
{
    public class CheckListApiTests : IClassFixture<IntegrationTestFixture<CheckList>>
    {
        public CheckListApiTests(IntegrationTestFixture<CheckList> fixture)
        {
            _client = fixture.Client;
        }

        private readonly IWebClient<CheckList> _client;

        [Theory]
        [InlineData("title", "Checklist title is mandatory.")]
        [InlineData("id", "Checklist id is mandatory.")]
        [InlineData("template", "Template name is mandatory.")]
        public async void Post_WithInValidCheckListHavingNullValue_Returns400BadRequest(
            string fieldWhichIsDestinedToBeNonExistential, string message)
        {
            try
            {
                var checklist = RequestBuilder<CheckList>.BuildPostRequest("CreateCheckList.json", fieldWhichIsDestinedToBeNonExistential);
                await _client.Post(checklist, "/check-lists");
            }
            catch (Exception ex)
            {
                var errorResponse = ex.Message;
                var mainObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(errorResponse);

                mainObject["message"].Should().Be("The request is invalid.");
                mainObject["modelState"].ToString().Should().Contain(message);
            }
        }

        [Theory]
        [InlineData("title", "Checklist title is mandatory.")]
        [InlineData("checkListId", "Checklist id is mandatory.")]
        [InlineData("template", "Template name is mandatory.")]
        public async void Post_WithInValidCheckListHavingEmptyValues_Returns400BadRequest(
            string fieldWhichIsDestinedToBeNonExistential, string message)
        {
            try
            {
                var checklist = RequestBuilder<CheckList>.BuildPostRequest("CreateCheckList.json", fieldWhichIsDestinedToBeNonExistential);
                await _client.Post(checklist, "/check-lists");
            }
            catch (Exception ex)
            {
                var errorResponse = ex.Message;
                var mainObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(errorResponse);

                mainObject["message"].Should().Be("The request is invalid.");
                mainObject["modelState"].ToString().Should().Contain(message);
            }
        }

        [Fact]
        public async void Get_WithValidCheckListId_Returns200CheckList()
        {
            var checklist = RequestBuilder<CheckList>.BuildPostRequest("CreateCheckList.json");
            checklist.CheckListId = new Random().Next().ToString();

            await _client.Post(checklist, "/check-lists");
            var response = await _client.Get($"/check-lists/{checklist.CheckListId}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.CheckListId.Should().Be(checklist.CheckListId);
        }

        [Fact]
        public async void Post_WithValidCheckList_Returns201CreatedWithCheckList()
        {
            var checklist = RequestBuilder<CheckList>.BuildPostRequest("CreateCheckList.json");
            checklist.CheckListId = new Random().Next().ToString();
            var response = await _client.Post(checklist, "/check-lists");
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Body.Id.Should().NotBeNull();
        }
    }
}