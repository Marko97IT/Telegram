namespace Telegram.Helpers
{
    public static class StreamExtensions
    {
        public static string ReadString(this Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            using var streamReader = new StreamReader(stream);
            return streamReader.ReadToEnd();
        }
    }
}