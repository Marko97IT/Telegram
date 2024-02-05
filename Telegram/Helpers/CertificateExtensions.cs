using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Telegram.Helpers
{
    internal static class CertificateExtensions
    {
        /// <summary>
        /// Get the byte array of a public certificate in PEM format.
        /// </summary>
        /// <param name="certificate">The instance of the certificate as <see cref="X509Certificate2"/> object.</param>
        /// <returns>The byte array of the public certificate in PEM format.</returns>
        public static byte[] GetByteArrayAsPem(this X509Certificate2 certificate)
        {
            var pem = certificate.ExportCertificatePem();
            return Encoding.UTF8.GetBytes(pem);
        }
    }
}