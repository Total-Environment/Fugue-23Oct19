using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;

namespace TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Controller.Dto
{
    /// <summary>
    /// The DTO for Service With Data Type
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto.IJsonSerializable"/>
    public class ServiceWithDataTypeDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceWithDataTypeDto"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        public ServiceWithDataTypeDto(IService service)
        {
            Headers =
                service.Headers.Select(
                    h =>
                        new HeaderDataTypeDto
                        {
                            Key = h.Key,
                            Name = h.Name,
                            Columns =
                                h.Columns.Select(
                                    c => new ColumnDataTypeDto {Key = c.Key, Name = c.Name, Value = c.Value, DataType = GetDataTypeDto(c, h.Key,service), IsSearchable = GetIsSerchable(c, h.Key, service), IsRequired = GetIsRequired(c, h.Key, service) }).ToList()
                        });
            Group = service.Group;
            Id = service.Id;
        }

        private bool GetIsRequired(IColumnData columnData, string headerKey, IService service)
        {
            if (headerKey == "classification_definition")
            {
                return false;
            }
            var definition = GetColumnDefinition(columnData.Key, headerKey, service);
            return definition.IsRequired;
        }

        private bool GetIsSerchable(IColumnData columnData, string headerKey, IService service)
        {
            if (headerKey == "classification_definition")
            {
                return false;
            }
            var definition = GetColumnDefinition(columnData.Key, headerKey, service);
            return definition.IsSearchable;
        }

        private DataTypeDto GetDataTypeDto(IColumnData columnData, string headerKey, IService service)
        {
            var dataTypeDto = new DataTypeDto();
            if (headerKey == "classification_definition")
            {
                dataTypeDto.Name = null;
                dataTypeDto.SubType = null;
            }
            else
            {
                var definition = GetColumnDefinition(columnData.Key, headerKey, service);
                dataTypeDto.SetDomain(definition.DataType);
            }
            return dataTypeDto;
        }

        private IColumnDefinition GetColumnDefinition(string columnDataKey, string headerKey, IService service)
        {
            var headerDefinition = service.ComponentDefinition.Headers.First(h => h.Key == headerKey);
            var columnDefinition = headerDefinition.Columns.First(c => c.Key == columnDataKey);
            return columnDefinition;
        }

        public string Id { get; set; }

        public string Group { get; set; }

        public IEnumerable<HeaderDataTypeDto> Headers { get; set; }

        private static IHeaderDefinition CreateHeaderDefinition(IHeaderData headerData)
        {
            var columns =
                headerData.Columns.Select(
                        headerDataColumn =>
                            new ColumnDefinition(headerDataColumn.Name, headerDataColumn.Key, new StringDataType()))
                    .ToList();
            return new HeaderDefinition(headerData.Name, headerData.Key, columns);
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
                var subType = ((MasterDataDataType) currentDataType).DataList.Id;
                dataType = GetDataTypeWithSubType(currentDataType, subType);
            }
            else if (currentDataType is UnitDataType)
            {
                var subType = ((UnitDataType) currentDataType).Value;
                dataType = GetDataTypeWithSubType(currentDataType, subType);
            }
            else if (currentDataType is RangeDataType)
            {
                var subType = ((RangeDataType) currentDataType).Unit;
                dataType = GetDataTypeWithSubType(currentDataType, subType);
            }
            else if (currentDataType is ArrayDataType)
            {
                var subType = RenderDataType(((ArrayDataType) currentDataType).DataType);
                dataType = GetDataTypeWithSubType(currentDataType, subType);
            }
            else if (currentDataType is ConstantDataType)
            {
                var subType = ((ConstantDataType) currentDataType).Value;
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
            {
                return ((object[]) value).Select(RenderValue).ToArray();
            }
            if (value is CheckListValue)
            {
                var uiRoot = ConfigurationManager.AppSettings["ComponentLibraryUIRoot"];
                var id = ((CheckListValue) value).Id;
                return new Dictionary<string, object>
                {
                    {"ui", $"{uiRoot}/check-lists/{id}"},
                    {"data", $"/check-lists/{id}"},
                    {"id", id}
                };
            }
            if (value is StaticFile)
            {
                var uiRoot = ConfigurationManager.AppSettings["CdnBaseUrl"];
                var staticFile = (StaticFile) value;
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

            foreach (var columnData in columnsData)
            {
                var matchingColumnDefinition = columnDefinitions.First(cd => cd.Name == columnData.Name);
                var definition = new Dictionary<string, object>
                {
                    {"key", columnData == null ? null : columnData.Key},
                    {"Value", RenderValue(columnData.Value)},
                    {"isRequired", matchingColumnDefinition.IsRequired},
                    {"DataType", RenderDataType(matchingColumnDefinition.DataType)}
                };
                columnsWithDefinition.Add(columnData.Name, definition);
            }

            return columnsWithDefinition;
        }
    }
}