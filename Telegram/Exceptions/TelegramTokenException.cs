// Copyright (c) 2024 Marco Concas. All rights reserved.
// Licensed under the Apache License.

namespace Telegram.Exceptions
{
    public class TelegramTokenException : Exception
    {
        public TelegramTokenException()
        {
        }

        public TelegramTokenException(string? message) : base(message)
        {
        }

        public TelegramTokenException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}