// Copyright (c) 2024 Marco Concas. All rights reserved.
// Licensed under the Apache License.

using System.Text.Json;
using System.Text.Json.Serialization;
using Telegram.Exceptions;
using Telegram.Helpers;
using Telegram.Types;

namespace Telegram
{
    public partial class TelegramClient : IDisposable
    {
        private const string _telegramBotEndpoint = "https://api.telegram.org/bot";
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        private readonly string _botToken;

        public TelegramClient(string botToken)
        {
            _httpClient = new();
            _botToken = botToken;

            // Call it to check if the token is valid
            _ = GetMe();
        }

        public async Task<User?> GetMeAsync(CancellationToken cancellationToken = default)
        {
            using var httpResponse = await _httpClient.GetAsync($"{_telegramBotEndpoint}{_botToken}/getMe", cancellationToken);
            var telegramResponse = await httpResponse.Content.ReadFromJsonAsync<Response<User>>(_jsonSerializerOptions, cancellationToken);

            if (telegramResponse != null)
            {
                if (telegramResponse.Ok)
                {
                    return telegramResponse.Result;
                }
                else
                {
                    throw new TelegramResponseException($"{telegramResponse.ErrorCode} {telegramResponse.Description}");
                }
            }
            else
            {
                httpResponse.EnsureSuccessStatusCode();
            }

            return null;
        }

        public User? GetMe()
        {
            using var httpResponse = _httpClient.Get($"{_telegramBotEndpoint}{_botToken}/getMe");
            var telegramResponse = httpResponse.Content.ReadFromJson<Response<User>>(_jsonSerializerOptions);

            if (telegramResponse != null)
            {
                if (telegramResponse.Ok)
                {
                    return telegramResponse.Result;
                }
                else
                {
                    throw new TelegramResponseException($"{telegramResponse.ErrorCode} {telegramResponse.Description}");
                }
            }
            else
            {
                httpResponse.EnsureSuccessStatusCode();
            }

            return null;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}