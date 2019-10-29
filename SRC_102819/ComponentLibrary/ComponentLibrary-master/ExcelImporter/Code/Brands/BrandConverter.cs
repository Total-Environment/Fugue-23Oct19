using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ExcelImporter.Code.Documents;
using TE.ComponentLibrary.ExcelImporter.Code.Excels;

namespace TE.ComponentLibrary.ExcelImporter.Code.Brands
{
    internal class BrandConverter : JsonConverter, IConverter
    {
        private readonly BrandDefinitionDto _brandDefinition;
        private readonly Dictionary<string, List<StaticFileInformation>> _importedFiles;
        private List<StaticFileInformation> _staticFileRecord;

        public BrandConverter(BrandDefinitionDto brandDefinition, Dictionary<string, List<StaticFileInformation>> importedFiles)
        {
            _brandDefinition = brandDefinition;
            _importedFiles = importedFiles;
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
            WriteRow(writer, keyValuePair);
            writer.WriteEndObject();
        }

        public string WriteJsonPropertyName(JsonWriter writer, string excelPropertyName)
        {
            writer.WritePropertyName(excelPropertyName);
            return excelPropertyName;
        }

        private void WriteColumn(JsonWriter writer, string key, string name, string value)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("key");
            writer.WriteValue(key);
            writer.WritePropertyName("name");
            writer.WriteValue(name);
            writer.WritePropertyName("value");
            if ((value == "-- NA --") || (value == "N/A") || (value == "NA"))
                writer.WriteNull();
            else
                writer.WriteValue(value);
            writer.WriteEndObject();
        }

        private void WriteRow(JsonWriter writer, Dictionary<ICustomCell, ICustomCell> keyValuePair)
        {
            var brandCode = keyValuePair.SingleOrDefault(k => k.Key.Value == "Brand Code").Value.Value;
            if (brandCode != null)
            {
                _staticFileRecord = _importedFiles.SingleOrDefault(i => i.Key == brandCode).Value;
            }
            writer.WritePropertyName("columns");
            writer.WriteStartArray();
            foreach (var columnNameCell in keyValuePair.Keys)
            {
                var relatedColumnCell =
                    _brandDefinition.Columns.FirstOrDefault(customCell => customCell.Name == columnNameCell.Value);
                if (relatedColumnCell == null) continue;

                try
                {
                    var trimmedCellValue = keyValuePair[columnNameCell].Value?.Trim();
                    if ((columnNameCell.Value == "Brand Code") || (columnNameCell.Value == "Material Code"))
                    {
                        if (trimmedCellValue == null)
                            throw new InvalidDataException($"{columnNameCell.Value} cannot have a null value.");
                    }

                    var column = _brandDefinition.Columns.FirstOrDefault(c => c.Name == columnNameCell.Value);
                    var type = column.DataType;
                    var dataType = type.Name;
                    switch (dataType.ToLower())
                    {
                        case "staticfile":
                            var staticFile =
                                _staticFileRecord.SingleOrDefault(
                                        v => v.SecondaryHeaderName.ToLower() == columnNameCell.Value.ToLower())
                                    .StaticFiles.FirstOrDefault();
                            if (staticFile != null)
                            {
                                WriteStaticFile(writer, relatedColumnCell.Key, columnNameCell.Value, staticFile.Id,
                                    staticFile.Name);
                            }
                            var cellValue = keyValuePair[columnNameCell];
                            UrlUpdater?.Invoke(keyValuePair, columnNameCell, cellValue, dataType.ToLower());
                            break;

                        case "array":
                            var staticFiles =
                                _staticFileRecord.SingleOrDefault(
                                        v => v.SecondaryHeaderName.ToLower() == columnNameCell.Value.ToLower())
                                    .StaticFiles;
                            if (staticFiles != null)
                            {
                                WriteStaticFileArray(writer, relatedColumnCell.Key, columnNameCell.Value,
                                    staticFiles);
                            }
                            cellValue = keyValuePair[columnNameCell];
                            UrlUpdater?.Invoke(keyValuePair, columnNameCell, cellValue, dataType.ToLower());
                            break;

                        default:
                            WriteColumn(writer, relatedColumnCell.Key, columnNameCell.Value, trimmedCellValue);
                            break;
                    }
                }
                catch (NullReferenceException ex)
                {
                    var message = ex.Message;
                    Console.WriteLine(message);
                }
                catch (InvalidDataException ex)
                {
                    var message = ex.Message;
                    Console.WriteLine(message);
                    throw;
                }
                catch (Exception ex)
                {
                    var message = $"Error while writing cell {columnNameCell.Value}, Exception {ex.StackTrace}";
                    Console.WriteLine(message);
                    throw;
                }
            }
            writer.WriteEndArray();
        }

        private void WriteStaticFile(JsonWriter writer, string key, string name, string staticFileId, string staticFileName)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("key");
            writer.WriteValue(key);

            writer.WritePropertyName("name");
            writer.WriteValue(name);

            writer.WritePropertyName("value");

            writer.WriteStartObject();

            writer.WritePropertyName("id");
            writer.WriteValue(staticFileId);

            writer.WritePropertyName("name");
            writer.WriteValue(staticFileName);

            writer.WriteEndObject();

            writer.WriteEndObject();
        }

        private void WriteStaticFileArray(JsonWriter writer, string key, string name, List<StaticFile> staticFiles)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("key");
            writer.WriteValue(key);

            writer.WritePropertyName("name");
            writer.WriteValue(name);

            writer.WritePropertyName("value");

            writer.WriteStartArray();

            foreach (var staticFile in staticFiles)
            {
                writer.WriteStartObject();

                writer.WritePropertyName("id");
                writer.WriteValue(staticFile.Id);

                writer.WritePropertyName("name");
                writer.WriteValue(staticFile.Name);

                writer.WriteEndObject();
            }

            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }
}