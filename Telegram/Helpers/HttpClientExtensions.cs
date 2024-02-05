// Copyright (c) 2024 Marco Concas. All rights reserved.
// Licensed under the Apache License.

using System.Net.Http.Headers;
using System.Text.Json;

namespace Telegram.Helpers
{
    internal static class HttpClientExtensions
    {
        public static HttpResponseMessage Get(this HttpClient httpClient, string? requestUri)
        {
            using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
            return httpClient.Send(httpRequestMessage);
        }

        public static HttpResponseMessage Post(this HttpClient httpClient, string? requestUri, HttpContent? content)
        {
            using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri) { Content = content };
            return httpClient.Send(httpRequestMessage);
        }

        public static HttpResponseMessage PostAsJson<T>(this HttpClient httpClient, string? requestUri, T value, JsonSerializerOptions? options = null)
        {
            var json = JsonSerializer.Serialize(value, options);
            using var content = new StringContent(json, MediaTypeHeaderValue.Parse("application/json"));
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, requestUri) { Content = content };
            return httpClient.Send(httpRequest);
        }

        public static T? ReadFromJson<T>(this HttpContent content, JsonSerializerOptions? options = null)
        {
            using var contentStream = content.ReadAsStream();
            return JsonSerializer.Deserialize<T>(contentStream, options);
        }
    }
}