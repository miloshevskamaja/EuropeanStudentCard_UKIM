namespace EuropeanStudentCard.Api.Errors
{
    public class ApiErrorResponse
    {
        public string Code { get; set; } = default!;
        public string Message { get; set; } = default!;
        public object? Details { get; set; }
    }
}
