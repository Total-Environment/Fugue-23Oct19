using System.Collections.Generic;
using System.Linq;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors
{
    /// <summary>
    /// The Adapter for Material DTO
    /// </summary>
    public static class MaterialDtoAdapter
    {
        /// <summary>
        /// Gets Material Data DTO from the material.
        /// </summary>
        /// <param name="material">The material.</param>
        /// <returns></returns>
        public static MaterialDataDto FromMaterial(IMaterial material)
        {
            var materialDefinition = material.ComponentDefinition;
            var headers = new List<HeaderDto>();
            foreach (var header in material.Headers)
            {
                var headerDefinition = materialDefinition.Headers.FirstOrDefault(h => header.Key == h.Key);
                headers.Add(FromHeader(header, headerDefinition));
            }
            return new MaterialDataDto
            {
                Id = material.Id,
                Group = material.Group,
                Headers = headers
            };
        }

        private static HeaderDto FromHeader(IHeaderData header, IHeaderDefinition headerDefinition)
        {
            var columns = new List<ColumnDto>();
            foreach (var column in header.Columns)
            {
                var columnDefinition = headerDefinition.Columns.FirstOrDefault(c => c.Key == column.Key);
                columns.Add(FromColumn(column, columnDefinition));
            }

            return new HeaderDto()
            {
                Name = headerDefinition.Name,
                Key = headerDefinition.Key,
                Columns = columns
            };
        }

        private static ColumnDto FromColumn(IColumnData column, IColumnDefinition columnDefinition)
        {
            var columnValue = ColumnValue(columnDefinition, column);
            return new ColumnDto
            {
                Value = columnValue,
                Name = columnDefinition.Name,
                Key = columnDefinition.Key,
            };
        }

        /// <summary>
        /// Converts the DTO to the material.
        /// </summary>
        /// <param name="materialDto">The material dto.</param>
        /// <returns></returns>
        public static IMaterial ToMaterial(MaterialDataDto materialDto)
        {
            return new Material
            {
                Group = materialDto.Group,
                Id = materialDto.Id,
                Headers = materialDto.Headers.Select(h => h.GetDomain())
            };
        }

        private static object ColumnValue(IColumnDefinition columnDefinition, IColumnData column)
        {
            if (columnDefinition.DataType is ArrayDataType)
            {
                var columnDefinitionDataType = (ArrayDataType)columnDefinition.DataType;
                var columnValueArray = column.Value as object[];
                if (columnDefinitionDataType.DataType is BrandDataType)
                {
                    return columnValueArray?.Select(GetBrandColumnValue).ToArray();
                }
                else
                {
                    return MaterialDataTypeDtoAdaptor.ColumnValue(columnDefinition, column);
                }
            }
            else
            {
                if (columnDefinition.DataType is BrandDataType)
                    return GetBrandColumnValue(column.Value);
                else
                    return MaterialDataTypeDtoAdaptor.ColumnValue(columnDefinition, column);
            }
        }

        private static object GetBrandColumnValue(object columnValue)
        {
            if (columnValue == null) return null;
            var brand = columnValue as Brand;
            var brandDto = BrandDtoAdapter.FromBrand(brand);
            return brandDto;
        }
    }
}