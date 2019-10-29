using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <summary>
    /// Represents a material builder
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.AppService.IMaterialBuilder" />
    public class MaterialBuilder : IMaterialBuilder
    {
        private readonly IHeaderColumnDataValidator _matarialValidator;
        private readonly IMaterialCodeGenerator _materialCodeGenerator;

        /// <summary>
        /// Constructor for material builder.
        /// </summary>
        /// <param name="matarialValidator"></param>
        /// <param name="materialCodeGenerator"></param>
        public MaterialBuilder(IHeaderColumnDataValidator matarialValidator, IMaterialCodeGenerator materialCodeGenerator)
        {
            _matarialValidator = matarialValidator;
            _materialCodeGenerator = materialCodeGenerator;
        }

        /// <inheritdoc/>
        public async Task<IMaterial> BuildAsync(IMaterial material, IMaterialDefinition materialDefinition)
        {
            if (material == null)
                throw new ArgumentException("Material cannot be null.");
            if (materialDefinition == null)
                throw new ArgumentException("Material definition cannot be null.");
            ValidateMaterial(material, materialDefinition);
            var finalMaterial = await BuildMaterial(material, materialDefinition);
            var materialCode = await GenerateMaterialCode(materialDefinition, material);
            finalMaterial.Id = materialCode;
            SetMaterialGroup(finalMaterial);
            return finalMaterial;
        }

        private List<string> BuildBrandSearchKeywords(IBrand brand, IBrandDefinition brandDefinition)
        {
            var values = new List<string>();
            var searcheableColumnDefinitions = brandDefinition.Columns.Where(c => c.IsSearchable).ToList();

            foreach (var searcheableColumnDefinition in searcheableColumnDefinitions)
            {
                var searchableColumnData =
                    brand.Columns.FirstOrDefault(c => c.Key == searcheableColumnDefinition.Key);
                values.AddRange(GetBrandKeywords(searchableColumnData, searcheableColumnDefinition));
            }
            return values;
        }

        private async Task<Material> BuildMaterial(IMaterial material, IMaterialDefinition materialDefinition)
        {
            var headerColumnDataBuilder = new HeaderColumnDataBuilder();
            var headerDatas = await headerColumnDataBuilder.BuildData(materialDefinition.Headers, material.Headers);

            var buildMaterial = new Material()
            {
                ComponentDefinition = materialDefinition,
                Group = material.Group,
                Headers = headerDatas
            };
            buildMaterial.AppendSearchKeywords(BuildSearchKeywords(buildMaterial, materialDefinition));
            return buildMaterial;
        }

        private List<string> BuildSearchKeywords(IMaterial material, IMaterialDefinition materialDefinition)
        {
            var values = new List<string>();
            var searcheableColumnDefinitions = materialDefinition.Headers.SelectMany(h => h.Columns).Where(c => c.IsSearchable).ToList();

            foreach (var searcheableColumnDefinition in searcheableColumnDefinitions)
            {
                var searchableColumnData =
                    material.Headers.SelectMany(h => h.Columns)
                        .FirstOrDefault(c => c.Key == searcheableColumnDefinition.Key);
                values.AddRange(GetKeywords(searchableColumnData, searcheableColumnDefinition));
            }
            return values;
        }

        private async Task<string> GenerateMaterialCode(IMaterialDefinition materialDefinition, IMaterial material)
        {
            return await _materialCodeGenerator.Generate(materialDefinition.Code, material);
        }

        private List<string> GetBrandKeywords(IColumnData columnData, ISimpleColumnDefinition columnDefinition)
        {
            if (columnData?.Value == null)
                return new List<string>();
            if (columnDefinition.DataType is ArrayDataType)
                return ((object[])columnData.Value).Select(data => data.ToString()).ToList();
            var value = columnData.Value?.ToString();
            var keywords = new List<string>();
            if (value != HeaderColumnDataBuilder.Na)
                keywords.Add(value);
            return keywords;
        }

        private List<string> GetKeywords(IColumnData columnData, IColumnDefinition columnDefinition)
        {
            if (columnData?.Value == null)
                return new List<string>();
            if (columnDefinition.DataType is ArrayDataType)
            {
                if (((ArrayDataType)columnDefinition.DataType).DataType is BrandDataType)
                {
                    var brandKeywords = new List<string>();
                    foreach (var brandObject in (object[])columnData.Value)
                    {
                        var brand = (Brand)brandObject;
                        brandKeywords.AddRange(BuildBrandSearchKeywords(brand, brand.BrandDefinition));
                    }
                    return brandKeywords;
                }
                return ((object[])columnData.Value).Select(data => data.ToString()).ToList();
            }
            var value = columnData.Value?.ToString();
            var keywords = new List<string>();
            if (value != HeaderColumnDataBuilder.Na)
                keywords.Add(value);
            return keywords;
        }

        private void SetMaterialGroup(Material finalMaterial)
        {
            finalMaterial.Group = ((dynamic)finalMaterial).classification.material_level_2.Value;
        }

        private void ValidateMaterial(IMaterial material, IMaterialDefinition materialDefinition)
        {
            if (material == null)
                throw new ArgumentException("Material cannot be null.");

            if (materialDefinition == null)
                throw new ArgumentException("Material definition cannot be null.");

            var isValidMaterial = _matarialValidator.Validate(materialDefinition.Headers, material.Headers);
            if (isValidMaterial != null && !isValidMaterial.Item1)
                throw new ArgumentException(isValidMaterial.Item2);
        }
    }
}