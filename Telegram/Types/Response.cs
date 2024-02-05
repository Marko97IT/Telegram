// Copyright (c) 2024 Marco Concas. All rights reserved.
// Licensed under the Apache License.

namespace Telegram.Types
{
    internal class Response<T>
    {
        public bool Ok { get; set; }
        public int? ErrorCode { get; set; }
        public string? Description { get; set; }
        public T? Result { get; set; }
    }
}