using interviewProject.DTOs;
using interviewProject.Services;
using interviewProject.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace interviewProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountryController : ControllerBase
    {
        private readonly ICountryService _countryService;

        public CountryController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCountries([FromQuery] string countryCode = null)
        {
            var result = await _countryService.GetAllCountriesAsync(countryCode);

            return result.IsSuccess ? Ok(result.Value) : NotFound();
        }

        [HttpGet("{countryCode}")]
        public async Task<IActionResult> GetCountryByCode(string countryCode)
        {
            var result = await _countryService.GetCountryByCodeAsync(countryCode);

            return result.IsSuccess ? Ok(result.Value) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> AddCountry([FromBody] CountryDto countryDto)
        {
            var result = await _countryService.AddCountryAsync(countryDto);

            return result.IsSuccess ?
                CreatedAtAction(nameof(GetCountryByCode), new { countryCode = countryDto.CountryCode }, countryDto)
                : BadRequest(result.Error);
        }

        [HttpPut("{countryCode}")]
        public async Task<IActionResult> UpdateCountry(string countryCode, [FromBody] CountryDto countryDto)
        {
            var result = await _countryService.UpdateCountryAsync(countryCode, countryDto);

            return result.IsSuccess ? NoContent() : NotFound(result.Error);
        }

        [HttpDelete("{countryCode}")]
        public async Task<IActionResult> DeleteCountry(string countryCode)
        {
            var result = await _countryService.DeleteCountryAsync(countryCode);

            return result.IsSuccess ? NoContent() : NotFound(result.Error);
        }

        [HttpGet("{countryCode}/Mba")]
        public async Task<IActionResult> GetMbasByCountryCode(string countryCode)
        {
            var result = await _countryService.GetMbasByCountryCodeAsync(countryCode);

            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        [HttpPost("{countryCode}/Mba")]
        public async Task<IActionResult> AddMbaToCountry(string countryCode, [FromBody] MbaDto mbaDto)
        {
            var result = await _countryService.AddMbaToCountryAsync(countryCode, mbaDto);

            return result.IsSuccess ? NoContent() : NotFound(result.Error);
        }
    }
}
