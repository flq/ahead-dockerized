using System.Text.Json;

namespace Ahead.Common;

public class SerializationUtilities
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = false,
    };
}