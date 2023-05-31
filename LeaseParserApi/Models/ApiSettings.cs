namespace LeaseParserApi.Models;

public class ApiSettings
{
    public string ApiUrl { get; set; }
    public string ApiUser { get; set; } // The username for basic authentication
    public string ApiPassword { get; set; } // The password for basic authentication
}