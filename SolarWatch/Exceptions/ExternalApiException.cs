namespace SolarWatch.Exceptions;

public class ExternalApiException(string message, Exception? innerException = null) : CustomException(message);