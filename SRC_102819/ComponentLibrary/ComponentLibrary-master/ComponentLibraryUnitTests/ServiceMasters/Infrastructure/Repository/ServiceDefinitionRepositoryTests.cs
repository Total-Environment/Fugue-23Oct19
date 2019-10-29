using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Driver;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository.Dao;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.ServiceMasters.Infrastructure.Repository
{
	public class ServiceDefinitionRepositoryTest
	{
		private class Fixture
		{
			private readonly Mock<IDataTypeFactory> _factoryMock = new Mock<IDataTypeFactory>();

			private readonly Mock<IMongoCollection<ServiceDefinitionDao>> _mockCollection =
				new Mock<IMongoCollection<ServiceDefinitionDao>>();

			private readonly List<Action> _verifications = new List<Action>();

			public Fixture Accepting(ServiceDefinition serviceDefinition)
			{
				_verifications.Add(
					() =>
						_mockCollection.Verify(
							m =>
								m.InsertOneAsync(It.IsAny<ServiceDefinitionDao>(), It.IsAny<InsertOneOptions>(),
									It.IsAny<CancellationToken>()), Times.Once()));
				return this;
			}

			public ServiceDefinitionRepository SystemUnderTest()
			{
				return new ServiceDefinitionRepository(_mockCollection.Object, _factoryMock.Object,
					new Mock<IDependencyDefinitionRepository>().Object, new Mock<ICodePrefixTypeMappingRepository>().Object);
			}

			public void VerifyExpectations()
			{
				_verifications.ForEach(v => v.Invoke());
			}

			public Fixture WithExisting(IServiceDefinition serviceDefinition)
			{
				TestHelper.MockCollectionWithExisting(_mockCollection, new ServiceDefinitionDao(serviceDefinition));
				return this;
			}

			public Fixture WithEmpty()
			{
				TestHelper.MockCollectionWithExisting(_mockCollection, null);
				return this;
			}

			public Mock<IMongoCollection<ServiceDefinitionDao>> MongoCollection()
			{
				return _mockCollection;
			}
		}

		[Fact]
		public async void Add_ShouldSave_WhenPassedServiceDefinition()
		{
			var serviceDefinition = new ServiceDefinition("sattar");
			var fixture = new Fixture().WithEmpty().Accepting(serviceDefinition);
			await fixture.SystemUnderTest().Add(serviceDefinition);
			fixture.VerifyExpectations();
		}

		[Fact]
		public void Add_ShouldThrowDuplicateResourceException_WhenPassedExistingCode()
		{
			var serviceDefinition = new ServiceDefinition("sattar") { Code = "SAT" };
			var sut = new Fixture().WithExisting(new ServiceDefinition("abdul") { Code = "SAT" }).SystemUnderTest();
			Func<Task> act = async () => await sut.Add(serviceDefinition);
			act.ShouldThrow<DuplicateResourceException>().WithMessage("Code: SAT already exists.");
		}

		[Fact]
		public void Add_ShouldThrowDuplicateResourceException_WhenPassedExistingName()
		{
			var serviceDefinition = new ServiceDefinition("sattar");
			var sut = new Fixture().WithExisting(serviceDefinition).SystemUnderTest();
			Func<Task> act = async () => await sut.Add(serviceDefinition);
			act.ShouldThrow<DuplicateResourceException>().WithMessage("Name: sattar already exists.");
		}

		[Fact]
		public async void Find_ShouldReturnServiceDefinition_WhenPassedExistingGroup()
		{
			var serviceDefinition = new ServiceDefinition("sattar");
			var sut = new Fixture().WithExisting(serviceDefinition).SystemUnderTest();
			(await sut.Find("sattar")).Name.Should().Be("sattar");
		}

		[Fact]
		public void Find_ShouldThrowResourceNotFoundException_WhenPassedNonExistingGroup()
		{
			var sut = new Fixture().WithEmpty().SystemUnderTest();
			Func<Task> act = async () => await sut.Find("sattar");
			act.ShouldThrow<ResourceNotFoundException>();
		}

		[Fact]
		public async void Update_ShouldQueryMongoCollection_WhenPassedServiceDefinition()
		{
			var serviceDefinition = new ServiceDefinition("sattar");
			var fixture = new Fixture();

			await fixture.SystemUnderTest().Update(serviceDefinition);

			fixture.MongoCollection()
				.Verify(m => m.ReplaceOneAsync(
					It.IsAny<FilterDefinition<ServiceDefinitionDao>>(),
					It.Is<ServiceDefinitionDao>(n => n.Name == "sattar"),
					null,
					default(CancellationToken)), Times.Once);
		}
	}
}