using System;
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
    /// The Adapter for Material Documents
    /// </summary>
    public class MaterialDocumentDtoAdaptor
    {
        /// <summary>
        /// Gets material document from the material.
        /// </summary>
        /// <param name="material">The material.</param>
        /// <param name="columnKey">The column key.</param>
        /// <returns></returns>
        public static MaterialDocumentDto FromMaterial(IMaterial material, string columnKey)
        {
            return new MaterialDocumentDto()
            {
                MaterialCode = (string)((dynamic) material).general.material_code.Value,
                ShortDescription = (string)((dynamic) material).general.short_description.Value,
                DocumentDtos = GetDocumentsFromColumn(material, columnKey)
            };
        }

        private static List<DocumentDto> GetDocumentsFromColumn(IMaterial material, string columnKey)
        {
            var documentColumn = material.Headers.SelectMany(h => h.Columns).FirstOrDefault(c => c.Key == columnKey);
            var columnDefinition = GetColumnDefinition(material, columnKey);
            if (columnDefinition == null)
                throw new ArgumentException($"{columnKey} does not exist in definition.");
            if (documentColumn == null)
                return new List<DocumentDto>();
            if (columnDefinition.DataType is ArrayDataType)
                return
                    ((object[]) documentColumn.Value).Select(c => GetDocument(c, columnDefinition))
                    .Where(d => d != null)
                    .ToList();
            return new List<DocumentDto> {GetDocument(documentColumn.Value, columnDefinition)};
        }

        private static DocumentDto GetDocument(object columnValue, IColumnDefinition columnDefinition)
        {

            if (columnDefinition.DataType is ArrayDataType)
            {

                if (((ArrayDataType)columnDefinition.DataType).DataType is StaticFileDataType)
                {
                    return GetStaticFile(columnValue);
                }
            }
            if (columnDefinition.DataType is StaticFileDataType)
            {
                return GetStaticFile(columnValue);
            }
            if (columnDefinition.DataType is CheckListDataType)
            {
                return GetChecklist(columnValue);
            }

            return null;
        }

        private static DocumentDto GetStaticFile(object columnValue)
        {
            var staticFile = columnValue as StaticFile;
            if (staticFile == null) return null;
            var urlRoot = ConfigurationManager.AppSettings["CdnBaseUrl"];
            return new DocumentDto()
            {
                Name = staticFile.Name,
                Id = staticFile.Id,
                Url = $"{urlRoot}/static-files/{staticFile.Name}"
            };
        }

        private static DocumentDto GetChecklist(object columnValue)
        {
            var checklist = columnValue as CheckListValue;
            if (checklist == null) return null;
            var urlRoot = ConfigurationManager.AppSettings["ComponentLibraryUIRoot"];
            return new DocumentDto()
            {
                Id = checklist.Id,
                Name = $"/check-lists/{checklist.Id}",
                Url = $"{urlRoot}/check-lists/{checklist.Id}"
            };
        }

        private static IColumnDefinition GetColumnDefinition(IMaterial material, string columnKey)
        {
            return
                material.ComponentDefinition.Headers.SelectMany(h => h.Columns).FirstOrDefault(c => c.Key == columnKey);
        }
    }
}