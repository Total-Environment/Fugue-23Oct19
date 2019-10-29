using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.ComponentMasters.Infrastructure.Repository
{
    public class StaticFileRepositoryTests
    {
        [Fact]
        public async void GetById_ShouldForAValidStaticFileId_ReturnStaticFile()
        {
            var mockMongoRepository = new Mock<IMongoRepository<StaticFileDao>>();
            var staticFileId = "qweret";
            var expectedStaticFile = new StaticFile("qweret", "Static File");
            mockMongoRepository.Setup(r => r.FindBy("StaticFileId", staticFileId))
                .ReturnsAsync(new StaticFileDao {StaticFile = expectedStaticFile});
            var repository = new StaticFileRepository(mockMongoRepository.Object);

            var result = await repository.GetById(staticFileId);

            result.Name.Should().Be(expectedStaticFile.Name);
        }

        [Fact]
        public void GetById_ShouldForStaticFileIdThatDoesNotExist_ThrowNotFoundException()
        {
            var mockMongoRepository = new Mock<IMongoRepository<StaticFileDao>>();
            var staticFileId = "33gh-th4-yy6l";
            mockMongoRepository.Setup(r => r.GetById(staticFileId))
                .ReturnsAsync(null);

            var repository = new StaticFileRepository(mockMongoRepository.Object);

            Func<Task<StaticFile>> result = () => repository.GetById(staticFileId);

            result.ShouldThrow<ResourceNotFoundException>();
        }

        [Fact]
        public async void Add_ShouldForAValidStaticFile_ReturnUploadedStaticFile()
        {
            var mockMongoRepository = new Mock<IMongoRepository<StaticFileDao>>();
            var staticFile = new StaticFile("ssdswd", "Static File");
            var staticFileDao = new StaticFileDao(staticFile);
            mockMongoRepository.Setup(r => r.Add(It.IsAny<StaticFileDao>())).ReturnsAsync(staticFileDao);
            var staticFileRepository = new StaticFileRepository(mockMongoRepository.Object);

            var result = await staticFileRepository.Add(staticFile);

            result.Id.Should().NotBeNullOrEmpty();
            result.Name.Should().Be("Static File");
        }

        [Fact]
        public async void FindByName_ShouldForAStaticFileThatAlreadyExists_ReturnStaticFile()
        {
            var mockMongoRepository = new Mock<IMongoRepository<StaticFileDao>>();
            var staticFileName = "ssdswd";
            var staticFile = new StaticFile(staticFileName, "Static File");
            var staticFileDao = new StaticFileDao(staticFile);
            mockMongoRepository.Setup(r => r.FindBy(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(staticFileDao);
            var staticFileRepository = new StaticFileRepository(mockMongoRepository.Object);

            var result = await staticFileRepository.FindByName(staticFileName);

            result.Id.Should().NotBeNullOrEmpty();
            result.Name.Should().Be("Static File");
        }

        [Fact]
        public void FindByName_ShouldForANonExistentStaticFile_ReturnResourceNotFoundException()
        {
            var mockMongoRepository = new Mock<IMongoRepository<StaticFileDao>>();
            var staticFileName = "ssdswd";
            var staticFile = new StaticFile(staticFileName, "Static File");
            mockMongoRepository.Setup(r => r.FindBy(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(null);
            var staticFileRepository = new StaticFileRepository(mockMongoRepository.Object);


            Func<Task<StaticFile>> result = () => staticFileRepository.FindByName(staticFileName);

            result.ShouldThrow<ResourceNotFoundException>();
        }
    }
}