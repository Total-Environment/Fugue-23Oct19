using System.ComponentModel.DataAnnotations;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs
{
	/// <summary>
	/// </summary>
	public class ProjectDto
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ProjectDto"/> class.
		/// </summary>
		public ProjectDto()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ProjectDto"/> class.
		/// </summary>
		/// <param name="project">The project.</param>
		public ProjectDto(Project project)
		{
			ProjectCode = project.ProjectCode;
			ProjectName = project.ProjectName;
			ShortName = project.ShortName;
		}

		/// <summary>
		/// Gets or sets the project code.
		/// </summary>
		/// <value>The project code.</value>
		[Required(AllowEmptyStrings = false)]
		public string ProjectCode { get; set; }

		/// <summary>
		/// Gets or sets the name of the project.
		/// </summary>
		/// <value>The name of the project.</value>
		[Required(AllowEmptyStrings = false)]
		public string ProjectName { get; set; }

		/// <summary>
		/// Gets or sets the short name.
		/// </summary>
		/// <value>The short name.</value>
		[Required(AllowEmptyStrings = false)]
		public string ShortName { get; set; }

		/// <summary>
		/// Gets or sets to project.
		/// </summary>
		/// <value>To project.</value>
		public Project ToProject()
		{
			return new Project(this.ProjectCode, this.ProjectName, this.ShortName);
		}
	}
}