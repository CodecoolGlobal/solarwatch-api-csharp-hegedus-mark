namespace SolarWatch.Exceptions;

public class CriticalServerLogicException(string message) : ApiException(message);