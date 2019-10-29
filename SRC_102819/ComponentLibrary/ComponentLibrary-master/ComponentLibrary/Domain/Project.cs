using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
	/// <summary>
	/// </summary>
	public class Project
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Project"/> class.
		/// </summary>
		/// <param name="projectCode">The project code.</param>
		/// <param name="projectName">Name of the project.</param>
		/// <param name="shortName">The short name.</param>
		public Project(string projectCode, string projectName, string shortName)
		{
			ProjectCode = projectCode;
			ProjectName = projectName;
			ShortName = shortName;
		}

		/// <summary>
		/// Gets the project code.
		/// </summary>
		/// <value>The project code.</value>
		public string ProjectCode { get; }

		/// <summary>
		/// Gets the name of the project.
		/// </summary>
		/// <value>The name of the project.</value>
		public string ProjectName { get; }

		/// <summary>
		/// Gets the short name.
		/// </summary>
		/// <value>The short name.</value>
		public string ShortName { get; }
	}
}