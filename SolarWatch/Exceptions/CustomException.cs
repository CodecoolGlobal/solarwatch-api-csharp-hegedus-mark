using System.Net;

namespace SolarWatch.Exceptions;

public abstract class CustomException(string message, HttpStatusCode statusCode) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}