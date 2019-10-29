using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MongoDB.Driver;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
	/// <summary>
	/// </summary>
	/// <seealso cref="TE.ComponentLibrary.ComponentLibrary.RepositoryPort.IProjectRepository"/>
	public class ProjectRepository : IProjectRepository
	{
		private readonly IMongoCollection<ProjectDao> _mongoCollection;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProjectRepository"/> class.
		/// </summary>
		/// <param name="mongoCollection">The mongo collection.</param>
		public ProjectRepository(IMongoCollection<ProjectDao> mongoCollection)
		{
			_mongoCollection = mongoCollection;
		}

		/// <summary>
		/// Finds the specified project code.
		/// </summary>
		/// <param name="projectCode">The project code.</param>
		/// <returns></returns>
		public async Task<Project> Find(string projectCode)
		{
			var project = (await _mongoCollection.FindAsync(p => p.ProjectCode == projectCode)).FirstOrDefault();
			if (project == null)
			{
				throw new ResourceNotFoundException($"Project {projectCode}");
			}
			return project.ToProject();
		}

		/// <summary>
		/// Adds the specified project.
		/// </summary>
		/// <param name="project">The project.</param>
		/// <returns></returns>
		public async Task<Project> Add(Project project)
		{
			var projectDao = new ProjectDao(project);
			await CheckProjectDuplicacy(projectDao);
			await CheckProjectCodeDuplicacy(projectDao);
			await _mongoCollection.InsertOneAsync(projectDao);
			return projectDao.ToProject();
		}

		private async Task CheckProjectDuplicacy(ProjectDao projectDao)
		{
			var builder = Builders<ProjectDao>.Filter;
			var filters = new List<FilterDefinition<ProjectDao>>
			{
				builder.Eq(dao => dao.ProjectCode, projectDao.ProjectCode),
				builder.Eq(dao => dao.ProjectName, projectDao.ProjectName),
				builder.Eq(dao => dao.ShortName, projectDao.ShortName)
			};
			var filterDefinition = builder.And(filters);
			var existingCount = await _mongoCollection.CountAsync(filterDefinition);
			if (existingCount > 0)
			{
				var combination = new StringBuilder();
				combination.Append(projectDao.ProjectCode + ", ");
				combination.Append(projectDao.ProjectName + ", ");
				combination.Append(projectDao.ShortName);

				throw new DuplicateResourceException(
					$"Project for this combination {combination} already exists. Please revisit and submit.");
			}
		}

		private async Task CheckProjectCodeDuplicacy(ProjectDao projectDao)
		{
			var builder = Builders<ProjectDao>.Filter;
			var filters = new List<FilterDefinition<ProjectDao>>
			{
				builder.Eq(dao => dao.ProjectCode, projectDao.ProjectCode)
			};
			var filterDefinition = builder.And(filters);
			var existingCount = await _mongoCollection.CountAsync(filterDefinition);
			if (existingCount > 0)
			{
				throw new DuplicateResourceException(
					$"Project for this code {projectDao.ProjectCode} already exists. Please revisit and submit.");
			}
		}

	    public async Task<IEnumerable<Project>> List()
	    {
	        return (await _mongoCollection.AsQueryable().ToListAsync()).Select(p => p.ToProject());
	    }
	}
}