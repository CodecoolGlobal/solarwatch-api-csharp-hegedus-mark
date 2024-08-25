namespace SolarWatch.Exceptions;

public class NotFoundException(string message) : CustomException(message);