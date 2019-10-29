using System.Collections.Generic;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerPort
{
    /// <summary>
    ///Represents a response record for bulk edit respone.
    /// </summary>
    public class BuildRateResponseRecord<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildRateResponseRecord"/> class.
        /// </summary>
        public BuildRateResponseRecord()
        {
            RecordData = new List<T>();
        }

        /// <summary>
        /// Gets or sets the message related to the record.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets the material rate dto data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public List<T> RecordData { get; private set; }

        /// <summary>
        /// Gets or sets the record status. Succeded or failed.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string Status { get; set; }
    }
}