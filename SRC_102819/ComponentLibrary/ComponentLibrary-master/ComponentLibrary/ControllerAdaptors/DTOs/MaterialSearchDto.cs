using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs
{
    /// <summary>
    ///     Class MaterialSearchDto.
    /// </summary>
    public class MaterialSearchDto
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MaterialSearchDto" /> class.
        /// </summary>
        /// <param name="material">The material data.</param>
        public MaterialSearchDto(IMaterial material)
        {
            Material = material;
        }

        private IMaterial Material { get; }

        /// <summary>
        ///     Gets the material classification level1.
        /// </summary>
        /// <value>The material classification level1.</value>
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public object MaterialClassificationLevel1 => Material["Classification"]["Material Level 1"];

        /// <summary>
        ///     Gets the material classification level2.
        /// </summary>
        /// <value>The material classification level2.</value>
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public object MaterialClassificationLevel2 => Material["Classification"]["Material Level 2"];

        /// <summary>
        ///     Gets the material classification level3.
        /// </summary>
        /// <value>The material classification level3.</value>
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public object MaterialClassificationLevel3 => Material["Classification"]["Material Level 3"];

        /// <summary>
        ///     Gets the material classification level4.
        /// </summary>
        /// <value>The material classification level4.</value>
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public object MaterialClassificationLevel4 => Material["Classification"]["Material Level 4"];

        /// <summary>
        ///     Gets the material classification level5.
        /// </summary>
        /// <value>The material classification level5.</value>
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public object MaterialClassificationLevel5 => Material["Classification"]["Material Level 5"];

        /// <summary>
        ///     Gets the material classification level6.
        /// </summary>
        /// <value>The material classification level6.</value>
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public object MaterialClassificationLevel6 => Material["Classification"]["Material Level 6"];

        /// <summary>
        ///     Gets the material classification level7.
        /// </summary>
        /// <value>The material classification level7.</value>
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public object MaterialClassificationLevel7 => Material["Classification"]["Material Level 7"];

        /// <summary>
        ///     Gets the name of the material.
        /// </summary>
        /// <value>The name of the material.</value>
        public string MaterialName => Convert.ToString(Material["General"]["Material Name"]);

        /// <summary>
        ///     Gets the material code.
        /// </summary>
        /// <value>The material code.</value>
        public string MaterialCode => Material.Id;

        /// <summary>
        ///     Gets the short description.
        /// </summary>
        /// <value>The short description.</value>
        public string ShortDescription => Convert.ToString(Material["General"]["Short Description"]);

        /// <summary>
        ///     Gets the shade no.
        /// </summary>
        /// <value>The shade no.</value>
        public string ShadeNo => Convert.ToString(Material["General"]["Shade Number"]);

        /// <summary>
        ///     Gets the shade description.
        /// </summary>
        /// <value>The shade description.</value>
        public string ShadeDescription => Convert.ToString(Material["General"]["Shade Description"]);

        /// <summary>
        ///     Gets the image.
        /// </summary>
        /// <value>The image.</value>
        public object Image => Material["General"]["Image"];

        /// <summary>
        ///     Gets the material status.
        /// </summary>
        /// <value>The material status.</value>
        public string MaterialStatus => Convert.ToString(Material["General"]["Material Status"]);

        /// <summary>
        ///     Gets the specifications.
        /// </summary>
        /// <value>The specifications.</value>
        public Dictionary<string, object> Specifications
        {
            get { return Material["Specifications"].Columns.ToDictionary(x => x.Name, x => x.Value); }
        }
    }
}