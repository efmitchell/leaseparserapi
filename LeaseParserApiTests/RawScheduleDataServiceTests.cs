using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using LeaseParserApi.Exceptions;
using LeaseParserApi.Models;
using LeaseParserApi.Services;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Contrib.HttpClient;
using Xunit;

namespace LeaseParserApTests
{
    public class RawScheduleDataServiceTests
    {
        private readonly RawScheduleDataService _service;
        private readonly Mock<HttpMessageHandler> _handler;
        private readonly Mock<IOptions<ApiSettings>> _mockApiSettings;

        public RawScheduleDataServiceTests()
        {
            _handler = new Mock<HttpMessageHandler>();
            var clientFactory = new Mock<IHttpClientFactory>();
            clientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(new HttpClient(_handler.Object));

            _mockApiSettings = new Mock<IOptions<ApiSettings>>();
            _mockApiSettings.Setup(x => x.Value).Returns(new ApiSettings { ApiUrl = "https://your-eyeexam-api-url.com/api/endpoint" });

            _service = new RawScheduleDataService(clientFactory.Object, _mockApiSettings.Object, maxRetries: 3);
        }

        [Fact]
        public async Task GetRawScheduleNoticeOfLeasesAsync_ReturnsData_WhenDataExists()
        {
            // Arrange
            var rawData = GetRawScheduleData();
            var json = JsonSerializer.Serialize(rawData);
            _handler.SetupRequest(HttpMethod.Get, "https://your-eyeexam-api-url.com/api/endpoint")
                .ReturnsResponse(HttpStatusCode.OK, json, "application/json");

            // Act
            var result = await _service.GetRawScheduleNoticeOfLeasesAsync();

            // Assert
            result.Should().BeEquivalentTo(rawData);
        }

        [Fact]
        public async Task GetRawScheduleNoticeOfLeasesAsync_ThrowsException_WhenApiFails()
        {
            // Arrange
            _handler.SetupRequest(HttpMethod.Get, "https://your-eyeexam-api-url.com/api/endpoint")
                .ReturnsResponse(HttpStatusCode.BadRequest);

            // Act
            Func<Task> act = async () => await _service.GetRawScheduleNoticeOfLeasesAsync();

            // Assert
            await act.Should().ThrowAsync<RawScheduleDataException>()
                .WithMessage("Failed to get raw schedule data from https://your-eyeexam-api-url.com/api/endpoint");
        }

        private RawScheduleNoticeOfLease[] GetRawScheduleData()
        {
            return new[]
            {
                new RawScheduleNoticeOfLease { EntryNumber = "1", EntryDate = "2023-01-01", EntryType = "Lease", EntryText = new List<string>() },
                new RawScheduleNoticeOfLease { EntryNumber = "2", EntryDate = "2023-02-01", EntryType = "Lease", EntryText = new List<string>() }
            };
        }
    }
}