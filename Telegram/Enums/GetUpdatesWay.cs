// Copyright (c) 2024 Marco Concas. All rights reserved.
// Licensed under the Apache License.

namespace Telegram.Enums
{
    /// <summary>
    /// The way incoming updates from Telegram are handled.
    /// </summary>
    public enum GetUpdatesWay
    {
        /// <summary>
        /// Using the HTTP polling.
        /// </summary>
        Polling,
        /// <summary>
        /// Using a webhook.
        /// </summary>
        Webhook
    }
}