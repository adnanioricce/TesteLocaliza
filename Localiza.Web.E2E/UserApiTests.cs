
using System.Text;
using DotNet.Testcontainers.Builders;
using Newtonsoft.Json;

namespace Localiza.Web.E2E;
using System;
using System.Net.Http;
using System.Threading.Tasks;
// using Testcontainers.Container.Abstractions.Hosting;
// using Testcontainers.Container.Abstractions.Models;
// using Testcontainers.Container.Database.PostgreSql;
using DotNet.Testcontainers.Containers;
using Xunit;
public class UserApiTests : IAsyncLifetime
{
    private readonly HttpClient _httpClient;
    private IContainer _dbContainer;
    private readonly string _apiBaseUrl;

    public UserApiTests()
    {
        _apiBaseUrl = "http://localhost:8084"; // Adjust if necessary
        _httpClient = new HttpClient();
    }

    public async Task InitializeAsync()
    {
        _dbContainer = new ContainerBuilder()
            .WithImage("adnanioricce/localiza-db")
            .WithPortBinding(5436, 5432)
            // .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(5436)))
            // Build the container configuration.
            .Build();

        await _dbContainer.StartAsync().ConfigureAwait(false);

        // Setup the API container
        var apiContainer = new ContainerBuilder()
            .WithImage("adnanioricce/localiza-api") // Adjust to your Docker image
            .WithPortBinding(8084,80) // Maps port 5000 on host to port 80 in container
            .WithEnvironment("ConnectionStrings__DefaultConnection", $"Host={_dbContainer.Hostname};Port=5432;Username=localizador;Password=localizapw;Database=localizadb")
            .Build();

        await apiContainer.StartAsync().ConfigureAwait(false);
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }

    [Fact]
    public async Task CreateUser_ShouldReturnSuccess()
    {
        // Arrange
        var requestBody = new
        {
            Nome = "John Doe",
            Email = "john.doe@example.com",
            Senha = "hashedpassword123"
        };

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{_apiBaseUrl}/usuarios")
        {
            Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json")
        };

        // Act
        var response = await _httpClient.SendAsync(requestMessage);

        // Assert
        response.EnsureSuccessStatusCode(); // Verify the response status code is successful (200-299)

        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("John Doe", responseContent);
        Assert.Contains("john.doe@example.com", responseContent);
    }
}