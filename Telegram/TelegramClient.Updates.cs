// Copyright (c) 2024 Marco Concas. All rights reserved.
// Licensed under the Apache License.

using System.Net;
using System.Text.Json;
using System.Text;
using Telegram.Exceptions;
using Telegram.Helpers;
using Telegram.Types;

namespace Telegram
{
    public partial class TelegramClient
    {
        public async Task<Update[]?> GetUpdatesAsync(int? offset = null, int? limit = null, int? timeout = null, string[]? allowedUpdates = null, CancellationToken cancellationToken = default)
        {
            var data = new Dictionary<string, object?>
            {
                { nameof(offset), offset },
                { nameof(limit), limit },
                { nameof(timeout), timeout },
                { nameof(allowedUpdates).ToSnakeCaseLower(), allowedUpdates }
            };
            using var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, data, cancellationToken: cancellationToken);
            using var content = new StringContent(stream.ReadString(), Encoding.UTF8, "application/json");

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_telegramBotEndpoint}{_botToken}/getUpdates") { Content = content };
            using var httpResponse = await _httpClient.SendAsync(httpRequest, cancellationToken);
            var telegramResponse = await httpResponse.Content.ReadFromJsonAsync<Response<Update[]>>(_jsonSerializerOptions, cancellationToken);

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

        public Update[]? GetUpdates(int? offset = null, int? limit = null, int? timeout = null, string[]? allowedUpdates = null)
        {
            var data = new Dictionary<string, object?>
            {
                { nameof(offset), offset },
                { nameof(limit), limit },
                { nameof(timeout), timeout },
                { nameof(allowedUpdates).ToSnakeCaseLower(), allowedUpdates }
            };
            using var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_telegramBotEndpoint}{_botToken}/getUpdates") { Content = content };
            using var httpResponse = _httpClient.Send(httpRequest);
            var telegramResponse = httpResponse.Content.ReadFromJson<Response<Update[]>>(_jsonSerializerOptions);

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

        public async Task SetWebhookAsync(string url, byte[]? certificate = null, string? ipAddress = null, int? maxConnections = null, string[]? allowedUpdates = null, bool? dropPendingUpdates = null, string? secretToken = null, CancellationToken cancellationToken = default)
        {
            using var form = new MultipartFormDataContent
            {
                { new StringContent(url), nameof(url) }
            };

            if (certificate != null)
            {
                form.Add(new ByteArrayContent(certificate), nameof(certificate), "certificate.pem");
            }

            if (IPAddress.TryParse(ipAddress, out _))
            {
                form.Add(new StringContent(ipAddress), nameof(ipAddress).ToSnakeCaseLower());
            }

            if (maxConnections != null && maxConnections.Value > 0 && maxConnections.Value <= 40)
            {
                form.Add(new StringContent(maxConnections.ToString()!), nameof(ipAddress).ToSnakeCaseLower());
            }

            if (allowedUpdates != null && allowedUpdates.Length > 0)
            {
                using var stream = new MemoryStream();
                await JsonSerializer.SerializeAsync(stream, allowedUpdates, cancellationToken: cancellationToken);
                form.Add(new StringContent(Encoding.UTF8.GetString(stream.ToArray())), nameof(allowedUpdates).ToSnakeCaseLower());
            }

            if (dropPendingUpdates.HasValue && dropPendingUpdates.Value)
            {
                form.Add(new StringContent("true"), nameof(dropPendingUpdates).ToSnakeCaseLower());
            }

            if (!string.IsNullOrEmpty(secretToken))
            {
                form.Add(new StringContent(secretToken), nameof(secretToken).ToSnakeCaseLower());
            }

            using var httpResponse = await _httpClient.PostAsync($"{_telegramBotEndpoint}{_botToken}/setWebhook", form, cancellationToken);
            var telegramResponse = await httpResponse.Content.ReadFromJsonAsync<Response<object>>(_jsonSerializerOptions, cancellationToken);

            if (telegramResponse != null)
            {
                if (!telegramResponse.Ok)
                {
                    throw new TelegramResponseException($"{telegramResponse.ErrorCode} {telegramResponse.Description}");
                }
            }
            else
            {
                httpResponse.EnsureSuccessStatusCode();
            }
        }

        public void SetWebhook(string url, byte[]? certificate = null, string? ipAddress = null, int? maxConnections = null, string[]? allowedUpdates = null, bool? dropPendingUpdates = null, string? secretToken = null)
        {
            using var form = new MultipartFormDataContent
            {
                { new StringContent(url), nameof(url) }
            };

            if (certificate != null)
            {
                form.Add(new ByteArrayContent(certificate), nameof(certificate), "certificate.pem");
            }

            if (IPAddress.TryParse(ipAddress, out _))
            {
                form.Add(new StringContent(ipAddress), nameof(ipAddress).ToSnakeCaseLower());
            }

            if (maxConnections != null && maxConnections.Value > 0 && maxConnections.Value <= 40)
            {
                form.Add(new StringContent(maxConnections.ToString()!), nameof(ipAddress).ToSnakeCaseLower());
            }

            if (allowedUpdates != null && allowedUpdates.Length > 0)
            {
                using var stream = new MemoryStream();
                JsonSerializer.Serialize(stream, allowedUpdates);
                form.Add(new StringContent(Encoding.UTF8.GetString(stream.ToArray())), nameof(allowedUpdates).ToSnakeCaseLower());
            }

            if (dropPendingUpdates.HasValue && dropPendingUpdates.Value)
            {
                form.Add(new StringContent("true"), nameof(dropPendingUpdates).ToSnakeCaseLower());
            }

            if (!string.IsNullOrEmpty(secretToken))
            {
                form.Add(new StringContent(secretToken), nameof(secretToken).ToSnakeCaseLower());
            }

            using var httpResponse = _httpClient.Post($"{_telegramBotEndpoint}{_botToken}/setWebhook", form);
            var telegramResponse = httpResponse.Content.ReadFromJson<Response<object>>(_jsonSerializerOptions);

            if (telegramResponse != null)
            {
                if (!telegramResponse.Ok)
                {
                    throw new TelegramResponseException($"{telegramResponse.ErrorCode} {telegramResponse.Description}");
                }
            }
            else
            {
                httpResponse.EnsureSuccessStatusCode();
            }
        }

        public async Task DeleteWebhookAsync(bool? dropPendingUpdates = null, CancellationToken cancellationToken = default)
        {
            var data = new Dictionary<string, object?>
            {
                { nameof(dropPendingUpdates).ToSnakeCaseLower(), dropPendingUpdates }
            };

            using var httpResponse = await _httpClient.PostAsJsonAsync($"{_telegramBotEndpoint}{_botToken}/setWebhook", data, cancellationToken);
            var telegramResponse = httpResponse.Content.ReadFromJson<Response<object>>(_jsonSerializerOptions);

            if (telegramResponse != null)
            {
                if (!telegramResponse.Ok)
                {
                    throw new TelegramResponseException($"{telegramResponse.ErrorCode} {telegramResponse.Description}");
                }
            }
            else
            {
                httpResponse.EnsureSuccessStatusCode();
            }
        }

        public void DeleteWebhook(bool? dropPendingUpdates = null)
        {
            var data = new Dictionary<string, object?>
            {
                { nameof(dropPendingUpdates).ToSnakeCaseLower(), dropPendingUpdates }
            };

            using var httpResponse = _httpClient.PostAsJson($"{_telegramBotEndpoint}{_botToken}/setWebhook", data);
            var telegramResponse = httpResponse.Content.ReadFromJson<Response<object>>(_jsonSerializerOptions);

            if (telegramResponse != null)
            {
                if (!telegramResponse.Ok)
                {
                    throw new TelegramResponseException($"{telegramResponse.ErrorCode} {telegramResponse.Description}");
                }
            }
            else
            {
                httpResponse.EnsureSuccessStatusCode();
            }
        }

        public async Task<WebhookInfo?> GetWebhookInfoAsync(CancellationToken cancellationToken = default)
        {
            using var httpResponse = await _httpClient.GetAsync($"{_telegramBotEndpoint}{_botToken}/getWebhookInfo", cancellationToken);
            var telegramResponse = await httpResponse.Content.ReadFromJsonAsync<Response<WebhookInfo>>(_jsonSerializerOptions, cancellationToken);

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

        public WebhookInfo? GetWebhookInfo()
        {
            using var httpResponse = _httpClient.Get($"{_telegramBotEndpoint}{_botToken}/getWebhookInfo");
            var telegramResponse = httpResponse.Content.ReadFromJson<Response<WebhookInfo>>(_jsonSerializerOptions);

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
    }
}