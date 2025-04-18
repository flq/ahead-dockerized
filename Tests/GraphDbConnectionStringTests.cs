using Ahead.Common;
using Xunit;

namespace Tests;

public class GraphDbConnectionStringTests
{

    [Fact]
    public void WorksAsIntended()
    {
        var graphConnectString = new GraphDbConnectionString
        {
            Host = "localhost",
            Username = "Horst",
            Password = "secretpwd",
            Port = 8282,
        };
        
        var connectString2 = new GraphDbConnectionString(graphConnectString.ToString());
        Assert.Equal("localhost", connectString2.Host);
        Assert.Equal("Horst", connectString2.Username);
        Assert.Equal("secretpwd", connectString2.Password);
        Assert.Equal(8282, connectString2.Port);
    }
    
}