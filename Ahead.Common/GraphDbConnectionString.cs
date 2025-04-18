using System.Data.Common;

namespace Ahead.Common;

public class GraphDbConnectionString
{
    public const string Name = "GraphDbConnectionString";
    private readonly DbConnectionStringBuilder connectStringBuilder;
    
    public GraphDbConnectionString()
    {
        connectStringBuilder = new DbConnectionStringBuilder();
    }

    public GraphDbConnectionString(string connectionString)
    {
        connectStringBuilder = new DbConnectionStringBuilder
        {
            ConnectionString = connectionString
        };
    }

    public string Host
    {
        get => connectStringBuilder["host"].ToString()!;
        set => connectStringBuilder["host"] = value;
    }

    public string Username
    {
        get => connectStringBuilder["username"].ToString()!;
        set => connectStringBuilder["username"] = value;
    }

    public string Password
    {
        get => connectStringBuilder["password"].ToString()!;
        set => connectStringBuilder["password"] = value;
    }

    public int Port
    {
        get => Convert.ToInt32(connectStringBuilder["port"].ToString()!);
        set => connectStringBuilder["port"] = value;
    }

    public bool UseSsl
    {
        get => bool.Parse(connectStringBuilder["useSsl"].ToString()!);
        set => connectStringBuilder["useSsl"] = value;
    }

    public override string ToString() => connectStringBuilder.ToString();

}