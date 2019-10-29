﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs
{
    /// <summary>
    /// The DTO for Material with Data Type
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto.IJsonSerializable" />
    public class MaterialWithDataTypeDto : IJsonSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialWithDataTypeDto"/> class.
        /// </summary>
        /// <param name="material">The material.</param>
        public MaterialWithDataTypeDto(IMaterial material)
        {
            Material = material;
        }

        /// <summary>
        /// Gets or sets the material.
        /// </summary>
        /// <value>
        /// The material.
        /// </value>
        public IMaterial Material { get; set; }

        /// <inheritdoc/>
        public IDictionary<string, object> ToJson()
        {
            var materialDefinition = Material.ComponentDefinition.Headers;
            var dictionary = new Dictionary<string, object>();

            foreach (var headerDefinition in Material.ComponentDefinition.Headers)
            {
                var headerData = Material.Headers.First(h => h.Name.Equals(headerDefinition.Name));
                dictionary.Add(headerDefinition.Name,
                    RenderValueWithdataTypes(headerData?.Columns, headerDefinition.Columns));
            }

            dictionary["id"] = Material.Id;
            dictionary["group"] = Material.Group;
            var autogeneratedColumns = Material.ComponentDefinition.Headers.SelectMany(
                h => h.Columns.Where(c => c.DataType is AutogeneratedDataType)
                    .Select(
                        c =>
                            new Tuple<string, string, AutogeneratedDataType>(h.Name, c.Name,
                                (AutogeneratedDataType)c.DataType)));
            foreach (var column in autogeneratedColumns)
                ((IDictionary<string, object>)dictionary[column.Item1])[column.Item2] =
                    column.Item3.DataTypeResolve(Material);
            return dictionary;
        }

        private static Dictionary<string, object> GetDataTypeWithSubType(IDataType currentDataType, object subType)
        {
            var dataType = new Dictionary<string, object>();
            var dataTypeName = currentDataType.GetType().Name.Replace("DataType", "");
            dataType.Add("Name", dataTypeName);
            dataType.Add("SubType", subType);
            return dataType;
        }

        private static object RenderDataType(IDataType currentDataType)
        {
            var dataType = new Dictionary<string, object>();
            if (currentDataType is MasterDataDataType)
            {
                var subType = ((MasterDataDataType)currentDataType).DataList.Id;
                dataType = GetDataTypeWithSubType(currentDataType, subType);
            }
            else if (currentDataType is UnitDataType)
            {
                var subType = ((UnitDataType)currentDataType).Value;
                dataType = GetDataTypeWithSubType(currentDataType, subType);
            }
            else if (currentDataType is RangeDataType)
            {
                var subType = ((RangeDataType)currentDataType).Unit;
                dataType = GetDataTypeWithSubType(currentDataType, subType);
            }
            else if (currentDataType is ArrayDataType)
            {
                var subType = RenderDataType(((ArrayDataType)currentDataType).DataType);
                dataType = GetDataTypeWithSubType(currentDataType, subType);
            }
            else if (currentDataType is ConstantDataType)
            {
                var subType = ((ConstantDataType)currentDataType).Value;
                dataType = GetDataTypeWithSubType(currentDataType, subType);
            }
            else
            {
                var dataTypeName = currentDataType.GetType().Name.Replace("DataType", "");
                dataType.Add("Name", dataTypeName);
                dataType.Add("SubType", null);
            }
            return dataType;
        }

        private static object RenderValue(object value)
        {
            if (value is object[])
                return ((object[])value).Select(RenderValue).ToArray();
            if (value is CheckListValue)
            {
                var uiRoot = ConfigurationManager.AppSettings["ComponentLibraryUIRoot"];
                var id = ((CheckListValue)value).Id;
                return new Dictionary<string, object>
                {
                    {"ui", $"{uiRoot}/check-lists/{id}"},
                    {"url", $"/check-lists/{id}"},
                    {"id", id}
                };
            }
            if (value is StaticFile)
            {
                var uiRoot = ConfigurationManager.AppSettings["CdnBaseUrl"];
                var staticFile = (StaticFile)value;
                var name = staticFile.Name;
                return new Dictionary<string, object>
                {
                    {"url", $"{uiRoot}/static-files/{name}"},
                    {"name", name},
                    {"id", staticFile.Id}
                };
            }
            return value;
        }

        private static object RenderValueWithdataTypes(IEnumerable<IColumnData> columnsData,
                                    IEnumerable<IColumnDefinition> columnsDefinition)
        {
            var columnsWithDefinition = new Dictionary<string, object>();
            var columnDefinitions = columnsDefinition as IList<IColumnDefinition> ?? columnsDefinition.ToList();

            foreach (var columnDefintion in columnDefinitions)
            {
                var columnData = columnsData?.FirstOrDefault(c => c.Name.Equals(columnDefintion.Name));
                var definition = new Dictionary<string, object>
                {
                    {"key",columnData == null ? null :columnData.Key },
                    {"Value", columnData == null ? null : RenderValue(columnData.Value)},
                    {"isRequired", columnDefintion.IsRequired},
                    {"DataType", RenderDataType(columnDefintion.DataType)}
                };
                columnsWithDefinition.Add(columnDefintion.Name, definition);
            }

            return columnsWithDefinition;
        }
    }
}