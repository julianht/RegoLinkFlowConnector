using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITROI.Clarity.XogClient.Exceptions
{
    /// <summary>
    /// XogClientFailureException
    /// </summary>
    [Serializable]
    public class XogClientFailureException : XogClientException
    {
        /// <summary>
        /// XogClientFailureException
        /// </summary>
        /// <param name="message">Exception message</param>
        public XogClientFailureException(string message)
            : base(message)
        {
        }
    }
}
