using System.Text.Json;
using Ahead.Common.Dto;
using Xunit;

namespace Tests;

public class SerializationFacts
{
    [Fact]
    public void TheMessagingSerializationWorks()
    {
        var time = new TestTimeProvider(DateTime.UtcNow);
        var body = JsonSerializer.SerializeToUtf8Bytes(new ReportRequest
        {
            Type = "report",
            RelevantDate = time.FixedDateTime
        });
        var deserializedBody = JsonSerializer.Deserialize<ReportRequest>(body.AsMemory().Span);
        Assert.NotNull(deserializedBody);
        Assert.Equal(time.FixedDateTime, deserializedBody.RelevantDate);
    }

}