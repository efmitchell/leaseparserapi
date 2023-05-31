using LeaseParserApi.Exceptions;
using LeaseParserApi.Interfaces;
using LeaseParserApi.Models;

namespace LeaseParserApi.Parser;

public class LeaseDateParser : IEntryTextParser
{
    public bool CanParse(string keyword) => keyword == "LEASEDATE";
    
    public void Parse(ParsedScheduleNoticeOfLease parsedData, string value)
    {
        parsedData.DateOfLeaseAndTerm = ParseDate(value);
    }

    private string ParseDate(string date)
    {
        if (!DateTime.TryParse(date, out var parsedDate))
            throw new ScheduleParseException($"Failed to parse date: {date}");

        return parsedDate.ToString("dd.MM.yyyy");
    }
}