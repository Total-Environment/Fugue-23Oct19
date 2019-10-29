using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors
{
    /// <summary>
    /// 
    /// </summary>
    public class CompositeComponentDocumentDtoAdaptor
    {
        /// <summary>
        /// Gets material document from the composite component.
        /// </summary>
        /// <param name="compositeComponent"></param>
        /// <param name="columnKey">The column key.</param>
        /// <returns></returns>
        public static CompositeComponentDocumentDto FromCompositeComponent(CompositeComponent compositeComponent, string columnKey)
        {
            var type = compositeComponent.CompositeComponentDefinition.Code;
            if (String.IsNullOrEmpty(type))
            {
                throw new Exception("Invalid Composite Component Type / Code.");
            }
            if (type.ToLower() == "pkg")
            {
                type = "package";
            }
            var general = compositeComponent.Headers.FirstOrDefault(h => h.Key == "general");
            string code = "", shortDescription = "";
            var codeDetails = general?.Columns.FirstOrDefault(c => c.Key == $"{type.ToLower()}_code");
            if (codeDetails != null)
                code = (string)codeDetails.Value;
            var shortDescriptionDetails = general?.Columns.FirstOrDefault(c => c.Key == "short_description");
            if (shortDescriptionDetails != null)
                shortDescription = (string)shortDescriptionDetails?.Value;
            return new CompositeComponentDocumentDto()
            {
                Code = code,
                ShortDescription = shortDescription,
                DocumentDtos = GetDocumentsFromColumn(compositeComponent, columnKey)
            };
        }

        private static List<DocumentDto> GetDocumentsFromColumn(CompositeComponent compositeComponent, string columnKey)
        {
            var documentColumn = compositeComponent.Headers.SelectMany(h => h.Columns).FirstOrDefault(c => c.Key == columnKey);
            var columnDefinition = GetColumnDefinition(compositeComponent, columnKey);
            if (columnDefinition == null)
                throw new ArgumentException($"{columnKey} does not exist in definition.");
            if (documentColumn == null)
                return new List<DocumentDto>();
            if (columnDefinition.DataType is ArrayDataType)
                return
                    ((object[])documentColumn.Value).Select(c => GetDocument(c, columnDefinition))
                    .Where(d => d != null)
                    .ToList();
            return new List<DocumentDto> { GetDocument(documentColumn.Value, columnDefinition) };
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

        private static IColumnDefinition GetColumnDefinition(CompositeComponent compositeComponent, string columnKey)
        {
            return 
                compositeComponent.CompositeComponentDefinition.Headers.SelectMany(h => h.Columns).FirstOrDefault(c => c.Key == columnKey);
        }
    }
}