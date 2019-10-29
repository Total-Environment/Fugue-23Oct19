using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.Extensions;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <summary>
	/// </summary>
	public class CostPriceRatioValidator
	{
		private readonly IProjectRepository _projectRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="CostPriceRatioValidator"/> class.
		/// </summary>
		/// <param name="projectRepository">The project repository.</param>
		public CostPriceRatioValidator(IProjectRepository projectRepository)
		{
			_projectRepository = projectRepository;
		}

		/// <summary>
		/// Validates the project code.
		/// </summary>
		/// <param name="projectCode">The project code.</param>
		/// <returns></returns>
		protected async Task<Tuple<bool, string>> ValidateProjectCode(string projectCode)
		{
			try
			{
				var project = await _projectRepository.Find(projectCode);
				return new Tuple<bool, string>(true, string.Empty);
			}
			catch (ResourceNotFoundException exception)
			{
				return new Tuple<bool, string>(false, $"{projectCode} is not valid. Please enter a valid project code.");
			}
		}

		/// <summary>
		/// Validates the applied from.
		/// </summary>
		/// <param name="appliedFrom">The applied from.</param>
		/// <returns></returns>
		protected Tuple<bool, string> ValidateAppliedFrom(DateTime appliedFrom)
		{
			if (appliedFrom.Kind == DateTimeKind.Unspecified)
				return new Tuple<bool, string>(false, $"Applied from date should be in ISO format.");

			var appliedOnInUtc = TimeZoneInfo.ConvertTimeToUtc(appliedFrom);
			var todayIndianTime = DateTime.UtcNow.InIst();
			var appliedOnIndianTime = appliedFrom.InIst();

			if (appliedOnInUtc.CompareTillMinutePrecision(DateTime.UtcNow) == -1)
				return new Tuple<bool, string>(false, $"Applied from date cannot be set to past.");

			if (appliedOnIndianTime.Date == todayIndianTime.Date)
				return new Tuple<bool, string>(false, $"Applied from date cannot be set on same day in IST.");

			return new Tuple<bool, string>(true, string.Empty);
		}
	}
}