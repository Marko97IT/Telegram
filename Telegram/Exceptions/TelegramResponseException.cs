// Copyright (c) 2024 Marco Concas. All rights reserved.
// Licensed under the Apache License.

namespace Telegram.Exceptions
{
    public class TelegramResponseException : Exception
    {
        public TelegramResponseException()
        {
        }

        public TelegramResponseException(string? message) : base(message)
        {
        }

        public TelegramResponseException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}