using System.Collections.Generic;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerPort
{
    /// <summary>
    /// Builk rate edit response.
    /// </summary>
    public class BuildRateResponse<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildRateResponse"/> class.
        /// </summary>
        public BuildRateResponse()
        {
            Records = new List<BuildRateResponseRecord<T>>();
        }

        /// <summary>
        /// Status: Can be Succeeded, PartiallySucceeded or Failed
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets the data of the response.
        /// </summary>
        /// <value>
        /// The Data.
        /// </value>
        public List<BuildRateResponseRecord<T>> Records { get; private set; }
    }
}