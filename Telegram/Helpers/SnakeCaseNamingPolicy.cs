using System.Text;
using System.Text.Json;

namespace Telegram.Helpers
{
    [Obsolete("In .NET 8 the snake case naming policy is available without create custom converters.")]
    internal class SnakeCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            var stringBuilder = new StringBuilder();
            var chars = name.AsSpan();

            for (int i = 0; i < chars.Length; i++)
            {
                if (i > 0 && char.IsUpper(chars[i]))
                {
                    stringBuilder.Append('_');
                }

                stringBuilder.Append(char.ToLower(chars[i]));
            }

            return stringBuilder.ToString();
        }
    }
}