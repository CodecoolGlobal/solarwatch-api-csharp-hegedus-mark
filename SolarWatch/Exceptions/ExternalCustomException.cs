namespace SolarWatch.Exceptions;

public class ExternalCustomException(string message, Exception? innerException = null) : CustomException(message);