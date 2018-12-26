using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITROI.Clarity.XogClient.DTO
{
    /// <summary>
    /// Class with query data parameters
    /// </summary>
    public class GetQueryDataDTO
    {
        /// <summary>
        /// Get/set query name
        /// </summary>
        public String QueryName{ get; set; }

        /// <summary>
        /// Get/set the query slice options
        /// </summary>
        public SliceDTO SliceDTO { get; set; }

        /// <summary>
        /// Get/set the query sort options
        /// </summary>
        public IList<SortDTO> SortDTO { get; set; }

        /// <summary>
        /// Get/set the query filters
        /// </summary>
        public IDictionary<string,string> DictionaryFilters { get; set; }
    }
}
