namespace Booking.Host.Logging;

public class LoggerSettings
{
    public string AppName { get; set; } = "mock.web.api";
    public string ElasticSearchUrl { get; set; } = string.Empty;
    public bool WriteToFile { get; set; } = false;
    public bool StructuredConsoleLogging { get; set; } = false;
    public string MinimumLogLevel { get; set; } = "Information";
}