using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;

namespace TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes
{
	/// <summary>
	/// The Brand Data Type
	/// </summary>
	/// <seealso cref="IDataType"/>
	public class BrandDataType : IDataType
	{
		/// <summary>
		/// Gets the brand code
		/// </summary>
		/// <value>The brand code.</value>
		public string BrandCodePrefix { get; }

		private readonly IBrandDefinition _brandDefinition;

		private readonly IBrandCodeGenerator _brandCodeGenerator;

		private readonly ICounterRepository _counterRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="BrandDataType"/> class.
		/// </summary>
		/// <param name="brandDefinition">The brand definition.</param>
		/// <param name="brandCode">The brand code.</param>
		/// <param name="brandCodeGenerator">The brand code generator.</param>
		/// <exception cref="System.ArgumentNullException">brandDefinition or brandCode</exception>
		public BrandDataType(IBrandDefinition brandDefinition, string brandCode, IBrandCodeGenerator brandCodeGenerator, ICounterRepository counterRepository)
		{
			if (brandDefinition == null)
			{
				throw new ArgumentNullException(nameof(brandDefinition));
			}
			_brandDefinition = brandDefinition;
			if (brandCode == null)
			{
				throw new ArgumentNullException(nameof(brandCode));
			}
			BrandCodePrefix = brandCode;
			if (brandCodeGenerator == null)
			{
				throw new ArgumentNullException(nameof(brandCodeGenerator));
			}
			_brandCodeGenerator = brandCodeGenerator;
			if (counterRepository == null)
			{
				throw new ArgumentNullException(nameof(counterRepository));
			}
			_counterRepository = counterRepository;
		}

		/// <inheritdoc/>
		public async Task<object> Parse(object columnData)
		{
			if (!(columnData is IDictionary<string, object>))
			{
				throw new FormatException($"Expected dictionary. Got {columnData.GetType().Name}.");
			}
			var columnDataDictionary = (IDictionary<string, object>)columnData;
			var columns = columnDataDictionary["columns"];
			if (!(columns is IEnumerable<object>))
			{
				throw new FormatException($"Expected List of columns. Got {columns.GetType().Name}.");
			}
			var listOfColumns = (IEnumerable<object>)columns;
			var columnDefinitions = listOfColumns.Select(column => (IDictionary<string, object>)column).ToList();
			var brand = await _brandDefinition.Parse(columnDefinitions);
			var inputBrandCodeColumn = columnDefinitions.FirstOrDefault(column => (string)column["key"] == "brand_code");
			if (inputBrandCodeColumn?["value"] == null)
			{
				brand["brand_code"] = await _brandCodeGenerator.Generate(BrandCodePrefix);
			}
			else
			{
				var brandCode = (string)inputBrandCodeColumn["value"];
				Validate(brandCode);
				await UpdateCounter(brandCode);
				brand["brand_code"] = brandCode;
			}
			return brand;
		}

		private void Validate(string brandCode)
		{
			if (brandCode.Length != 9)
			{
				throw new FormatException("Invalid brand code: Length should be 9");
			}
			if (!brandCode.StartsWith(BrandCodePrefix))
			{
				throw new FormatException("Invalid brand code: Wrong prefix");
			}
			if (Last6Characters(brandCode).Any(d => !char.IsDigit(d)))
			{
				throw new FormatException("Invalid brand code: Last six characters should be numeric");
			}
		}

		private string Last6Characters(string brandCode)
		{
			return brandCode.Substring(brandCode.Length - 6);
		}

		private async Task UpdateCounter(string brandCode)
		{
			var numericPart = int.Parse(Last6Characters(brandCode).TrimStart('0'));
			var currentCounter = await _counterRepository.CurrentValue("Brand");
			if (numericPart > currentCounter)
				await _counterRepository.Update(numericPart, "Brand");
		}
	}
}