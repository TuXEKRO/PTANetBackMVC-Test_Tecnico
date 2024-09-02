using interviewProject.Data;
using interviewProject.DTOs;
using interviewProject.Models;
using interviewProject.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace interviewProject.Services
{
    public interface ICountryService
    {
        Task<Result<IEnumerable<CountryDto>>> GetAllCountriesAsync(string countryCode = null);
        Task<Result<CountryDto>> GetCountryByCodeAsync(string countryCode);
        Task<Result> AddCountryAsync(CountryDto countryDto);
        Task<Result> UpdateCountryAsync(string countryCode, CountryDto countryDto);
        Task<Result> DeleteCountryAsync(string countryCode);
        Task<Result<IEnumerable<MbaDto>>> GetMbasByCountryCodeAsync(string countryCode);
        Task<Result> AddMbaToCountryAsync(string countryCode, MbaDto mbaDto);
    }

    public class CountryService : ICountryService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CountryService> _logger;

        public CountryService(ApplicationDbContext context, ILogger<CountryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<CountryDto>>> GetAllCountriesAsync(string countryCode = null)
        {
            IQueryable<Country> query = _context.Countries;

            if (!string.IsNullOrEmpty(countryCode))
            {
                query = query.Where(c => c.CountryCode == countryCode);
            }

            var countries = await query
                .Select(c => new CountryDto
                {
                    Country = c.CountryName,
                    CountryCode = c.CountryCode,
                    Mbas = c.Mbas.Select(m => new MbaDto
                    {
                        Code = m.Code,
                        Name = m.Name
                    }).ToList()
                })
                .ToListAsync();

            if (countries == null || countries.Count == 0)
            {
                _logger.LogWarning($"Country with code {countryCode} not found.");
                return Result<IEnumerable<CountryDto>>.Failure("Country not found.");
            }

            return Result<IEnumerable<CountryDto>>.Success(countries);
        }

        public async Task<Result<CountryDto>> GetCountryByCodeAsync(string countryCode)
        {
            var country = await _context.Countries
                .Where(c => c.CountryCode == countryCode)
                .Select(c => new CountryDto
                {
                    Country = c.CountryName,
                    CountryCode = c.CountryCode,
                    Mbas = c.Mbas.Select(m => new MbaDto
                    {
                        Code = m.Code,
                        Name = m.Name
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (country == null)
            {
                _logger.LogWarning($"Country with code {countryCode} not found.");
                return Result<CountryDto>.Failure("Country not found.");
            }

            return Result<CountryDto>.Success(country);
        }

        public async Task<Result> AddCountryAsync(CountryDto countryDto)
        {
            if (await _context.Countries.AnyAsync(c => c.CountryCode == countryDto.CountryCode))
            {
                _logger.LogWarning($"Country with code {countryDto.CountryCode} already exists.");
                return Result.Failure("Country with this code already exists.");
            }

            var country = new Country
            {
                CountryName = countryDto.Country,
                CountryCode = countryDto.CountryCode,
                Mbas = countryDto.Mbas?.Select(m => new Mba
                {
                    Code = m.Code,
                    Name = m.Name
                }).ToList()
            };

            _context.Countries.Add(country);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> UpdateCountryAsync(string countryCode, CountryDto countryDto)
        {
            var country = await _context.Countries
                .Include(c => c.Mbas)
                .FirstOrDefaultAsync(c => c.CountryCode == countryCode);

            if (country == null)
            {
                _logger.LogWarning($"Country with code {countryCode} not found.");
                return Result.Failure("Country not found.");
            }

            country.CountryName = countryDto.Country;

            if (countryDto.Mbas != null)
            {
                country.Mbas = countryDto.Mbas.Select(m => new Mba
                {
                    Code = m.Code,
                    Name = m.Name
                }).ToList();
            }

            _context.Countries.Update(country);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> DeleteCountryAsync(string countryCode)
        {
            var country = await _context.Countries
                .FirstOrDefaultAsync(c => c.CountryCode == countryCode);

            if (country == null)
            {
                _logger.LogWarning($"Country with code {countryCode} not found.");
                return Result.Failure("Country not found.");
            }

            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<IEnumerable<MbaDto>>> GetMbasByCountryCodeAsync(string countryCode)
        {
            var mbas = await _context.Mbas
                .Where(m => m.Country.CountryCode == countryCode)
                .Select(m => new MbaDto
                {
                    Code = m.Code,
                    Name = m.Name
                })
                .ToListAsync();

            if (!mbas.Any())
            {
                _logger.LogWarning($"No MBAs found for country code {countryCode}.");
                return Result<IEnumerable<MbaDto>>.Failure("No MBAs found for this country.");
            }

            return Result<IEnumerable<MbaDto>>.Success(mbas);
        }

        public async Task<Result> AddMbaToCountryAsync(string countryCode, MbaDto mbaDto)
        {
            var country = await _context.Countries
                .Include(c => c.Mbas)
                .FirstOrDefaultAsync(c => c.CountryCode == countryCode);

            if (country == null)
            {
                _logger.LogWarning($"Country with code {countryCode} not found.");
                return Result.Failure("Country not found.");
            }

            var mba = new Mba
            {
                Code = mbaDto.Code,
                Name = mbaDto.Name
            };

            country.Mbas.Add(mba);

            _context.Countries.Update(country);
            await _context.SaveChangesAsync();

            return Result.Success();
        }
    }
}
