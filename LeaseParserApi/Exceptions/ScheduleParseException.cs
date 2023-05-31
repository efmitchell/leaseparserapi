namespace LeaseParserApi.Exceptions
{
    public class ScheduleParseException : Exception
    {
        public ScheduleParseException(string message) : base(message)
        {
        }

        public ScheduleParseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}