using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITROI.Clarity.XogClient
{
    /// <summary>
    /// Class of constant handling XOG
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// Query columns name
        /// </summary>
        public static string QUERY_COLUMNS = "spwp_query_columns";
    }

    /// <summary>
    /// States result XOG
    /// </summary>
    public enum StatusXog
    {
        /// <summary>
        /// Failure result
        /// </summary>
        FAILURE = 0,

        /// <summary>
        /// Success result
        /// </summary>
        SUCCESS = 1
    }
}