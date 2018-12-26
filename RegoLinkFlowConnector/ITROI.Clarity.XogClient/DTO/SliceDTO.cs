using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITROI.Clarity.XogClient.DTO
{
    /// <summary>
    /// Class with query data slice parameters
    /// </summary>
    public class SliceDTO
    {
        /// <summary>
        /// Get/set page number
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Get/set number of records
        /// </summary>
        public int Size { get; set; }
    }
}
