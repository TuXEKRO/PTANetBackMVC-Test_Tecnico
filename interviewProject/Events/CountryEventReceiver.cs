using interviewProject.DTOs;
using interviewProject.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;

namespace interviewProject.Events
{
    public class CountryEventReceiver : IEventReceiver
    {
        private readonly ILogger<CountryEventReceiver> _logger;

        public CountryEventReceiver(ILogger<CountryEventReceiver> logger)
        {
            _logger = logger;
        }

        public void Register()
        {
            EventManager.Register<(HttpContext, Result<IEnumerable<CountryDto>>)>("OnGetAllCountries", HandleEvent);
            EventManager.Register<(HttpContext, Result<IEnumerable<CountryDto>>)>("OnGetCountryByCode", HandleEvent);
            EventManager.Register<(HttpContext, Result<IEnumerable<CountryDto>>)>("OnAddCountry", HandleEvent);
            EventManager.Register<(HttpContext, Result<IEnumerable<CountryDto>>)>("OnUpdateCountry", HandleEvent);
            EventManager.Register<(HttpContext, Result<IEnumerable<CountryDto>>)>("OnDeleteCountry", HandleEvent);
            EventManager.Register<(HttpContext, Result<IEnumerable<CountryDto>>)>("OnGetMbasByCountryCode", HandleEvent);
            EventManager.Register<(HttpContext, Result<IEnumerable<CountryDto>>)>("OnAddMbaToCountry", HandleEvent);
        }

        public void Unregister()
        {
            EventManager.Unregister<(HttpContext, Result<IEnumerable<CountryDto>>)>("OnGetAllCountries", HandleEvent);
            EventManager.Unregister<(HttpContext, Result<IEnumerable<CountryDto>>)>("OnGetCountryByCode", HandleEvent);
            EventManager.Unregister<(HttpContext, Result<IEnumerable<CountryDto>>)>("OnAddCountry", HandleEvent);
            EventManager.Unregister<(HttpContext, Result<IEnumerable<CountryDto>>)>("OnUpdateCountry", HandleEvent);
            EventManager.Unregister<(HttpContext, Result<IEnumerable<CountryDto>>)>("OnDeleteCountry", HandleEvent);
            EventManager.Unregister<(HttpContext, Result<IEnumerable<CountryDto>>)>("OnGetMbasByCountryCode", HandleEvent);
            EventManager.Unregister<(HttpContext, Result<IEnumerable<CountryDto>>)>("OnAddMbaToCountry", HandleEvent);
        }

        private void HandleEvent((HttpContext httpContext, Result<IEnumerable<CountryDto>> result) eventData)
        {
            var (httpContext, result) = eventData;

            string action = httpContext.Request.Path.Value;
            string requestIp = httpContext.Connection.RemoteIpAddress.ToString();
            string timestamp = DateTime.UtcNow.ToString("o");

            if (result.IsSuccess)
            {
                _logger.LogInformation($"[{timestamp}] [{requestIp}] [{action}] Operation successful. Result: {JsonSerializer.Serialize(result.Value)}");
            }
            else
            {
                _logger.LogError($"[{timestamp}] [{requestIp}] [{action}] Operation failed: {result.Error}");
            }
        }
    }
}