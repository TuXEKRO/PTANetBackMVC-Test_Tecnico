using interviewProject.DTOs;
using interviewProject.Events;
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

            EventManager.Emit("OnGetAllCountries", (HttpContext, result));

            return result.IsSuccess ? Ok(result.Value) : NotFound();
        }

        [HttpGet("{countryCode}")]
        public async Task<IActionResult> GetCountryByCode(string countryCode)
        {
            var result = await _countryService.GetCountryByCodeAsync(countryCode);

            EventManager.Emit("OnGetCountryByCode", (HttpContext, result));

            return result.IsSuccess ? Ok(result.Value) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> AddCountry([FromBody] CountryDto countryDto)
        {
            var result = await _countryService.AddCountryAsync(countryDto);

            EventManager.Emit("OnAddCountry", (HttpContext, result));

            return result.IsSuccess ?
                CreatedAtAction(nameof(GetCountryByCode), new { countryCode = countryDto.CountryCode }, countryDto)
                : BadRequest(result.Error);
        }

        [HttpPut("{countryCode}")]
        public async Task<IActionResult> UpdateCountry(string countryCode, [FromBody] CountryDto countryDto)
        {
            var result = await _countryService.UpdateCountryAsync(countryCode, countryDto);

            EventManager.Emit("OnUpdateCountry", (HttpContext, result));

            return result.IsSuccess ? NoContent() : NotFound(result.Error);
        }

        [HttpDelete("{countryCode}")]
        public async Task<IActionResult> DeleteCountry(string countryCode)
        {
            var result = await _countryService.DeleteCountryAsync(countryCode);

            EventManager.Emit("OnDeleteCountry", (HttpContext, result));

            return result.IsSuccess ? NoContent() : NotFound(result.Error);
        }

        [HttpGet("{countryCode}/Mba")]
        public async Task<IActionResult> GetMbasByCountryCode(string countryCode)
        {
            var result = await _countryService.GetMbasByCountryCodeAsync(countryCode);

            EventManager.Emit("OnGetMbasByCountryCode", (HttpContext, result));

            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        [HttpPost("{countryCode}/Mba")]
        public async Task<IActionResult> AddMbaToCountry(string countryCode, [FromBody] MbaDto mbaDto)
        {
            var result = await _countryService.AddMbaToCountryAsync(countryCode, mbaDto);

            EventManager.Emit("OnAddMbaToCountry", (HttpContext, result));

            return result.IsSuccess ? NoContent() : NotFound(result.Error);
        }
    }
}
