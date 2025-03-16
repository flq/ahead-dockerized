namespace Tests;

public class TestTimeProvider(DateTime fixedDateTime) : TimeProvider
{
    public DateTime FixedDateTime { get; } = fixedDateTime;
    public override DateTimeOffset GetUtcNow() => new(FixedDateTime);
}