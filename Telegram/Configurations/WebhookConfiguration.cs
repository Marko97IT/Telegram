// Copyright (c) 2024 Marco Concas. All rights reserved.
// Licensed under the Apache License.

namespace Telegram.Configurations
{
    /// <summary>
    /// Provides a class for setting webhook informations.
    /// </summary>
    public class WebhookConfiguration
    {
        /// <summary>
        /// The domain without protocol prefix and without paths that will be used by Telegram to communicate incoming updates. It can be also an IP address.
        /// </summary>
        public required string Domain { get; set; }
        /// <summary>
        /// The port used to serve the local server with HTTPS protocol. The default value is <b>443</b>.
        /// </summary>
        public int HttpsPort { get; set; } = 443;
        /// <summary>
        /// The maximum allowed number of simultaneous HTTPS connections to the webhook for update delivery (min 1, max 100). If not specified the value is <b>40</b>.
        /// </summary>
        public int? MaxConnections { get; set; }
        /// <summary>
        /// This will ignore previously updates that hasn't processed.
        /// </summary>
        public bool? DropPendingUpdates { get; set; }

        /// <summary>
        /// Cache the host domain from <see cref="Domain"/>.
        /// </summary>
        private string _host;

        /// <summary>
        /// Parse the <see cref="Domain"/> field into the host domain.
        /// </summary>
        /// <returns>The domain host.</returns>
        internal string GetHostDomain()
        {
            if (_host == null)
            {
                var uriBuilder = new UriBuilder(Domain);
                _host = uriBuilder.Host;
                return _host;
            }
            else
            {
                return _host;
            }
        }
    }
}