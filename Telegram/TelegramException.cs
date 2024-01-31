// Copyright (c) 2024 Marco Concas. All rights reserved.
// Licensed under the Apache License.

namespace Telegram
{
    public class TelegramException : Exception
    {
        public TelegramException()
        {
        }

        public TelegramException(string? message) : base(message)
        {
        }

        public TelegramException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}