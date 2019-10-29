using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DomainTests
{
    public class CheckListTests
    {
        [Fact]
        public void NameIdGetDataTemplate_Should_ReturnNameIdAndGetDataTemplate()
        {
            const string name = "Feneration";
            const string id = "MFNQ0001";
            var checklistId = Guid.NewGuid().ToString();
            const string template = "testTemplate";

            var entries = new List<Entry>();

            var tableMock = new Mock<Table>();
            tableMock.Setup(t => t.Content()).Returns(entries);
            var checkList = new CheckList
            {
                Title = name,
                Id = id,
                CheckListId = checklistId,
                Content = tableMock.Object,
                Template = template
            };

            checkList.Title.Should().Be(name);
            checkList.Id.Should().Be(id);
            checkList.GetData().Should().Equal(entries);
            checkList.Template.Should().Be(template);
            checkList.ToString().Should().Be(checklistId);
        }
    }
}