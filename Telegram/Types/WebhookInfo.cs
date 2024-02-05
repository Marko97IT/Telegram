using System.Text.Json.Serialization;
using Telegram.Helpers;

namespace Telegram.Types
{
    public class WebhookInfo
    {
        public string Url { get; set; }
        public bool HasCustomCertificate { get; set; }
        public int PendingUpdateCount { get; set; }
        public string? IpAddress { get; set; }
        [JsonConverter(typeof(UnixToDateTimeConverter))] public DateTime? LastErrorDate { get; set; }
        public string? LastErrorMessage { get; set; }
        [JsonConverter(typeof(UnixToDateTimeConverter))] public DateTime? LastSynchronizationErrorDate { get; set; }
        public int? MaxConnections { get; set; }
        public string[]? AllowedUpdates { get; set; }
    }
}