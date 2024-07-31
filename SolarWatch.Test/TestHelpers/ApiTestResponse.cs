using System.ComponentModel.DataAnnotations;

namespace SolarWatch.Test.TestHelpers;

public class ApiTestResponse
{
    [Required] public string Name { get; set; }
}