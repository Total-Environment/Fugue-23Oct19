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
    /// Class MaterialDto.
    /// </summary>
    /// <seealso cref="IJsonSerializable"/>
    public class MaterialDto : IJsonSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialDto"/> class.
        /// </summary>
        /// <param name="material">The material data.</param>
        public MaterialDto(IMaterial material)
        {
            Material = material;
        }

        /// <summary>
        /// Gets or sets the material data.
        /// </summary>
        /// <value>The material data.</value>
        public IMaterial Material { get; set; }

        /// <summary>
        /// To the json.
        /// </summary>
        /// <returns>IDictionary&lt;System.String, System.Object&gt;.</returns>
        public IDictionary<string, object> ToJson()
        {
            var dictionary = Material.Headers.ToDictionary(h => h.Name,
                h => (object)h.Columns.ToDictionary(c => c.Name, c => RenderValue(c.Value)));
            dictionary["id"] = Material.Id;
            dictionary["group"] = Material.Group;

            //ConvertBrandsToJson();
            var autogeneratedColumns = Material.ComponentDefinition.Headers.SelectMany(
                h =>
                    h.Columns.Where(c => c.DataType is AutogeneratedDataType)
                        .Select(
                            c =>
                                new Tuple<string, string, AutogeneratedDataType>(h.Name, c.Name,
                                    (AutogeneratedDataType)c.DataType)));

            foreach (var column in autogeneratedColumns)
                ((IDictionary<string, object>)dictionary[column.Item1])[column.Item2] =
                    column.Item3.Resolve(Material);

            return dictionary;
        }

        private static object RenderValue(object value)
        {
            if (value is Brand)
            {
                return new BrandDataDto
                {
                    Columns =
                        ((Brand)value).Columns.Select(c => new ColumnDto { Key = c.Key, Name = c.Name, Value = c.Value })
                };
            }
            if (value is object[])
                return ((object[])value).Select(RenderValue).ToArray();
            if (value is CheckListValue)
            {
                var uiRoot = ConfigurationManager.AppSettings["ComponentLibraryUIRoot"];
                var id = ((CheckListValue)value).Id;
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
    }
}