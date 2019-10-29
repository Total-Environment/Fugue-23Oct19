using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.Shared.CloudServiceFramework.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.ComponentMasters.Infrastructure.Repository
{
    public class DocumentRepositoryTests
    {
        [Fact]
        public async void Upload_ShouldCallBlobStorageWithFileNameAndStream_WhenPassedWithSame()
        {
            var stream = new Mock<Stream>();
            const string fileName = "fileName";
            var fixture = new Fixture().ShouldCallBlobStorageWith(fileName, stream.Object);
            await fixture.Sut().Upload(fileName, stream.Object);
            fixture.VerifyExpectations();
        }

        [Fact]
        public async void Upload_ShouldStaticFileRepoWithFileName_WhenBlobStorageStoreTheFiles()
        {
            var stream = new Mock<Stream>();
            const string fileName = "fileName";
            var fixture = new Fixture().ShouldCallStaticFileRepositoryWithStaticFileHavingName(fileName);

            await fixture.Sut().Upload(fileName, stream.Object);

            fixture.VerifyExpectations();
        }

        [Fact]
        public void DocumentRepository_ShouldImplementIDocumentRepo()
        {
            var documentRepository = new DocumentRepository(new Mock<IBlobStorageService>().Object, new Mock<IStaticFileRepository>().Object);

            documentRepository.Should().BeAssignableTo<IDocumentRepository>();
        }

        class Fixture
        {
            private readonly Mock<IBlobStorageService> _blobStorageMock = new Mock<IBlobStorageService>();
            private readonly List<Action> _expectations = new List<Action>();
            private readonly Mock<IStaticFileRepository> _staticFileRepository = new Mock<IStaticFileRepository>();
            public DocumentRepository Sut()
            {
                return new DocumentRepository(_blobStorageMock.Object,_staticFileRepository.Object);
            }

            public Fixture ShouldCallBlobStorageWith(string filename, Stream streamObject)
            {
                _expectations.Add(()=>_blobStorageMock.Verify(b=>b.Upload(filename,streamObject,It.IsAny<string>()),Times.Once));
                return this;
            }

            public void VerifyExpectations()
            {
                _expectations.ForEach(e=>e.Invoke());
            }

            public Fixture ShouldCallStaticFileRepositoryWithStaticFileHavingName(string fileName)
            {
                _expectations.Add( () => _staticFileRepository.Verify(s => s.Add(It.Is<StaticFile>(f=>f.Name == fileName)),Times.Once));
                return this;
            }
        }
    }
}
