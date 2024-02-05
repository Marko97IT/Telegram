using System.Text.Json;
using System.Text.Json.Serialization;

namespace Telegram.Helpers
{
    internal class UnixToDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TryGetInt32(out var unix))
            {
                return DateTimeOffset.FromUnixTimeSeconds(unix).UtcDateTime;
            }

            throw new InvalidCastException("Invalid unix time");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds());
        }
    }
}