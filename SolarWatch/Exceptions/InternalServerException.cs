namespace SolarWatch.Exceptions;

public class InternalServerException(string message, Exception? innerException = null) : ApiException(message);