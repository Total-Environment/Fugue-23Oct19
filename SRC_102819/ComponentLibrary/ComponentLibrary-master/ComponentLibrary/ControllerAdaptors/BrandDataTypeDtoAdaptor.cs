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
    /// The Adapter for Brand Data Type
    /// </summary>
    public class BrandDataTypeDtoAdaptor
    {
        /// <summary>
        /// Froms the brand.
        /// </summary>
        /// <param name="brand">The brand.</param>
        /// <returns></returns>
        public static BrandDataTypeDto FromBrand(IBrand brand)
        {
            var brandDefinition = brand.BrandDefinition;
            var columns = new List<ColumnDataTypeDto>();
            foreach (var column in brand.Columns)
            {
                var columnDefinition = brandDefinition.Columns.FirstOrDefault(h => column.Key == h.Key);
                columns.Add(FromColumn(column, columnDefinition));
            }
            return new BrandDataTypeDto
            {
                Columns = columns
            };
        }

        private static ColumnDataTypeDto FromColumn(IColumnData column, ISimpleColumnDefinition columnDefinition)
        {
            var dataTypeDto = new DataTypeDto();
            dataTypeDto.SetDomain(columnDefinition.DataType);
            var columnValue = ColumnValue(columnDefinition, column);
            return new ColumnDataTypeDto
            {
                Value = columnValue,
                Name = columnDefinition.Name,
                Key = columnDefinition.Key,
                DataType = dataTypeDto
            };
        }

        /// <summary>
        /// Columns the value.
        /// </summary>
        /// <param name="columnDefinition">The column definition.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public static object ColumnValue(ISimpleColumnDefinition columnDefinition, IColumnData column)
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
            }
            else
            {
                if (columnDefinition.DataType is StaticFileDataType)
                    return GetStaticFileColumnValue(column.Value);
                if (columnDefinition.DataType is CheckListDataType)
                    return GetCheckListColumnValue(column.Value);
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
    }
}