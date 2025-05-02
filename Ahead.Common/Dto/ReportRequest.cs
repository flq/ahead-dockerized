namespace Ahead.Common.Dto;

public class ReportRequest
{
    public required string Type { get; init; }
    public required DateTime RelevantDate  { get; init; }
}