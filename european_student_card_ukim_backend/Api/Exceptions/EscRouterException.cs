using System.Net;

namespace EuropeanStudentCard.Api.Exceptions;

/// <summary>
/// Custom exception for ESC Router API errors.
/// </summary>
public class EscRouterException : Exception
{

    public HttpStatusCode? StatusCode { get; }

    public string? ResponseBody { get; }

    public string? Endpoint { get; }

    public EscRouterException(string message) : base(message)
    {
    }

    public EscRouterException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }

    public EscRouterException(
        string message, 
        HttpStatusCode statusCode, 
        string? responseBody = null,
        string? endpoint = null) 
        : base(message)
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
        Endpoint = endpoint;
    }

    public EscRouterException(
        string message, 
        HttpStatusCode statusCode, 
        Exception innerException,
        string? responseBody = null,
        string? endpoint = null) 
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
        Endpoint = endpoint;
    }
}

