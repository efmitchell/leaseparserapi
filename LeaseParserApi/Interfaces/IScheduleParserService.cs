using LeaseParserApi.Models;

namespace LeaseParserApi.Interfaces;

public interface IScheduleParserService
{
    Task<IEnumerable<ParsedScheduleNoticeOfLease>> ParseRawScheduleNoticeOfLeasesAsync(
        IEnumerable<RawScheduleNoticeOfLease> rawScheduleData);
}