namespace SocialMediaDetailAppBackend.Model
{
    public class WebhookPayload
    {
        public string id { get; set; }
        public string result { get; set; }
        public string reason { get; set; }
        public List<string> links { get; set; }
    }
}
