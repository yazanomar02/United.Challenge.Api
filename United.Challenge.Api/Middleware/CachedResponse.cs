namespace United.Challenge.Api.Middleware
{
    public class CachedResponse
    {
        public int StatusCode { get; set; }
        public string? ContentType { get; set; }
        public string Body { get; set; } = string.Empty;

        public Dictionary<string, string> Headers { get; set; } = new();
    }
}
