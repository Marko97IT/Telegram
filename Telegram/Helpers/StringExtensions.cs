using System.Text;

namespace Telegram.Helpers
{
    internal static class StringExtensions
    {
        /// <summary>Returns a copy of this string converted to snakecase lowercase.</summary>
        /// <returns>A string in snakecase lowercase.</returns>
        public static string ToSnakeCaseLower(this string input)
        {
            var output = new StringBuilder();
            var chars = input.AsSpan();

            for (int i = 0; i < chars.Length; i++)
            {
                if (i > 0 && char.IsUpper(chars[i]))
                {
                    output.Append('_');
                }

                output.Append(char.ToLower(chars[i]));
            }

            return output.ToString();
        }

        /// <summary>Generate a random string with fixed length.</summary>
        /// <returns>A random string with fixed length.</returns>
        public static string GenerateRandomString(int length)
        {
            const string validChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-";

            var random = new Random();
            var output = new char[length];

            for (int i = 0; i < length; i++)
            {
                output[i] = validChars[random.Next(validChars.Length)];
            }

            return new string(output);
        }
    }
}