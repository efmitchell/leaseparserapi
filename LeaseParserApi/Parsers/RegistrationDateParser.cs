using LeaseParserApi.Exceptions;
using LeaseParserApi.Interfaces;
using LeaseParserApi.Models;

namespace LeaseParserApi.Parser;

public class RegistrationDateParser : IEntryTextParser
{
    public bool CanParse(string keyword) => keyword == "REGDATE";
    
    public void Parse(ParsedScheduleNoticeOfLease parsedData, string value)
    {
        parsedData.RegistrationDateAndPlanRef = ParseDate(value);
    }

    private string ParseDate(string date)
    {
        if (!DateTime.TryParse(date, out var parsedDate))
            throw new ScheduleParseException($"Failed to parse date: {date}");

        return parsedDate.ToString("dd.MM.yyyy");
    }
}