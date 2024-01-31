// Copyright (c) 2024 Marco Concas. All rights reserved.
// Licensed under the Apache License.

using System.Net.Http.Json;
using System.Text.Json;

namespace Telegram.Helpers
{
    internal static class HttpClientExtensions
    {
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

        public static async Task<TelegramResponse<T>?> GetTelegramResponseAsync<T>(this HttpContent content, CancellationToken cancellationToken = default)
        {
            return await content.ReadFromJsonAsync<TelegramResponse<T>>(_jsonSerializerOptions, cancellationToken);
        }

        public static TelegramResponse<T>? GetTelegramResponse<T>(this Stream stream)
        {
            return JsonSerializer.Deserialize<TelegramResponse<T>>(stream, _jsonSerializerOptions);
        }
    }
}