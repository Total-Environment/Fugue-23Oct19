using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors
{
    /// <summary>
    ///     Request for updating material rate
    /// </summary>
    public class UpdateMaterialRateRequest
    {
        /// <summary>
        /// Gets or sets the material code.
        /// </summary>
        /// <value>
        /// The material code.
        /// </value>
        [Required]
        public string MaterialCode { get; set; }

        /// <summary>
        /// Gets or sets the last purchase rate.
        /// </summary>
        /// <value>
        /// The last purchase rate.
        /// </value>
        [Required]
        public Dictionary<string, object> LastPurchaseRate { get; set; }

        /// <summary>
        /// Gets or sets the weighted average purchase rate.
        /// </summary>
        /// <value>
        /// The weighted average purchase rate.
        /// </value>
        [Required]
        public Dictionary<string, object> WeightedAveragePurchaseRate { get; set; }
    }
}