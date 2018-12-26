using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Security.Cryptography;
using System.IO;

namespace ITROI.Clarity.XogClient
{
    public class XogBase
    {
        /// <summary>
        /// Url
        /// </summary>
        protected string _url;

        //private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(XogBase));

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="url">cA PPM url</param>
        public XogBase(string url)
        {
            _url = url;
        }

        /// <summary>
        /// Gets a new WebProxy instance
        /// </summary>
        /// <returns>WebProxy object</returns>
        protected WebProxy GetWebProxy()
        {
            string proxyUrl = ConfigurationManager.AppSettings["ecBossProxyUrl"];

            if (string.IsNullOrEmpty(proxyUrl))
            {
                return null;
            }

            this.WriteLog("Start GetWebProxy");

            WebProxy proxy;

            string proxyPort = ConfigurationManager.AppSettings["ecBossProxyPort"];

            if (string.IsNullOrEmpty(proxyPort))
            {
                proxy = new WebProxy(proxyUrl);
            }
            else
            {
                proxy = new WebProxy(proxyUrl.Replace("http://", ""), Convert.ToInt32(proxyPort));
                this.WriteLog("Proxy port: " + proxyPort);
            }

            string byPassOnLocal = ConfigurationManager.AppSettings["ecBossProxyByPassOnLocal"];

            if (!string.IsNullOrEmpty(byPassOnLocal))
            {
                proxy.BypassProxyOnLocal = byPassOnLocal == "1";
                this.WriteLog("Proxy BypassProxyOnLocal: " + proxy.BypassProxyOnLocal);
            }

            string useDefaultCredentials = ConfigurationManager.AppSettings["ecBossProxyUseDefaultCredentials"];

            if (!string.IsNullOrEmpty(useDefaultCredentials))
            {
                proxy.UseDefaultCredentials = useDefaultCredentials == "1";
                this.WriteLog("Proxy UseDefaultCredentials: " + proxy.UseDefaultCredentials);
            }

            string proxyUsername = ConfigurationManager.AppSettings["ecBossProxyUsername"],
                   proxyPassword = ConfigurationManager.AppSettings["ecBossProxyPassword"];

            if (!string.IsNullOrEmpty(proxyUsername) &&
                !string.IsNullOrEmpty(proxyPassword))
            {
                this.WriteLog("Proxy with credentials");

                proxyUsername = this.DecryptText(proxyUsername);
                proxyPassword = this.DecryptText(proxyPassword);

                proxy.Credentials = new NetworkCredential(proxyUsername,
                                                          proxyPassword);
            }

            string bypassURL = ConfigurationManager.AppSettings["ecBossProxyBypassURL"];

            if (!string.IsNullOrEmpty(bypassURL) &&
                bypassURL == "1")
            {
                proxy.BypassList = new string[] { _url };
                this.WriteLog("CA PPM url included in BypassList: " + _url);
            }

            return proxy;
        }

        /// <summary>
        /// Decrypts text using AES
        /// </summary>
        /// <param name="text">text to decrypt</param>
        /// <returns>Decrypted text</returns>
        private string DecryptText(string text)
        {
            try
            {
                string encryptKey = "1TR01SoluTion$1n";
                byte[] key = Encoding.UTF8.GetBytes(encryptKey),
                       iv = Encoding.UTF8.GetBytes(encryptKey);

                using (var rijAlg = new RijndaelManaged())
                {
                    rijAlg.Mode = CipherMode.CBC;
                    rijAlg.Padding = PaddingMode.PKCS7;
                    rijAlg.FeedbackSize = 128;

                    rijAlg.Key = key;
                    rijAlg.IV = iv;

                    var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                    using (var msDecrypt = new MemoryStream(Convert.FromBase64String(text)))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                text = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }

                return text;
            }
            catch (Exception ex)
            {
                this.WriteLogError(ex.Message);
                return text;
            }
        }

        /// <summary>
        /// Writes the message in the log
        /// </summary>
        /// <param name="message">Message to write in the log</param>
        protected void WriteLog(string message)
        {
            //log.Info(message);
        }

        /// <summary>
        /// Writes the message in the log
        /// </summary>
        /// <param name="message">Message to write in the log</param>
        protected void WriteLogError(string message)
        {
            //log.Error(message);
        }
    }
}
