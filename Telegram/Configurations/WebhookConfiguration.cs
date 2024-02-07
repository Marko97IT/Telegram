// Copyright (c) 2024 Marco Concas. All rights reserved.
// Licensed under the Apache License.

using System.Security.Cryptography.X509Certificates;

namespace Telegram.Configurations
{
    /// <summary>
    /// Provides a class for setting webhook informations.
    /// </summary>
    public class WebhookConfiguration
    {
        /// <summary>
        /// The port used to serve the local server with HTTP protocol.
        /// </summary>
        public int? HttpPort { get; set; }
        /// <summary>
        /// The port used to serve the local server with HTTPS protocol.
        /// </summary>
        public int? HttpsPort { get; set; }
        /// <summary>
        /// The domain without protocol prefix and without paths that will be used by Telegram to communicate incoming updates.
        /// </summary>
        public required string Domain { get; set; }
        /// <summary>
        /// The certificate in PKCS12 format as <see cref="X509Certificate2"/> object that will used by the local server. If is not specified, a self signed certificate will used.
        /// </summary>
        public X509Certificate2? Certificate { get; set; }
        /// <summary>
        /// Send the public certificate in PEM format to Telegram to verify if it's valid. You should set it to true if you don't use a trusted certificate.
        /// </summary>
        public bool? SendPublicCertificateToTelegram { get; set; } = false;
        /// <summary>
        /// The maximum allowed number of simultaneous HTTPS connections to the webhook for update delivery (min 1, max 100). If not specified the value is 40.
        /// </summary>
        public int? MaxConnections { get; set; }
        /// <summary>
        /// This will ignore previously updates that hasn't processed.
        /// </summary>
        public bool? DropPendingUpdates { get; set; }
    }
}