using LeaseParserApi.Exceptions;
using LeaseParserApi.Interfaces;
using LeaseParserApi.Models;

namespace LeaseParserApi.Services;

public class ScheduleParserService : IScheduleParserService
{
    private readonly IEnumerable<IEntryTextParser> _entryTextParsers;

    public ScheduleParserService(IEnumerable<IEntryTextParser> entryTextParsers)
    {
        _entryTextParsers = entryTextParsers;
    }

    public async Task<IEnumerable<ParsedScheduleNoticeOfLease>> ParseRawScheduleNoticeOfLeasesAsync(
        IEnumerable<RawScheduleNoticeOfLease> rawScheduleData)
    {
        List<ParsedScheduleNoticeOfLease> parsedDataList = new();

        foreach (var data in rawScheduleData)
            try
            {
                var parsedData = ParseSingleScheduleNotice(data);
                parsedDataList.Add(parsedData);
            }
            catch (Exception ex)
            {
                throw new ScheduleParseException(
                    $"Failed to parse schedule notice with EntryNumber: {data.EntryNumber}", ex);
            }

        return parsedDataList;
    }

    private ParsedScheduleNoticeOfLease ParseSingleScheduleNotice(RawScheduleNoticeOfLease rawData)
    {
        ParsedScheduleNoticeOfLease parsedData = new();

        parsedData.EntryNumber = ParseEntryNumber(rawData.EntryNumber);

        foreach (var text in rawData.EntryText)
        {
            var parts = text.Split(new[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
            {
                continue;
            }

            var keyword = parts[0];
            var value = parts[1].Trim();
            
            foreach (var parser in _entryTextParsers)
            {
                if (parser.CanParse(keyword))
                {
                    parser.Parse(parsedData, value);
                    break;
                }
            }
        }

        parsedData.Notes = ParseNotes(rawData.EntryText);

        return parsedData;
    }

    private int ParseEntryNumber(string entryNumber)
    {
        if (!int.TryParse(entryNumber, out var result))
            throw new ScheduleParseException($"Failed to parse EntryNumber: {entryNumber}");

        return result;
    }

    private List<string> ParseNotes(List<string> entryText)
    {
        return entryText.Where(text => text.StartsWith("NOTE")).ToList();
    }
}