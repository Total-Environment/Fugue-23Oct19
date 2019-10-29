using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <summary>
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.AppService.ComponentCoefficientValidator"/>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.AppService.IComponentCoefficientValidator"/>
    public class MaterialCoefficientValidator : ComponentCoefficientValidator, IComponentCoefficientValidator
    {
        private readonly IMaterialRepository _materialRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialCoefficientValidator"/> class.
        /// </summary>
        /// <param name="materialRepository">The material repository.</param>
        public MaterialCoefficientValidator(IMaterialRepository materialRepository)
        {
            _materialRepository = materialRepository;
        }

        /// <summary>
        /// Validates the specified material coefficient.
        /// </summary>
        /// <param name="materialCoefficient">The material coefficient.</param>
        /// <returns></returns>
        public new async Task<Tuple<bool, string>> Validate(ComponentCoefficient materialCoefficient)
        {
            var result = base.Validate(materialCoefficient);
            if (result.Item1)
            {
                return await ValidateMaterialCode(materialCoefficient.Code);
            }
            else
            {
                return result;
            }
        }

        private async Task<Tuple<bool, string>> ValidateMaterialCode(string materialCode)
        {
            if (string.IsNullOrWhiteSpace(materialCode))
                return new Tuple<bool, string>(false, "Material code is mandatory.");

            var count = await _materialRepository.Count(new Dictionary<string, Tuple<string, object>>
            {
                {"material_code", new Tuple<string, object>("Regex", $"((?i){materialCode}(?-i))")}
            });
            if (count == 1)
                return new Tuple<bool, string>(true, string.Empty);
            else
                return new Tuple<bool, string>(false, $"Invalid material code {materialCode}");
        }
    }
}