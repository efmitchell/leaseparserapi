using FluentAssertions;
using LeaseParserApi.Exceptions;
using LeaseParserApi.Interfaces;
using LeaseParserApi.Models;
using LeaseParserApi.Parser;
using LeaseParserApi.Services;

namespace LeaseParserApTests
{
    public class ScheduleServiceTests
    {
        private IScheduleParserService _service;
        private List<RawScheduleNoticeOfLease> _rawScheduleNoticeOfLeases;

        public ScheduleServiceTests()
        {
            _service = new ScheduleParserService(GetParsers());
            _rawScheduleNoticeOfLeases = GetRawScheduleNoticeOfLeases();
        }

        private IEnumerable<IEntryTextParser> GetParsers()
        {
            return new List<IEntryTextParser>
            {
                new RegistrationDateParser(),
                new PropertyParser(),
                new LeaseDateParser(),
                new LesseeTitleParser()
            };
        }

        private List<RawScheduleNoticeOfLease> GetRawScheduleNoticeOfLeases()
        {
            return new List<RawScheduleNoticeOfLease>
            {
                new RawScheduleNoticeOfLease
                {
                    EntryNumber = "1234",
                    EntryText = new List<string> 
                    { 
                        "REGDATE: 01.01.2023",
                        "PROPERTY: Property1",
                        "LEASEDATE: 02.02.2023",
                        "LESSEETITLE: LesseeTitle1" 
                    }
                },
                new RawScheduleNoticeOfLease
                {
                    EntryNumber = "5678",
                    EntryText = new List<string> 
                    { 
                        "REGDATE: 03.03.2023",
                        "PROPERTY: Property2",
                        "LEASEDATE: 04.04.2023",
                        "LESSEETITLE: LesseeTitle2" 
                    }
                }
            };
        }

        [Fact]
        public async Task ParseRawScheduleNoticeOfLeasesAsync_ShouldReturnParsedData_WhenDataIsValid()
        {
            // Act
            var result = await _service.ParseRawScheduleNoticeOfLeasesAsync(_rawScheduleNoticeOfLeases);

            // Assert
            result.Count().Should().Be(2);
            result.First().EntryNumber.Should().Be(1234);
            result.Last().EntryNumber.Should().Be(5678);
        }

        [Fact]
        public async Task ParseRawScheduleNoticeOfLeasesAsync_ShouldThrowException_WhenDataIsInvalid()
        {
            // Arrange
            _rawScheduleNoticeOfLeases.First().EntryNumber = "InvalidEntryNumber";

            // Act
            Func<Task> act = async () => await _service.ParseRawScheduleNoticeOfLeasesAsync(_rawScheduleNoticeOfLeases);

            // Assert
            await act.Should().ThrowAsync<ScheduleParseException>();
        }
    }
}