namespace Ahead.Common.Dto;

public class ReportRequest
{
    public string Type { get; init; }
    public required DateTime RelevantDate  { get; init; }
}