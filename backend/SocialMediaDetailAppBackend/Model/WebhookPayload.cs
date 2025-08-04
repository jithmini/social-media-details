namespace SocialMediaDetailAppBackend.Model
{
    public class SourceRecord
    {
        public string Url { get; set; }
        public bool IsFlagged { get; set; }
        public string Reasoning { get; set; }
        public int Severity { get; set; }
    }

    //public class WebhookResult
    //{
    //    public bool IsSuccess { get; private set; }
    //    public bool NotFound { get; private set; }
    //    public string ErrorMessage { get; private set; }
    //    public WebhookPayload Payload { get; private set; }

    //    public static WebhookResult Success(WebhookPayload payload) => new() { IsSuccess = true, Payload = payload };
    //    public static WebhookResult Fail(string error) => new() { IsSuccess = false, ErrorMessage = error };
    //    public static WebhookResult NotFound(string error) => new() { NotFound = true, ErrorMessage = error };
    //}
}
