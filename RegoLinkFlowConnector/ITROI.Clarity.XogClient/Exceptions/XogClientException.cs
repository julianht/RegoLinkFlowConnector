using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITROI.Clarity.XogClient.Exceptions
{
    /// <summary>
    /// XogClientException
    /// </summary>
    [Serializable]
    public class XogClientException : Exception
    {
        /// <summary>
        /// XogClientException
        /// </summary>
        /// <param name="message">Exception message</param>
        public XogClientException(string message)
            : base(message)
        {
        }
    }

}
