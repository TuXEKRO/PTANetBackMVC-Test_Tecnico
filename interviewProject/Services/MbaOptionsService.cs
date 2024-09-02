using interviewProject.Data;
using interviewProject.Models;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using interviewProject.DTOs;

public class MbaOptionsService : IHostedService, IDisposable
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHttpClientFactory _httpClientFactory;
    private Timer _timer;

    public MbaOptionsService(IServiceScopeFactory scopeFactory, IHttpClientFactory httpClientFactory)
    {
        _scopeFactory = scopeFactory;
        _httpClientFactory = httpClientFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(12));
        return Task.CompletedTask;
    }

    private async void DoWork(object state)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var client = _httpClientFactory.CreateClient();

            var response = await client.GetAsync("https://api.opendata.esett.com/EXP03/MBAOptions");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var countries = JsonSerializer.Deserialize<List<CountryDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                foreach (var countryDto in countries)
                {
                    var existingCountry = await dbContext.Countries
                        .Include(c => c.Mbas)
                        .FirstOrDefaultAsync(c => c.CountryCode == countryDto.CountryCode);

                    if (existingCountry != null)
                    {
                        existingCountry.CountryName = countryDto.Country;

                        foreach (var mbaDto in countryDto.Mbas)
                        {
                            if (!existingCountry.Mbas.Any(m => m.Name == mbaDto.Name))
                            {
                                existingCountry.Mbas.Add(new Mba
                                {
                                    Code = mbaDto.Code,
                                    Name = mbaDto.Name
                                });
                            }
                        }

                        dbContext.Countries.Update(existingCountry);
                    }
                    else
                    {
                        var country = new Country
                        {
                            CountryName = countryDto.Country,
                            CountryCode = countryDto.CountryCode,
                            Mbas = countryDto.Mbas.Select(mba => new Mba
                            {
                                Code = mba.Code,
                                Name = mba.Name
                            }).ToList()
                        };

                        dbContext.Countries.Add(country);
                    }

                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
