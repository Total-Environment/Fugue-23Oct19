using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ExcelImporter.Code.Excels;
using TE.Diagnostics.Logging;

namespace TE.ComponentLibrary.ExcelImporter.Code.Components
{
    public class ComponentConverter : JsonConverter, IConverter
    {
        private readonly ComponentDefinitionDao _componentDefinition;
        private readonly IEnumerable<Header> _headers;
        private readonly Logger _logger;

        public ComponentConverter(IEnumerable<Header> headers, ComponentDefinitionDao componentDefinition)
        {
            _componentDefinition = componentDefinition;
            _headers = headers;
            _logger = new Logger(typeof(ComponentConverter));
        }

        public event OnNewUrl UrlUpdater;

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            return existingValue;
        }

        public JsonConverter ToJsonConverter()
        {
            return this;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var keyValuePair = value as Dictionary<ICustomCell, ICustomCell>;
            if (keyValuePair == null) return;

            writer.WriteStartObject();

            foreach (var header in _headers)
            {
                var systemLogs = "System Logs";
                if (header.Name == systemLogs)
                {
                    WriteJsonPropertyName(writer, systemLogs);
                    writer.WriteStartObject();
                    writer.WriteEndObject();
                    continue;
                }

                if (WriteJsonPropertyName(writer, header.Name) == null)
                    continue;
                writer.WriteStartObject();
                WriteRow(writer, keyValuePair, header);
                writer.WriteEndObject();
            }
            writer.WriteEndObject();
        }

        public string WriteJsonPropertyName(JsonWriter writer, string excelPropertyName)
        {
            writer.WritePropertyName(excelPropertyName);
            return excelPropertyName;
        }

        private static void WriteArrayStringValue(JsonWriter writer, string writtenProperty, string stringValue)
        {
            if (writtenProperty == null) return;

            if (string.IsNullOrEmpty(stringValue))
            {
                writer.WriteNull();
                return;
            }

            var array = stringValue.Split(',');
            writer.WriteStartArray();
            foreach (var item in array)
                writer.WriteValue(item.Trim());
            writer.WriteEndArray();
        }

        private static void WriteBoolValue(JsonWriter writer, string writtenProperty, string stringValue)
        {
            if (writtenProperty == null) return;

            var convertedValue = false;
            if (string.IsNullOrEmpty(stringValue))
            {
                writer.WriteNull();
                return;
            }

            convertedValue = string.Equals(stringValue, "yes", StringComparison.OrdinalIgnoreCase);
            writer.WriteValue(convertedValue);
        }

        private object GetValue(ICustomCell columnValue, DataTypeDto dataType)
        {
            object result = columnValue == null ? null : columnValue.Value;
            if (result == null)
            {
                return null;
            }
            switch (dataType.Name.ToLower())
            {
                case "unit":
                    var unitValue = new Dictionary<string, string>();
                    unitValue.Add("Type", dataType.SubType.ToString());
                    unitValue.Add("Value", result.ToString());
                    return unitValue;

                case "boolean":
                    if (result.ToString().ToLower() == "no")

                    {
                        return "false";
                    }
                    else if (result.ToString().ToLower() == "yes")
                    {
                        return "true";
                    }
                    else
                    {
                        return result.ToString();
                    }

                case "array":
                    if (dataType.SubType.GetType() == typeof(JObject))
                    {
                        var subtype = ((JObject)dataType.SubType).ToObject<DataTypeDto>();
                        if (subtype.Name.ToLower() == "string")
                        {
                            return new List<string>() { result.ToString() };
                        }
                        return null;
                    }
                    break;
                case "range":
                    if (result == "--NA--")
                    {
                        return "--NA--";
                    }
                    var rangeValue = new Dictionary<string, string>();
                    rangeValue.Add("Unit", dataType.SubType.ToString());
                    var rangeComponents = result.ToString().Split('-');
                    if (rangeComponents.Length == 0 || rangeComponents.Length > 2)
                    {
                        return null;
                    }
                    rangeValue.Add("From", rangeComponents[0]);
                    if (rangeComponents.Length == 2)
                    {
                        rangeValue.Add("To", rangeComponents[1]);
                    }
                    return rangeValue;

                case "checklist":
                case "staticfile":
                    return null;

                default:
                    if (columnValue != null)
                    {
                        result = columnValue.Value;
                    }
                    break;
            }

            return result;
        }

        private void WriteArrayStringObject(JsonWriter writer, Dictionary<ICustomCell, ICustomCell> keyValuePair,
            ICustomCell columnNameCell)
        {
            var writtenPropertyName = WriteJsonPropertyName(writer, columnNameCell.Value);
            if (writtenPropertyName == null) return;

            var arrayCommaSeperated = keyValuePair[columnNameCell].Value;
            WriteArrayStringValue(writer, writtenPropertyName, arrayCommaSeperated);
        }

        private void WriteBoolObject(JsonWriter writer, Dictionary<ICustomCell, ICustomCell> keyValuePair,
            ICustomCell columnNameCell)
        {
            var boolPropertyName = WriteJsonPropertyName(writer, columnNameCell.Value);
            if (boolPropertyName == null) return;

            var stringValue = keyValuePair[columnNameCell].Value;
            WriteBoolValue(writer, boolPropertyName, stringValue);
        }

        private void WriteRangeObject(JsonWriter writer, ICustomCell customCell, string dataValue, Header relatedHeader)
        {
            if (WriteJsonPropertyName(writer, customCell.Value) == null)
                return;
            if (string.IsNullOrWhiteSpace(dataValue))
            {
                writer.WriteNull();
                return;
            }
            if (dataValue == "-- NA --")
            {
                writer.WriteValue("-- NA --");
                return;
            }

            writer.WriteStartObject();
            writer.WritePropertyName("Unit");
            var unit = relatedHeader.ColumnCells.First(c => c.Item1.Value == customCell.Value).Item2;
            writer.WriteValue(unit);

            var values = dataValue.Split('-');

            writer.WritePropertyName("From");
            writer.WriteValue(values[0]);

            if (values.Length > 1)
            {
                writer.WritePropertyName("To");
                writer.WriteValue(values[1]);
            }

            writer.WriteEndObject();
        }

        private void WriteRow(JsonWriter writer, Dictionary<ICustomCell, ICustomCell> keyValuePair, Header header)
        {
            foreach (var columnNameCell in keyValuePair.Keys)
            {
                var relatedColumnCell = header.ColumnCells.FirstOrDefault(customCell => customCell.Item1.Value == columnNameCell.Value);
                if (relatedColumnCell == null) continue;

                try
                {
                    var relatedHeader = _componentDefinition.Headers.FirstOrDefault(
                        h => h.Columns.Any(c => c.Name == columnNameCell.Value));
                    if (relatedHeader == null)
                    {
                        throw new NullReferenceException(
                            $"Null Exception: {columnNameCell.Value} does not have related Header.");
                    }
                    var trimmedCellValue = keyValuePair[columnNameCell].Value?.Trim();
                    if ((columnNameCell.Value == "Service Code" && relatedHeader.Name == "General") || (columnNameCell.Value == "Material Code" && relatedHeader.Name == "General"))
                    {
                        if (trimmedCellValue == null)
                        {
                            throw new InvalidDataException($"{columnNameCell.Value} cannot have a null value.");
                        }
                        var group = trimmedCellValue.Substring(0, 3);
                        if (group != _componentDefinition.Code)
                        {
                            throw new InvalidDataException(
                                $"Invalid {_componentDefinition.GetType().Name.Remove(_componentDefinition.GetType().Name.Length - 13)} group code in {relatedColumnCell.Item1.Value}. ");
                        }
                    }
                    var column = relatedHeader.Columns.FirstOrDefault(c => c.Name == columnNameCell.Value);
                    var type = column.DataType;
                    var dataType = type.Name;
                    switch (dataType.ToLower())
                    {
                        case "date":
                            var propertyName = WriteJsonPropertyName(writer, columnNameCell.Value);
                            if (propertyName != null)
                            {
                                if (trimmedCellValue == null)
                                {
                                    writer.WriteNull();
                                }
                                else
                                {
                                    writer.WriteValue(DateTime.FromOADate(double.Parse(trimmedCellValue)).ToString("yyyy-MM-dd"));
                                }
                            }
                            break;
                        case "unit":
                            WriteUnitObject(writer, columnNameCell, trimmedCellValue, header);
                            break;

                        case "boolean":
                            WriteBoolObject(writer, keyValuePair, columnNameCell);
                            break;

                        case "array":
                            if (type.SubType.GetType() == typeof(JObject))
                            {
                                var subtype = ((JObject)type.SubType).ToObject<DataTypeDto>();
                                if (subtype.Name.ToLower() == "staticfile")
                                {
                                    var dataCell = keyValuePair[columnNameCell];
                                    UrlUpdater?.Invoke(keyValuePair, columnNameCell, dataCell, dataType.ToLower());
                                    WriteStaticFile(writer, relatedColumnCell.Item1, dataCell);
                                }
                                else
                                {
                                    WriteArrayStringObject(writer, keyValuePair, columnNameCell);
                                }
                            }
                            break;

                        case "range":
                            WriteRangeObject(writer, columnNameCell, trimmedCellValue, header);
                            break;

                        case "checklist":
                        case "staticfile":
                            var cellValue = keyValuePair[columnNameCell];
                            UrlUpdater?.Invoke(keyValuePair, columnNameCell, cellValue, dataType.ToLower());
                            WriteStaticFile(writer, relatedColumnCell.Item1, cellValue);
                            break;

                        default:
                            var writtenProperty = WriteJsonPropertyName(writer, columnNameCell.Value);
                            if (writtenProperty != null)
                            {
                                var value = trimmedCellValue;
                                if (header.Name.ToLower() == "classification" && (value == "-- NA --" || value == "N/A"))
                                {
                                    writer.WriteNull();
                                }
                                else
                                {
                                    writer.WriteValue(value);
                                }
                            }
                            break;
                    }
                }
                catch (NullReferenceException ex)
                {
                    var message = ex.Message;
                    Console.WriteLine(message);
                    _logger.Error(message);
                }
                catch (InvalidDataException ex)
                {
                    var message = ex.Message;
                    Console.WriteLine(message);
                    _logger.Error(message);
                    throw;
                }
                catch (Exception ex)
                {
                    var message = $"Error while writing cell {columnNameCell.Value}, Exception {ex.StackTrace}";
                    Console.WriteLine(message);
                    _logger.Error(message);
                    throw;
                }
            }
        }

        private void WriteStaticFile(JsonWriter writer, ICustomCell columnNameCell, ICustomCell dataCell)
        {
            var writtenValue = WriteJsonPropertyName(writer, columnNameCell.Value?.Trim());
            if (writtenValue == null) return;
            writer.WriteNull();
        }

        private void WriteUnitObject(JsonWriter writer, ICustomCell customCell, string dataValue, Header relatedHeader)
        {
            if (WriteJsonPropertyName(writer, customCell.Value) == null)
                return;
            if (dataValue == "NA")
            {
                writer.WriteValue("NA");
                return;
            }

            if (dataValue == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartObject();
            writer.WritePropertyName("Type");
            var unit = relatedHeader.ColumnCells.First(c => c.Item1.Value == customCell.Value).Item2;
            writer.WriteValue(unit);

            writer.WritePropertyName("Value");
            writer.WriteValue(dataValue);
            writer.WriteEndObject();
        }
    }
}