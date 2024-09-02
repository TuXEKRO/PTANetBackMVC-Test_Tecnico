using System.Text;
using System.Text.Json;
using interviewProject.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;

public class CountryControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public CountryControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    // Se ejecuta antes de cada prueba
    public async Task InitializeAsync()
    {
        await CleanupCountry("TS"); // Limpia cualquier entrada previa con el mismo código de país
    }

    private async Task CleanupCountry(string countryCode)
    {
        var deleteResponse = await _client.DeleteAsync($"/api/Country/{countryCode}");
        // Ignorar errores de que no exista el recurso
    }

    [Fact]
    public async Task Can_Add_And_Get_Country()
    {
        // Limpia antes de comenzar la prueba
        await InitializeAsync();

        // Arrange
        var countryDto = new CountryDto
        {
            Country = "TEST50",
            CountryCode = "TS",
            Mbas = new List<MbaDto>
            {
                new MbaDto { Code = "TS_code_50", Name = "ts1" }
            }
        };

        var content = new StringContent(JsonSerializer.Serialize(countryDto), Encoding.UTF8, "application/json");

        try
        {
            // Act - POST
            var postResponse = await _client.PostAsync("/api/Country", content);
            postResponse.EnsureSuccessStatusCode();

            // Act - GET
            var getResponse = await _client.GetAsync("/api/Country/TS");
            getResponse.EnsureSuccessStatusCode();

            var responseContent = await getResponse.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<CountryDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Assert
            Assert.NotNull(result);
            Assert.Equal("TEST50", result.Country);
            Assert.Equal("TS", result.CountryCode);
            Assert.Single(result.Mbas);
            Assert.Equal("TS_code_50", result.Mbas[0].Code);
            Assert.Equal("ts1", result.Mbas[0].Name);
        }
        catch (HttpRequestException ex)
        {
            // Loggear el error y relanzar la excepción
            throw new HttpRequestException($"POST request failed with status code {ex.StatusCode} and body: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new Exception("An unexpected error occurred during the test execution.", ex);
        }
    }
}
