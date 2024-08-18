namespace SolarWatch.RequestsAndResponses.Auth;

public record AuthResponse(string Email, string UserName, string Token);