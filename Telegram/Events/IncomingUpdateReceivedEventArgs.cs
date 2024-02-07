// Copyright (c) 2024 Marco Concas. All rights reserved.
// Licensed under the Apache License.

using Telegram.Enums;
using Telegram.Types;

namespace Telegram.Events
{
    /// <summary>
    /// Provides data for the <see cref="TelegramUpdatesHandler.IncomingUpdateReceived"/> event.
    /// </summary>
    public class IncomingUpdateReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// The date when the update has received (UTC).
        /// </summary>
        public DateTime UpdateReceivedDate { get; set; }
        /// <summary>
        /// The way in which the update was received.
        /// </summary>
        public GetUpdatesWay UpdateWay { get; set; }
        /// <summary>
        /// The update as a <see cref="Types.Update"/> object.
        /// </summary>
        public Update Update { get; set; }
    }
}