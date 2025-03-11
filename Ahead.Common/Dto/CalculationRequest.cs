using System.Text.Json.Serialization;

namespace Ahead.Common.Dto;

public class CalculationRequest
{
    public required string Calculation  { get; set; }
}