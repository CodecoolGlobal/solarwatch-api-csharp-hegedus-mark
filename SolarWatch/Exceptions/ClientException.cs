using System.Net;

namespace SolarWatch.Exceptions;

public class ClientException(string message) : CustomException(message, HttpStatusCode.BadRequest);



