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