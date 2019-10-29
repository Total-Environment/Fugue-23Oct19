using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
	/// <summary>
	/// </summary>
	public class ProjectDao : Entity
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ProjectDao"/> class.
		/// </summary>
		/// <param name="project">The project.</param>
		public ProjectDao(Project project)
		{
			ProjectCode = project.ProjectCode;
			ProjectName = project.ProjectName;
			ShortName = project.ShortName;
		}

		/// <summary>
		/// Gets or sets the project code.
		/// </summary>
		/// <value>The project code.</value>
		public string ProjectCode { get; set; }

		/// <summary>
		/// Gets or sets the name of the project.
		/// </summary>
		/// <value>The name of the project.</value>
		public string ProjectName { get; set; }

		/// <summary>
		/// Gets or sets the short name.
		/// </summary>
		/// <value>The short name.</value>
		public string ShortName { get; set; }

		/// <summary>
		/// To the project.
		/// </summary>
		/// <returns></returns>
		public Project ToProject()
		{
			var project = new Project(ProjectCode, ProjectName, ShortName);
			return project;
		}
	}
}