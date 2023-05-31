using LeaseParserApi.Interfaces;
using LeaseParserApi.Models;

namespace LeaseParserApi.Parser;

public class PropertyParser : IEntryTextParser
{
    public bool CanParse(string keyword) => keyword == "PROPERTY";
    
    public void Parse(ParsedScheduleNoticeOfLease parsedData, string value)
    {
        parsedData.PropertyDescription = value;
    }
}