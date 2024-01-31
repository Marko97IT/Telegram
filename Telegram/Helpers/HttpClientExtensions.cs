using System.Net.Http.Json;
using System.Text.Json;
using Telegram.Types;

namespace Telegram.Helpers
{
    internal static class HttpClientExtensions
    {
        internal static async Task<Response<T?>?> ReadResponseFromTelegramAsync<T>(this HttpContent content, CancellationToken cancellationToken = default)
        {
            return await content.ReadFromJsonAsync<Response<T?>>(new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            }, cancellationToken);
        }
    }
}