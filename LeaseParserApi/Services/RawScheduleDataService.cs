using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using LeaseParserApi.Exceptions;
using LeaseParserApi.Interfaces;
using LeaseParserApi.Models;
using Microsoft.Extensions.Options;

namespace LeaseParserApi.Services;

public class RawScheduleDataService : IRawScheduleDataService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ApiSettings _settings;
    private readonly int _maxRetries;

    public RawScheduleDataService(IHttpClientFactory clientFactory, IOptions<ApiSettings> settings, int maxRetries = 3)
    {
        _clientFactory = clientFactory;
        _settings = settings.Value;
        _maxRetries = maxRetries;
    }

    public async Task<RawScheduleNoticeOfLease[]> GetRawScheduleNoticeOfLeasesAsync()
    {
        var request = CreateRequest();

        for (var retryCount = 0; retryCount < _maxRetries; retryCount++)
        {
            try
            {
                return await ExecuteRequest(request);
            }
            catch (RawScheduleDataException)
            {
                throw new RawScheduleDataException($"Failed to get raw schedule data from {_settings.ApiUrl}");
            }
        }

        throw new RawScheduleDataException($"Failed to get raw schedule data from {_settings.ApiUrl} after {_maxRetries} retries.");
    }

    private HttpRequestMessage CreateRequest()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, _settings.ApiUrl);

        // Convert credentials to base64 and add basic authentication header
        var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_settings.ApiUser}:{_settings.ApiPassword}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authValue);

        return request;
    }

    private async Task<RawScheduleNoticeOfLease[]> ExecuteRequest(HttpRequestMessage request)
    {
        var client = _clientFactory.CreateClient();
        var response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            using var responseStream = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<IEnumerable<RawScheduleNoticeOfLease>>(responseStream);
            return result.ToArray();
        }

        throw new RawScheduleDataException($"Failed to get raw schedule data from {_settings.ApiUrl}");
    }
}