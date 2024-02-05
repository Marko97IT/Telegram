// Copyright (c) 2024 Marco Concas. All rights reserved.
// Licensed under the Apache License.

using Telegram.Enums;
using Telegram.Types;

namespace Telegram.Events
{
    public class IncomingUpdateReceivedEventArgs : EventArgs
    {
        public DateTime UpdateReceivedDate { get; set; }
        public GetUpdatesWay UpdateWay { get; set; }
        public Update Update { get; set; }
    }
}