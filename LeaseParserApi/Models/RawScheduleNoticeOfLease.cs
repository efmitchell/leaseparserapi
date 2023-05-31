namespace LeaseParserApi.Models;

public class RawScheduleNoticeOfLease
{
    public string EntryNumber { get; set; }
    public string EntryDate { get; set; }
    public string EntryType { get; set; }
    public List<string> EntryText { get; set; }
}