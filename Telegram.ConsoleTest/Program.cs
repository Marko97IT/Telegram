﻿using Telegram.Configurations;
using Telegram.Enums;

namespace Telegram.ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var telegramUpdatesHandler = new TelegramUpdatesHandler(GetUpdatesWay.Webhook, "6436458381:AAHSxWbCRqwlSbjnQH1vppmtbyl01PkZK_s", new WebhookConfiguration
            {
                Domain = "dev.telegramsdk.net",
                DontSendCertificate = false,
                MaxConnections = 50,
                DropPendingUpdates = true
            });

            Console.CancelKeyPress += (object? sender, ConsoleCancelEventArgs e) =>
            {
                telegramUpdatesHandler.StopReceivingUpdates();
            };
            
            telegramUpdatesHandler.StartReceivingUpdates();
            Console.ReadLine();
        }
    }
}