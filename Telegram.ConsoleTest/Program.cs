using Telegram.Types;

namespace Telegram.ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var telegramClient = new TelegramClient("6436458381:AAHSxWbCRqwlSbjnQH1vppmtbyl01PkZK_s");
            telegramClient.ReceiveUpdates().Start();
            telegramClient.NewUpdatesReceived += ((object? sender, Update[] e) =>
            {
                var a = 1;
            });
            Console.ReadKey();
            telegramClient.Dispose();
            Console.ReadKey();
        }
    }
}