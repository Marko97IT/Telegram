using Telegram.Helpers;
using Telegram.Types;

namespace Telegram
{
    /// <summary>
    /// Provides a class for sending HTTP requests and receiving HTTP responses from Telegram APIs.
    /// </summary>
    /// <param name="botToken">The bot token given from BotFather.</param>
    public class TelegramClient : IDisposable
    {
        public event EventHandler<Update[]> NewUpdatesReceived;

        private const string TELEGRAM_BOT_ENDPOINT = "https://api.telegram.org/bot";
        private readonly HttpClient _httpClient;
        private readonly CancellationTokenSource _ctsReceiveUpdatesThread;
        private readonly string _botToken;
        private bool _receiveUpdates;

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
            var httpResponse = _httpClient.GetAsync($"{TELEGRAM_BOT_ENDPOINT}{_botToken}/getMe").Result;
            var telegramResponse = httpResponse.Content.ReadResponseFromTelegramAsync<User>().Result;
            
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

        /// <summary>
        /// Start receiving incoming updates using long polling in a separate thread.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="TelegramException"></exception>
        public Thread ReceiveUpdates()
        {
            var offset = 0;
            var httpClient = new HttpClient();
            var thread = new Thread(async () =>
            {
                _receiveUpdates = true;
                while (_receiveUpdates)
                {
                    try
                    {
                        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{TELEGRAM_BOT_ENDPOINT}{_botToken}/getUpdates?timeout=50&offset={offset}");
                        using var httpResponse = await httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, _ctsReceiveUpdatesThread.Token);
                        var telegramResponse = await httpResponse.Content.ReadResponseFromTelegramAsync<Update[]>(_ctsReceiveUpdatesThread.Token);

                        if (telegramResponse != null)
                        {
                            if (!telegramResponse.Ok)
                            {
                                throw new TelegramException(telegramResponse.Description);
                            }
                            else if (telegramResponse.Result?.Length > 0)
                            {
                                NewUpdatesReceived?.Invoke(this, telegramResponse.Result);
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
            });

            return thread;
        }

        /// <summary>
        /// Stop receiving incoming updates using long polling.
        /// </summary>
        public void StopReceivingUpdates()
        {
            _receiveUpdates = false;
            _ctsReceiveUpdatesThread.Cancel();
        }
    }
}