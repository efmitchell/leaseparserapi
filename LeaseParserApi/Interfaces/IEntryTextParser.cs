using LeaseParserApi.Models;

namespace LeaseParserApi.Interfaces;

public interface IEntryTextParser
{
    bool CanParse(string keyword);
    void Parse(ParsedScheduleNoticeOfLease parsedData, string value);
}