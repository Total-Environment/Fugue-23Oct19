using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
	/// <summary>
	/// </summary>
	public interface IProjectRepository
	{
		/// <summary>
		/// Finds the specified project code.
		/// </summary>
		/// <param name="projectCode">The project code.</param>
		/// <returns></returns>
		Task<Project> Find(string projectCode);

		/// <summary>
		/// Adds the specified project.
		/// </summary>
		/// <param name="project">The project.</param>
		/// <returns></returns>
		Task<Project> Add(Project project);

	    Task<IEnumerable<Project>> List();
	}
}