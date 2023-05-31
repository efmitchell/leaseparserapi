using LeaseParserApi.Models;

namespace LeaseParserApi.Interfaces;

public interface IRawScheduleDataService
{
    Task<RawScheduleNoticeOfLease[]> GetRawScheduleNoticeOfLeasesAsync();
}