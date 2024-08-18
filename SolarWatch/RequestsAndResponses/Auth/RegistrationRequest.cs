using System.ComponentModel.DataAnnotations;

namespace SolarWatch.RequestsAndResponses.Auth;

public record RegistrationRequest(
    [Required] string Email,
    [Required] string Password,
    [Required] string Username);