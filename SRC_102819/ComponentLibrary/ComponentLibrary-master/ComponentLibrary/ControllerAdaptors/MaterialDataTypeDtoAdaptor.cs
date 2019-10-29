using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors
{
    /// <summary>
    /// The Adapter for Material Data Types
    /// </summary>
    public class MaterialDataTypeDtoAdaptor
    {
        /// <summary>
        /// Froms the material.
        /// </summary>
        /// <param name="material">The material.</param>
        /// <returns></returns>
        public static MaterialDataTypeDto FromMaterial(IMaterial material)
        {
            var materialDefinition = material.ComponentDefinition;
            var headers = new List<HeaderDataTypeDto>();
            foreach (var header in material.Headers)
            {
                var headerDefinition = materialDefinition.Headers.FirstOrDefault(h => header.Key == h.Key);
                headers.Add(FromHeader(header, headerDefinition));
            }
            return new MaterialDataTypeDto
            {
                Id = material.Id,
                Group = material.Group,
                Headers = headers
            };
        }

        private static HeaderDataTypeDto FromHeader(IHeaderData header, IHeaderDefinition headerDefinition)
        {
            var columns = new List<ColumnDataTypeDto>();
            foreach (var column in header.Columns)
            {
                var columnDefinition = headerDefinition.Columns.FirstOrDefault(c => c.Key == column.Key);
                columns.Add(FromColumn(column, columnDefinition));
            }

            return new HeaderDataTypeDto()
            {
                Name = headerDefinition.Name,
                Key = headerDefinition.Key,
                Columns = columns
            };
        }

        private static ColumnDataTypeDto FromColumn(IColumnData column, IColumnDefinition columnDefinition)
        {
            var dataTypeDto = new DataTypeDto();
            dataTypeDto.SetDomain(columnDefinition.DataType);
            var columnValue = ColumnValue(columnDefinition, column);
            return new ColumnDataTypeDto
            {
                Value = columnValue,
                Name = columnDefinition.Name,
                Key = columnDefinition.Key,
                DataType = dataTypeDto,
                IsRequired = columnDefinition.IsRequired,
                IsSearchable = columnDefinition.IsSearchable,
            };
        }

        /// <summary>
        /// Columns the value.
        /// </summary>
        /// <param name="columnDefinition">The column definition.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public static object ColumnValue(IColumnDefinition columnDefinition, IColumnData column)
        {
            if (columnDefinition.DataType is ArrayDataType)
            {
                var columnDefinitionDataType = (ArrayDataType)columnDefinition.DataType;
                var columnValueArray = column.Value as object[];
                if (columnDefinitionDataType.DataType is StaticFileDataType)
                {
                    return columnValueArray?.Select(GetStaticFileColumnValue).ToArray();
                }
                if (columnDefinitionDataType.DataType is CheckListDataType)
                {
                    return columnValueArray?.Select(GetCheckListColumnValue).ToArray();
                }
                if (columnDefinitionDataType.DataType is BrandDataType)
                {
                    return columnValueArray?.Select(GetBrandColumnValue).ToArray();
                }
            }
            else
            {
                if (columnDefinition.DataType is StaticFileDataType)
                    return GetStaticFileColumnValue(column.Value);
                if (columnDefinition.DataType is CheckListDataType)
                    return GetCheckListColumnValue(column.Value);
                if (columnDefinition.DataType is BrandDataType)
                    return GetBrandColumnValue(column.Value);
            }
            return column.Value;
        }

        private static object GetCheckListColumnValue(object columnValue)
        {
            if (columnValue == null) return null;
            var uiRoot = ConfigurationManager.AppSettings["ComponentLibraryUIRoot"];
            var id = ((CheckListValue)columnValue).Id;
            return new Dictionary<string, object>
                {
                    {"ui", $"{uiRoot}/check-lists/{id}"},
                    {"url", $"/check-lists/{id}"},
                    {"id", id}
                };
        }

        private static object GetStaticFileColumnValue(object columnValue)
        {
            if (columnValue == null) return null;
            var uiRoot = ConfigurationManager.AppSettings["CdnBaseUrl"];
            var staticFile = (StaticFile)columnValue;
            var name = staticFile.Name;
            return new Dictionary<string, object>
                {
                    {"url", $"{uiRoot}/static-files/{name}"},
                    {"name", name},
                    {"id", staticFile.Id}
                };
        }

        private static object GetBrandColumnValue(object columnValue)
        {
            if (columnValue == null) return null;
            var brand = columnValue as Brand;
            var brandDataTypeDto = BrandDataTypeDtoAdaptor.FromBrand(brand);
            return brandDataTypeDto;
        }
    }
}