using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <summary>
	/// Represents a brand code generator
	/// </summary>
	/// <seealso cref="TE.ComponentLibrary.ComponentLibrary.AppService.IBrandCodeGenerator"/>
	public class BrandCodeGenerator : IBrandCodeGenerator
	{
		private readonly ICounterRepository _counterRepository;

		/// <inheritdoc/>
		public BrandCodeGenerator(ICounterRepository counterRepository)
		{
			_counterRepository = counterRepository;
		}

		/// <inheritdoc/>
		public async Task<string> Generate(string brandCodePrefix)
		{
			return $"{brandCodePrefix}{await _counterRepository.NextValue("Brand"):D6}";
		}
	}
}