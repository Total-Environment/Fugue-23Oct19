using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ControllerPort;
using TE.ComponentLibrary.ComponentLibrary.DataAdaptors;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.ControllerPortTests
{
    public class ClassficationDefinitionControllerTests
    {
        [Fact]
        public async void Create_ShouldReturn201Created_WhenPassedADictionary()
        {
            var serviceClassificationDefinitionDto = new Dictionary<string, string>
            {
                {"FLOORING | DADO | PAVIOUR", "FLOORING | DADO | PAVIOUR - description"},
                {"Flooring", "Flooring - description"},
                {"Natural Stone", "Natural Stone - description"},
                {"Kota Blue", "Kota Blue - description"}
            };

            var fixture = new Fixture().WithStubbedValidServiceLevel().ShouldAdd();
            var classificationDefinitionDto = new Dictionary<string, Dictionary<string, string>>()
            {
                {
                    "service",
                    serviceClassificationDefinitionDto
                }
            };
            var result = await fixture.SystemUnderTest().Create(classificationDefinitionDto);
            result.Should().BeOfType<CreatedNegotiatedContentResult<Dictionary<string, Dictionary<string, string>>>>();
        }

        [Fact]
        public async void Create_ShouldReturn400BadRequest_WhenDictionaryWithMoreThan7PairsInputPassed()
        {
            var serviceClassificationDefinitionDto = new Dictionary<string, string>
            {
                {"FLOORING | DADO | PAVIOUR", "FLOORING | DADO | PAVIOUR - description"},
                {"Flooring", "Flooring - description"},
                {"Natural Stone", "Natural Stone - description"},
                {"Kota Blue", "Kota Blue - description"},
                {"Test1", "Test1 - description"},
                {"Test2", "Test2 - description"},
                {"Test3", "Test3 - description"},
                {"Test4", "Test4 - description"}
            };
            var classificationDefinitionDto = new Dictionary<string, Dictionary<string, string>>()
            {
                {
                    "service",
                    serviceClassificationDefinitionDto
                }
            };
            var sut = new Fixture().WithExceptionFromClassificationDefinitionBuilder("classificationDefinitionDto is containing more than 7 key/value pairs.").SystemUnderTest();

            var result = await sut.Create(classificationDefinitionDto);
            result.Should().BeOfType<BadRequestErrorMessageResult>();
            ((BadRequestErrorMessageResult)result).Message.Should()
                .Be("classificationDefinitionDto is containing more than 7 key/value pairs.");
        }

        [Fact]
        public async void Create_ShouldReturn400BadRequest_WhenEmptyDictionaryInputPassed()
        {
            var sut = new Fixture().WithExceptionFromClassificationDefinitionBuilder("classificationDefinitionDto is not containing any key/value pairs.").SystemUnderTest();
            var result = await sut.Create(new Dictionary<string, Dictionary<string, string>>()
            {
                {"service",new Dictionary<string, string> () }
            });
            result.Should().BeOfType<BadRequestErrorMessageResult>();
            ((BadRequestErrorMessageResult)result).Message.Should()
                .Be("classificationDefinitionDto is not containing any key/value pairs.");
        }

        [Fact]
        public async void Create_ShouldReturn400BadRequest_WhenEmptyInputPassed()
        {
            var sut = new Fixture().SystemUnderTest();
            var result = await sut.Create(new Dictionary<string, Dictionary<string, string>>());
            result.Should().BeOfType<BadRequestErrorMessageResult>();
            ((BadRequestErrorMessageResult)result).Message.Should()
                .Be("Component Type not specified for classification definition.");
        }

        [Fact]
        public async void Create_ShouldReturn400BadRequest_WhenNullInputPassed()
        {
            var sut = new Fixture().SystemUnderTest();
            var result = await sut.Create(null);
            result.Should().BeOfType<BadRequestErrorMessageResult>();
            ((BadRequestErrorMessageResult)result).Message.Should().Be("classificationDefinitionDto is null.");
        }

        [Fact]
        public async void Create_ShouldReturn400BadRequest_WhenRepositoryThrowsADuplicateResourceException()
        {
            var serviceClassificationDefinitionDto = new Dictionary<string, string>
            {
                {"Test1", "Test1 - description"},
                {"Test2", "Test2 - description"},
                {"Test3", "Test3 - description"},
                {"Test4", "Test4 - description"}
            };
            var classificationDefinitionDto = new Dictionary<string, Dictionary<string, string>>()
            {
                {
                    "service",
                    serviceClassificationDefinitionDto
                }
            };
            var fixture = new Fixture().WithStubbedValidServiceLevel().ShouldNotAddWithDuplicateResourceException();
            var sut = fixture.SystemUnderTest();
            var result = await sut.Create(classificationDefinitionDto);
            result.Should().BeOfType<ConflictResult>();
        }

        [Fact]
        public async void Create_ShouldReturn400BadRequest_WhenRepositoryThrowsArgumentException()
        {
            var serviceClassificationDefinitionDto = new Dictionary<string, string>
            {
                {"Test1", "Test1 - description"},
                {"Test2", "Test2 - description"},
                {"Test3", "Test3 - description"},
                {"Test4", "Test4 - description"}
            };
            var classificationDefinitionDto = new Dictionary<string, Dictionary<string, string>>()
            {
                {
                    "service",
                    serviceClassificationDefinitionDto
                }
            };
            var fixture = new Fixture().WithStubbedValidServiceLevel().ShouldNotAddWithArgumentException();
            var sut = fixture.SystemUnderTest();
            var result = await sut.Create(classificationDefinitionDto);
            result.Should().BeOfType<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async void Create_ShouldReturn400BadRequest_WhenServiceLevelIsNotFoundInMasterDataList()
        {
            var serviceClassificationDefinitionDto = new Dictionary<string, string>
            {
                {"Test1", "Test1 - description"},
                {"Test2", "Test2 - description"},
                {"Test3", "Test3 - description"},
                {"Test4", "Test4 - description"}
            };
            var classificationDefinitionDto = new Dictionary<string, Dictionary<string, string>>()
            {
                {
                    "service",
                    serviceClassificationDefinitionDto
                }
            };
            var sut = new Fixture().WithStubbedInvalidServiceLevel().SystemUnderTest();
            var result = await sut.Create(classificationDefinitionDto);
            result.Should().BeOfType<BadRequestErrorMessageResult>();
            ((BadRequestErrorMessageResult)result).Message.Should()
                .Be("Test1 is not found in master data list of service_level_1.");
        }

        private class Fixture
        {
            private readonly Mock<IClassificationDefinitionBuilder> _classificationDefinitionBuilder = new Mock<IClassificationDefinitionBuilder>();

            private readonly Mock<IClassificationDefinitionRepository>
                _mockServiceClassificationDefinitionRepository = new Mock<IClassificationDefinitionRepository>();

            public Fixture ShouldAdd()
            {
                _mockServiceClassificationDefinitionRepository.Setup(
                        m => m.CreateClassificationDefinition(It.IsAny<ClassificationDefinitionDao>()))
                    .Returns(Task.CompletedTask);
                return this;
            }

            public Fixture ShouldNotAddWithArgumentException()
            {
                _mockServiceClassificationDefinitionRepository.Setup(
                        m => m.CreateClassificationDefinition(It.IsAny<ClassificationDefinitionDao>()))
                    .Throws(new ArgumentException());
                return this;
            }

            public Fixture ShouldNotAddWithDuplicateResourceException()
            {
                _mockServiceClassificationDefinitionRepository.Setup(
                        m => m.CreateClassificationDefinition(It.IsAny<ClassificationDefinitionDao>()))
                    .Throws(new DuplicateResourceException("Duplicate"));
                return this;
            }

            public ClassficationDefinitionController SystemUnderTest()
            {
                return
                    new ClassficationDefinitionController(_mockServiceClassificationDefinitionRepository.Object,
                        _classificationDefinitionBuilder.Object);
            }

            public Fixture WithExceptionFromClassificationDefinitionBuilder(string error)
            {
                _classificationDefinitionBuilder.Setup(
                        m => m.BuildDao(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
                    .Throws(new ArgumentException(error));
                return this;
            }

            public Fixture WithStubbedInvalidServiceLevel()
            {
                _classificationDefinitionBuilder.Setup(
                        m => m.BuildDao(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
                    .Throws(new ArgumentException("Test1 is not found in master data list of service_level_1."));

                return this;
            }

            public Fixture WithStubbedValidServiceLevel()
            {
                return this;
            }
        }
    }
}