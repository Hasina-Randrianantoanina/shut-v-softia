namespace SHUT.Tests;

public class LaunchTest
{
    [Fact]
    public async Task HealthCheckTest()
    {
        using var client = new HttpClient();
        var response = await client.GetAsync("http://localhost:5154/HealthCheck");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal("Hello from Api", responseString);

    }
}