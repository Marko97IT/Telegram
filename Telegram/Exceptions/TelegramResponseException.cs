// Copyright (c) 2024 Marco Concas. All rights reserved.
// Licensed under the Apache License.

namespace Telegram.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a Telegram API call fail.
    /// </summary>
    public class TelegramResponseException : Exception
    {
        /// <summary>
        /// Initialize a new instance of <see cref="TelegramResponseException"/> class with default values.
        /// </summary>
        public TelegramResponseException()
        {

        }

        /// <summary>
        /// Initialize a new instance of <see cref="TelegramResponseException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public TelegramResponseException(string? message) : base(message)
        {

        }

        /// <summary>
        /// Initialize a new instance of <see cref="TelegramResponseException"/> class with a specified error message and inner exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="inner">The exception that is cause of the current exception. If the inner exception is not null, the current exception is raised in a catch block that handles the inner exception.</param>
        public TelegramResponseException(string message, Exception? inner) : base(message, inner)
        {
            
        }
    }
}