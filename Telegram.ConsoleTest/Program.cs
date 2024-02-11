using Telegram.Configurations;
using Telegram.Enums;
using Telegram.Events;

namespace Telegram.ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var botToken = "6436458381:AAF1hk46kEAJnZo8L1aYDsmnY7EXDDa-2_I";
            var webhookConfiguration = new WebhookConfiguration
            {
                Domain = "example.com"
            };
            using var handler = new TelegramUpdatesHandler(GetUpdatesWay.Webhook, botToken, webhookConfiguration);

            handler.IncomingUpdateReceived += (object? sender, IncomingUpdateReceivedEventArgs e) =>
            {
                var update = e.Update;
            };

            handler.IncomingUpdateReceived += (object? sender, IncomingUpdateReceivedEventArgs e) =>
            {
                var update = e.Update;
            };

            Console.CancelKeyPress += (object? sender, ConsoleCancelEventArgs e) =>
            {
                handler.StopReceivingUpdates();
            };
            
            handler.StartReceivingUpdates();
            Task.Delay(3000).Wait();
            handler.Dispose();
            Console.ReadLine();
        }
    }
}