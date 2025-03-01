namespace SocialMediaDetailAppBackend.Model
{
    public class UpdateDescriptionRequest
    {
        public string UserId { get; set; }
        public string AppName { get; set; }
        public string Description { get; set; }
    }
}
