using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITROI.Clarity.XogClient.Exceptions
{
    /// <summary>
    /// XogClientLoginException
    /// </summary>
    [Serializable]
    class XogClientLoginException : XogClientException
    {
        /// <summary>
        /// XogClientLoginException
        /// </summary>
        /// <param name="message">Exception message</param>
        public XogClientLoginException(string message)
            : base(message)
	    {
	    }
    }
}
