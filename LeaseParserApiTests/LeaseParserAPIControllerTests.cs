using FluentAssertions;
using LeaseParserApi.Controllers;
using LeaseParserApi.Interfaces;
using LeaseParserApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace LeaseParserApTests
{
    public class LeaseParserApiControllerTests
    {
        private readonly Mock<IScheduleParserService> _scheduleServiceMock;
        private readonly Mock<IRawScheduleDataService> _rawScheduleDataServiceMock;
        private readonly Mock<ICacheProvider> _cacheProviderMock;
        private readonly Mock<ILogger<LeaseParserApiController>> _loggerMock;
        private readonly LeaseParserApiController _controller;

        public LeaseParserApiControllerTests()
        {
            _scheduleServiceMock = new Mock<IScheduleParserService>();
            _rawScheduleDataServiceMock = new Mock<IRawScheduleDataService>();
            _cacheProviderMock = new Mock<ICacheProvider>();
            _loggerMock = new Mock<ILogger<LeaseParserApiController>>();
            _controller = new LeaseParserApiController(_scheduleServiceMock.Object, _rawScheduleDataServiceMock.Object, _cacheProviderMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ParseData_ReturnsParsedData_WhenDataExists()
        {
            // Arrange
            var rawScheduleData = GetRawScheduleData();
            var parsedData = GetParsedData();
            SetupForDataExists(rawScheduleData, parsedData);

            // Act
            var result = await _controller.ParseData();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(parsedData);
        }

        [Fact]
        public async Task ParseData_ReturnsNotFound_WhenDataDoesNotExist()
        {
            // Arrange
            SetupForDataDoesNotExist();

            // Act
            var result = await _controller.ParseData();

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task ParseData_ReturnsCachedData_WhenDataIsCached()
        {
            // Arrange
            var cachedData = GetParsedData();
            SetupForCachedData(cachedData);

            // Act
            var result = await _controller.ParseData();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(cachedData);
        }

        private RawScheduleNoticeOfLease[] GetRawScheduleData()
        {
            return new[]
            {
                new RawScheduleNoticeOfLease { EntryNumber = "1", EntryDate = "2023-01-01", EntryType = "Lease", EntryText = new List<string>() },
                new RawScheduleNoticeOfLease { EntryNumber = "2", EntryDate = "2023-02-01", EntryType = "Lease", EntryText = new List<string>() }
            };
        }

        private ParsedScheduleNoticeOfLease[] GetParsedData()
        {
            return new[]
            {
                new ParsedScheduleNoticeOfLease { EntryNumber = 1, EntryDate = new DateTime(2023, 1, 1), LesseesTitle = "Lease", Notes = new List<string>() },
                new ParsedScheduleNoticeOfLease { EntryNumber = 2, EntryDate = new DateTime(2023, 2, 1), LesseesTitle = "Lease", Notes = new List<string>() }
            };
        }

        private void SetupForDataExists(RawScheduleNoticeOfLease[] rawScheduleData, ParsedScheduleNoticeOfLease[] parsedData)
        {
            _rawScheduleDataServiceMock.Setup(x => x.GetRawScheduleNoticeOfLeasesAsync()).ReturnsAsync(rawScheduleData);
            _scheduleServiceMock.Setup(x => x.ParseRawScheduleNoticeOfLeasesAsync(rawScheduleData)).ReturnsAsync(parsedData as IEnumerable<ParsedScheduleNoticeOfLease>);
            _cacheProviderMock.Setup(x => x.TryGet<ParsedScheduleNoticeOfLease[]>("parsedScheduleNoticeOfLeases", out parsedData)).Returns(false);
        }

        private void SetupForDataDoesNotExist()
        {
            _rawScheduleDataServiceMock.Setup(x => x.GetRawScheduleNoticeOfLeasesAsync()).ReturnsAsync((RawScheduleNoticeOfLease[])null);
        }

        private void SetupForCachedData(ParsedScheduleNoticeOfLease[] cachedData)
        {
            _cacheProviderMock.Setup(x => x.TryGet<ParsedScheduleNoticeOfLease[]>("parsedScheduleNoticeOfLeases", out cachedData)).Returns(true);
        }
    }
}