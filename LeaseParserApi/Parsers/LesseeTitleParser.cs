using LeaseParserApi.Interfaces;
using LeaseParserApi.Models;

namespace LeaseParserApi.Parser;

public class LesseeTitleParser : IEntryTextParser
{
    public bool CanParse(string keyword) => keyword == "LESSEETITLE";
    
    public void Parse(ParsedScheduleNoticeOfLease parsedData, string value)
    {
        parsedData.LesseesTitle = value;
    }
}