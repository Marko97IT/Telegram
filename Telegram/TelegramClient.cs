// Copyright (c) 2024 Marco Concas. All rights reserved.
// Licensed under the Apache License.

using Telegram.Helpers;
using Telegram.Types;

namespace Telegram
{
    public class TelegramClient : IDisposable
    {
        private const string _telegramBotEndpoint = "https://api.telegram.org/bot";
        private readonly HttpClient _httpClient;
        private readonly CancellationTokenSource _ctsReceiveUpdatesThread;
        private readonly string _botToken;
        private bool _receiveUpdates;

        /// <summary>The event that will be invoked when new updates become available.</summary>
        public event EventHandler<IncomingUpdatesReceivedEventArgs> IncomingUpdatesReceived;

        /// <summary>Provides a class for sending HTTP requests and receiving HTTP responses from Telegram APIs.</summary>
        /// <param name="botToken">The bot token given from BotFather.</param>
        public TelegramClient(string botToken)
        {
            _httpClient = new();
            _ctsReceiveUpdatesThread = new();
            _botToken = botToken;

            ValidateBotToken();
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
            StopReceivingUpdates();
            _ctsReceiveUpdatesThread.Dispose();
            GC.SuppressFinalize(this);
        }

        private void ValidateBotToken()
        {
            using var httpResponse = _httpClient.GetAsync($"{_telegramBotEndpoint}{_botToken}/getMe").Result;
            var telegramResponse = httpResponse.Content.GetTelegramResponseAsync<User>().Result;
            
            if (telegramResponse != null)
            {
                if (!telegramResponse.Ok)
                {
                    throw new TelegramException(telegramResponse.Description);
                }
            }
            else
            {
                httpResponse.EnsureSuccessStatusCode();
            }
        }

        /// <summary>Start receiving incoming updates using long polling in a separate thread.</summary>
        public void StartReceivingUpdates()
        {
            if (!_receiveUpdates)
            {
                _receiveUpdates = true;
                new Thread(async () =>
                {
                    var offset = 0;
                    var httpClient = new HttpClient();

                    while (_receiveUpdates)
                    {
                        try
                        {
                            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{_telegramBotEndpoint}{_botToken}/getUpdates?timeout=50&offset={offset}");
                            using var httpResponse = await httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, _ctsReceiveUpdatesThread.Token);
                            var telegramResponse = await httpResponse.Content.GetTelegramResponseAsync<Update[]>(_ctsReceiveUpdatesThread.Token);

                            if (telegramResponse != null)
                            {
                                if (!telegramResponse.Ok)
                                {
                                    throw new TelegramException(telegramResponse.Description);
                                }
                                else if (telegramResponse.Result?.Length > 0)
                                {
                                    IncomingUpdatesReceived?.Invoke(this, new IncomingUpdatesReceivedEventArgs
                                    {
                                        ReceivedUtcDate = DateTime.UtcNow,
                                        Updates = telegramResponse.Result,
                                    });
                                    offset = telegramResponse.Result.Select(update => update.UpdateId).Max() + 1;
                                }
                            }
                            else
                            {
                                httpResponse.EnsureSuccessStatusCode();
                            }
                        }
                        catch (TaskCanceledException) { }
                    }

                    httpClient.Dispose();
                }).Start();
            }
            else
            {
                throw new InvalidOperationException("There is already an open thread dealing with getting incoming updates.");
            }
        }

        /// <summary>Stop receiving incoming updates using long polling.</summary>
        public void StopReceivingUpdates()
        {
            if (_receiveUpdates)
            {
                _receiveUpdates = false;
                _ctsReceiveUpdatesThread.Cancel();
            }
            else
            {
                throw new InvalidOperationException("There is not an open thread dealing with getting incoming updates.");
            }
        }

        /// <summary>Get basic information about the bot.</summary>
        /// <returns>A <see cref="User"/> object that represent the bot information by <see cref="TelegramResponse{T}.Result"/> field in an asynchronous operation.</returns>
        public async Task<TelegramResponse<User>?> GetMeAsync()
        {
            using var httpResponse = await _httpClient.GetAsync($"{_telegramBotEndpoint}{_botToken}/getMe");
            var telegramResponse = await httpResponse.Content.GetTelegramResponseAsync<User>();
            return telegramResponse;
        }

        /// <summary>Get basic information about the bot.</summary>
        /// <returns>A <see cref="User"/> object that represent the bot information by <see cref="TelegramResponse{T}.Result"/> field in a synchronous operation.</returns>
        public TelegramResponse<User>? GetMe()
        {
            using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"{_telegramBotEndpoint}{_botToken}/getMe");
            using var httpResponse = _httpClient.Send(httpRequestMessage);
            using var telegramResponseStream = httpResponse.Content.ReadAsStream();
            return telegramResponseStream.GetTelegramResponse<User>();
        }
    }
}