using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Driver;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.RepositoryPortTests
{
	public class ProjectRepositoryTests
	{
		[Fact]
		public async Task Find_ShouldReturnProject_WhenValidProjectCodeIsPassed()
		{
			var mockCursor = new Mock<IAsyncCursor<ProjectDao>>();
			var enumerator = new List<ProjectDao> { new ProjectDao(new Project("0053", "The Magic Faraway Tree", "TMFT")) }.GetEnumerator();
			mockCursor.Setup(m => m.Current).Returns(() => new List<ProjectDao> { enumerator.Current });
			mockCursor.Setup(m => m.MoveNext(default(CancellationToken))).Returns(() => enumerator.MoveNext());
			Mock<IMongoCollection<ProjectDao>> mockMongoCollection = new Mock<IMongoCollection<ProjectDao>>();
			mockMongoCollection.Setup(
				m =>
					m.FindAsync(It.IsAny<FilterDefinition<ProjectDao>>(), It.Is((FindOptions<ProjectDao, ProjectDao> f) => true),
						default(CancellationToken))).ReturnsAsync(mockCursor.Object);
			ProjectRepository projectRepository = new ProjectRepository(mockMongoCollection.Object);

			var result = await projectRepository.Find("0053");

			result.ProjectCode.ShouldBeEquivalentTo("0053");
		}

		[Fact]
		public void Find_ShouldThrowResourceNotFoundException_WhenInvalidValidProjectCodeIsPassed()
		{
			var mockCursor = new Mock<IAsyncCursor<ProjectDao>>();
			var enumerator = new List<ProjectDao> { }.GetEnumerator();
			mockCursor.Setup(m => m.Current).Returns(() => new List<ProjectDao> { enumerator.Current });
			mockCursor.Setup(m => m.MoveNext(default(CancellationToken))).Returns(() => enumerator.MoveNext());
			Mock<IMongoCollection<ProjectDao>> mockMongoCollection = new Mock<IMongoCollection<ProjectDao>>();
			mockMongoCollection.Setup(
				m =>
					m.FindAsync(It.IsAny<FilterDefinition<ProjectDao>>(), It.Is((FindOptions<ProjectDao, ProjectDao> f) => true),
						default(CancellationToken))).ReturnsAsync(mockCursor.Object);
			ProjectRepository projectRepository = new ProjectRepository(mockMongoCollection.Object);

			Func<Task> func = async () => await projectRepository.Find("0053");

			func.ShouldThrow<ResourceNotFoundException>().WithMessage("Project 0053 not found.");
		}

		[Fact]
		public async Task Add_ShouldAdd_WhenValidProjectIsPassed()
		{
			Mock<IMongoCollection<ProjectDao>> mockMongoCollection = new Mock<IMongoCollection<ProjectDao>>();
			mockMongoCollection.Setup(m => m.InsertOneAsync(It.IsAny<ProjectDao>(), null, default(CancellationToken))).Returns(Task.CompletedTask);
			ProjectRepository projectRepository = new ProjectRepository(mockMongoCollection.Object);
			Project project = new Project("0053", "The Magic Faraway Tree", "TMFT");

			var result = await projectRepository.Add(project);

			result.ProjectCode.ShouldBeEquivalentTo("0053");
		}

		[Fact]
		public void Add_ShouldThrowDuplicateResourceException_WhenExistingProjectIsPassed()
		{
			Mock<IMongoCollection<ProjectDao>> mockMongoCollection = new Mock<IMongoCollection<ProjectDao>>();
			mockMongoCollection.Setup(m => m.CountAsync(It.IsAny<FilterDefinition<ProjectDao>>(), null, default(CancellationToken))).ReturnsAsync(1);
			ProjectRepository projectRepository = new ProjectRepository(mockMongoCollection.Object);
			Project project = new Project("0053", "The Magic Faraway Tree", "TMFT");

			Func<Task> func = async () => await projectRepository.Add(project);

			func.ShouldThrow<DuplicateResourceException>()
				.WithMessage("Project for this combination 0053, The Magic Faraway Tree, TMFT already exists. Please revisit and submit.");
		}

        // TODO: Figure out how to write this test.
//	    [Fact]
//	    public async Task List_ShouldReturnListOfProjects()
//	    {
//	        var mockMongoCollection = new Mock<IMongoCollection<ProjectDao>>();
//            TestHelper.MockCollectionToReturnAllListItems(mockMongoCollection, new List<ProjectDao>{new ProjectDao(new Project("0053", "Pursuit of a Radical Rhapsody", "POARR"))});
//	        var projectRepository = new ProjectRepository(mockMongoCollection.Object);
//	        var projects = await projectRepository.List();
//	        projects.Should().HaveCount(1);
//            projects.First().ProjectCode.Should().Be("0053");
//	    }

		// TODO: Fix below test to work.

		//[Fact]
		//public void Add_ShouldThrowDuplicateResourceException_WhenExistingProjectCodeIsPassed()
		//{
		//	var builder = Builders<ProjectDao>.Filter;
		//	var filters = new List<FilterDefinition<ProjectDao>>
		//	{
		//		builder.Eq(dao => dao.ProjectCode, "0053"),
		//		builder.Eq(dao => dao.ProjectName, "The Magic Faraway Tree"),
		//		builder.Eq(dao => dao.ShortName, "TMFT")
		//	};
		//	var filterDefinition1 = builder.And(filters);
		//	Mock<IMongoCollection<ProjectDao>> mockMongoCollection = new Mock<IMongoCollection<ProjectDao>>();
		//	mockMongoCollection.Setup(m => m.CountAsync(filterDefinition1, null, default(CancellationToken))).ReturnsAsync(0);
		//	filters = new List<FilterDefinition<ProjectDao>>
		//	{
		//		builder.Eq(dao => dao.ProjectCode, "0053")
		//	};
		//	var filterDefinition2 = builder.And(filters);
		//	mockMongoCollection.Setup(m => m.CountAsync(filterDefinition2, null, default(CancellationToken))).ReturnsAsync(1);
		//	ProjectRepository projectRepository = new ProjectRepository(mockMongoCollection.Object);
		//	Project project = new Project("0053", "The Magic Faraway Tree", "TMFT");

		// Func<Task> func = async () => await projectRepository.Add(project);

		//	func.ShouldThrow<DuplicateResourceException>()
		//		.WithMessage("Project for this code 0053 already exists. Please revisit and submit.");
		//}
	}
}