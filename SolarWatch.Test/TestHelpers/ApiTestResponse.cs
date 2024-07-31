using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SolarWatch.Test.TestHelpers;

public class ApiTestResponse
{
    [Required] public string Name { get; set; }
}