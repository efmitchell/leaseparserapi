using LeaseParserApi.Interfaces;
using LeaseParserApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace LeaseParserApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LeaseParserApiController : Controller
{
    private readonly IScheduleParserService _scheduleParserService;
    private readonly IRawScheduleDataService _rawScheduleDataService;
    private readonly ICacheProvider _cacheProvider;
    private readonly ILogger<LeaseParserApiController> _logger;

    public LeaseParserApiController(IScheduleParserService scheduleParserService,
        IRawScheduleDataService rawScheduleDataService, ICacheProvider cacheProvider,
        ILogger<LeaseParserApiController> logger)
    {
        _scheduleParserService = scheduleParserService;
        _rawScheduleDataService = rawScheduleDataService;
        _cacheProvider = cacheProvider;
        _logger = logger;
    }

    [HttpGet("parseData")]
    public async Task<IActionResult> ParseData()
    {
        _logger.LogInformation("Starting to parse data...");

        if (!_cacheProvider.TryGet("parsedScheduleNoticeOfLeases", out ParsedScheduleNoticeOfLease[] cachedData))
        {
            _logger.LogInformation("Cache missed, retrieving raw data from API...");

            var rawScheduleData = await _rawScheduleDataService.GetRawScheduleNoticeOfLeasesAsync();
            if (rawScheduleData == null)
            {
                _logger.LogWarning("No data found in raw schedule data API. Returning 404...");
                return NotFound();
            }

            _logger.LogInformation("Raw data retrieved, parsing data...");

            var parsedData =
                await _scheduleParserService.ParseRawScheduleNoticeOfLeasesAsync(rawScheduleData.ToArray());

            _logger.LogInformation("Data parsed, caching results...");

            _cacheProvider.Set("parsedScheduleNoticeOfLeases", parsedData);
            _logger.LogInformation("Results cached, returning results...");

            return Ok(parsedData);
        }

        _logger.LogInformation("Cache hit, returning cached data...");
        return Ok(cachedData);
    }
}