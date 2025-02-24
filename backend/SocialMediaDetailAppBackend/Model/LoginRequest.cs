namespace SocialMediaDetailAppBackend.Model
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginUserDetails
    {
        public string PasswordHash { get; set; }
        public List<string> Roles { get; set; }
    }

    public class UserValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Roles { get; set; }
    }
}
