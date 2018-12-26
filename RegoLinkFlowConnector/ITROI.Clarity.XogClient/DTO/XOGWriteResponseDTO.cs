using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITROI.Clarity.XogClient.DTO
{
    /// <summary>
    /// Class with the XOG response parameters
    /// </summary>
    public class XOGWriteResponseDTO
    {
        /// <summary>
        /// Get/set XOG response
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// Get/set XOG response message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Get/set XOG response status
        /// </summary>
        public StatusXog Status { get; set; }
    }
}
