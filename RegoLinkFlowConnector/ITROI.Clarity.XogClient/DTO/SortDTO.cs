using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITROI.Clarity.XogClient.DTO
{
    /// <summary>
    /// Class with query data sorting
    /// </summary>
    public class SortDTO
    {
        /// <summary>
        /// Get/set sorting field
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get/set sorting direction (asc, desc)
        /// </summary>
        public string Direction { get; set; }
    }
}
