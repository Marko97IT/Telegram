// Copyright (c) 2024 Marco Concas. All rights reserved.
// Licensed under the Apache License.

using Telegram.Types;

namespace Telegram
{
    /// <summary>
    /// Provides data for incoming updates from Telegram.
    /// </summary>
    public class IncomingUpdatesReceivedEventArgs : EventArgs
    {
        public DateTime ReceivedUtcDate { get; set; }
        public Update[] Updates { get; set; }
    }
}