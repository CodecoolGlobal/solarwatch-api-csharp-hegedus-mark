using System.Net;

namespace SolarWatch.Exceptions;

public class CriticalServerLogicException(string message) : CustomException(message, HttpStatusCode.InternalServerError);