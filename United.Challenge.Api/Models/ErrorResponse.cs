public record FieldError(string Field, string Message);

namespace United.Challenge.Api.Models
{

    public class ErrorResponse
    {
        public List<FieldError> Errors { get; } = new();
    }
}