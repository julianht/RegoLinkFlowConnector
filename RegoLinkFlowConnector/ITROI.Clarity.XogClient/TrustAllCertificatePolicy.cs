using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace ITROI.Clarity.XogClient
{
    /// <summary>
    /// TrustAllCertificatePolicy class
    /// </summary>
    public static class TrustAllCertificatePolicy 
    {
        /// <summary>
        /// Validate server certificate
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="certificate">X509Certificate certificate</param>
        /// <param name="chain">X509Chain chain</param>
        /// <param name="sslPolicyErrors">SslPolicyErrors</param>
        /// <returns>true: certificate ok, false: certificate no ok</returns>
         public static bool ValidateServerCertificate(object sender, 
                                                      X509Certificate certificate, 
                                                      X509Chain chain, 
                                                      SslPolicyErrors sslPolicyErrors)
         {
             return true;
         }
    }
}