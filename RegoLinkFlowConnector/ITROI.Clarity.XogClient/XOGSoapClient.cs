using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Messaging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace ITROI.Clarity.XogClient
{
    public class XOGSoapClient : SoapClient
    {
        public XOGSoapClient(Uri destination)
            : base(destination)
        {
            string ecBossXOGTimeout = ConfigurationManager.AppSettings["ecBossXOGTimeout"];

            if (string.IsNullOrEmpty(ecBossXOGTimeout))
            {
                //1 hour
                base.Timeout = 3600000;
            }
            else
            {
                base.Timeout = Convert.ToInt32(ecBossXOGTimeout);
            }
        }

        [SoapMethod("DoSoapRequest")]
        public SoapEnvelope DoSoapRequest(SoapEnvelope envelope)
        {
            return base.SendRequestResponse("RequestResponseMethod", envelope);
        }
    }
}
