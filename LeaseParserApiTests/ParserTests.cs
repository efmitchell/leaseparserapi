using FluentAssertions;
using LeaseParserApi.Models;
using LeaseParserApi.Parser;

namespace LeaseParserApTests
{
    public class ParserTests
    {
        [Fact]
        public void RegistrationDateParser_ShouldParseCorrectly()
        {
            // Arrange
            var parser = new RegistrationDateParser();
            var input = "01.01.2023";
            var expected = "01.01.2023";
            var parsedNotice = new ParsedScheduleNoticeOfLease();

            // Act
            parser.Parse(parsedNotice, input);

            // Assert
            parsedNotice.RegistrationDateAndPlanRef.Should().Be(expected);
        }

        [Fact]
        public void PropertyParser_ShouldParseCorrectly()
        {
            // Arrange
            var parser = new PropertyParser();
            var input = "Property1";
            var expected = "Property1";
            var parsedNotice = new ParsedScheduleNoticeOfLease();

            // Act
            parser.Parse(parsedNotice, input);

            // Assert
            parsedNotice.PropertyDescription.Should().Be(expected);
        }

        [Fact]
        public void LeaseDateParser_ShouldParseCorrectly()
        {
            // Arrange
            var parser = new LeaseDateParser();
            var input = "02.02.2023";
            var expected = "02.02.2023";
            var parsedNotice = new ParsedScheduleNoticeOfLease();

            // Act
            parser.Parse(parsedNotice, input);

            // Assert
            parsedNotice.DateOfLeaseAndTerm.Should().Be(expected);
        }

        [Fact]
        public void LesseeTitleParser_ShouldParseCorrectly()
        {
            // Arrange
            var parser = new LesseeTitleParser();
            var input = "LesseeTitle1";
            var expected = "LesseeTitle1";
            var parsedNotice = new ParsedScheduleNoticeOfLease();

            // Act
            parser.Parse(parsedNotice, input);

            // Assert
            parsedNotice.LesseesTitle.Should().Be(expected);
        }
    }
}