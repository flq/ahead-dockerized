using System.Text.Json;
using Ahead.Common.Dto;
using Xunit;

namespace Tests;

public class SerializationFacts
{
    [Fact]
    public void TheMessagingSerializationWorks()
    {
        var body = JsonSerializer.SerializeToUtf8Bytes(new CalculationRequest { Calculation = "xyz" });
        var deserializedBody = JsonSerializer.Deserialize<CalculationRequest>(body.AsMemory().Span);
        Assert.NotNull(deserializedBody);
        Assert.Equal("xyz", deserializedBody.Calculation);
    }

}