using System.Diagnostics;
using System.Text;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace Ahead.Common;

public static class OTelUtilities
{
    public static readonly ActivitySource MessagingActivitySource = new("Ahead.Messaging");
    static readonly TraceContextPropagator TraceContextPropagator = new();

    public static void TryInjectTraceContextIntoDictionary(this Activity? activity, IDictionary<string, object?> headers)
    {
        if (activity is null) return;
        TraceContextPropagator.Inject(
            new PropagationContext(activity.Context, Baggage.Current),
            headers, 
            (hs, key, value) => hs[key] = value);
    }

    public static PropagationContext TryExtractPropagationContext(this IDictionary<string, object?>? headers) =>
        TraceContextPropagator.Extract(default, headers, (props, key) =>
        {
            if (!(props?.TryGetValue(key, out var value) ?? false))
                return [];
            var bytes = (byte[])value!;
            return [Encoding.UTF8.GetString(bytes)];
        });
}