using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.DataAdaptors;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <summary>
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.AppService.ComponentCoefficientValidator"/>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.AppService.IComponentCoefficientValidator"/>
    public class AssetCoefficientValidator : ComponentCoefficientValidator, IComponentCoefficientValidator
    {
        private readonly IMaterialRepository _materialRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetCoefficientValidator"/> class.
        /// </summary>
        /// <param name="materialRepository">The material repository.</param>
        public AssetCoefficientValidator(IMaterialRepository materialRepository)
        {
            _materialRepository = materialRepository;
        }

        /// <summary>
        /// Validates the specified asset coefficient.
        /// </summary>
        /// <param name="componentCoefficient">The asset coefficient.</param>
        /// <returns></returns>
        public new async Task<Tuple<bool, string>> Validate(ComponentCoefficient componentCoefficient)
        {
            var result = base.Validate(componentCoefficient);
            if (result.Item1)
            {
                var assetCodeResult = await ValidateAssetCode(componentCoefficient.Code);
                if (assetCodeResult.Item1)
                {
                    return ValidateUnitOfMeasure(componentCoefficient.UnitOfMeasure, componentCoefficient.Code);
                }
                else
                {
                    return assetCodeResult;
                }
            }
            else
            {
                return result;
            }
        }

        private async Task<Tuple<bool, string>> ValidateAssetCode(string assetCode)
        {
            if (string.IsNullOrWhiteSpace(assetCode))
                return new Tuple<bool, string>(false, "Asset code is mandatory.");

            var count = await _materialRepository.Count(new Dictionary<string, Tuple<string, object>>
            {
                {MaterialDao.MaterialCode, new Tuple<string, object>("Regex", $"((?i){assetCode}(?-i))")},
                {ComponentDao.CanBeUsedAsAnAsset, new Tuple<string, object>("Eq", true) }
            });
            if (count == 1)
                return new Tuple<bool, string>(true, string.Empty);
            else
                return new Tuple<bool, string>(false, $"Invalid asset code {assetCode}");
        }

        private Tuple<bool, string> ValidateUnitOfMeasure(string unitOfMeasure, string assetCode)
        {
            if (string.IsNullOrWhiteSpace(unitOfMeasure))
                return new Tuple<bool, string>(false, $"Unit of Measure field is required for asset {assetCode}");
            else
                return new Tuple<bool, string>(true, string.Empty);
        }
    }
}