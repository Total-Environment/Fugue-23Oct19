using System;
using System.Linq;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <inheritdoc/>
	public class CounterGenerator : ICounterGenerator
	{
		private readonly ICounterRepository _counterRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="CounterGenerator"/> class.
		/// </summary>
		/// <param name="counterRepository">The counter repository.</param>
		public CounterGenerator(ICounterRepository counterRepository)
		{
			_counterRepository = counterRepository;
		}

		/// <inheritdoc/>
		public async Task<string> Generate(string codePrefix, string code, string counterCollection)
		{
			if (string.IsNullOrEmpty(codePrefix))
			{
				//throw new InvalidOperationException("Code prefix for code cannot be empty.");
				throw new InvalidOperationException($"Short notation for the given classification level 1  value {code} is not found.");
			}
			if (code == null)
			{
				if (counterCollection == Keys.CounterCollections.PackageKey)
					return $"{codePrefix}{(await _counterRepository.NextValue(counterCollection)):D4}";
				else
					return $"{codePrefix}{(await _counterRepository.NextValue(counterCollection)):D6}";
			}
			ValidateMaterialCode(code, codePrefix);
			await UpdateCounter(code, counterCollection);
			return code;
		}

		/// <summary>
		/// Last6s the characters.
		/// </summary>
		/// <param name="code">The code.</param>
		/// <returns>The last 6 characters of code.</returns>
		private static string Last6Characters(string code)
		{
			return code.Substring(code.Length - 6);
		}

		/// <summary>
		/// Updates the counter.
		/// </summary>
		/// <param name="code">The code.</param>
		/// <param name="counterCollection">The counter collection.</param>
		/// <returns></returns>
		private async Task UpdateCounter(string code, string counterCollection)
		{
			var numericPart = int.Parse(Last6Characters(code).TrimStart('0'));
			var currentCounter = await _counterRepository.CurrentValue(counterCollection);
			if (numericPart > currentCounter)
				await _counterRepository.Update(numericPart, counterCollection);
		}

		/// <summary>
		/// Validates the material code.
		/// </summary>
		/// <param name="code">The code.</param>
		/// <param name="codePrefix">The code prefix.</param>
		/// <exception cref="System.ArgumentException">
		/// Invalid material code: Length should be 9 or Invalid material code: Wrong prefix or
		/// Invalid material code: Last six character should be numeric
		/// </exception>
		private void ValidateMaterialCode(string code, string codePrefix)
		{
			if (code.Length != 9)
			{
				throw new ArgumentException("Invalid material code: Length should be 9");
			}
			if (!code.StartsWith(codePrefix))
			{
				throw new ArgumentException("Invalid material code: Wrong prefix");
			}
			if (Last6Characters(code).Any(d => !char.IsDigit(d)))
			{
				throw new ArgumentException("Invalid material code: Last six character should be numeric");
			}
		}
	}
}