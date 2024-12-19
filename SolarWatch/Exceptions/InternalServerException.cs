using System.Net;

namespace SolarWatch.Exceptions;

public class InternalServerException(string message, Exception? innerException = null) : CustomException(message, HttpStatusCode.InternalServerError);