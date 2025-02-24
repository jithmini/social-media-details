using SocialMediaDetailAppBackend.DataLayer;
using SocialMediaDetailAppBackend.Model;

namespace SocialMediaDetailAppBackend.BusinessLayer
{
    public class LoginBL
    {
        private readonly LoginDL _loginDL;

        public LoginBL(LoginDL loginDL)
        {
            _loginDL = loginDL;
        }

        public UserValidationResult ValidateUser(string username, string password)
        {
            var userDetails = _loginDL.GetLoginUserDetails(username);
            var isValid = userDetails.PasswordHash != null && BCrypt.Net.BCrypt.Verify(password, userDetails.PasswordHash);

            return new UserValidationResult
            {
                IsValid = isValid,
                Roles = isValid ? userDetails.Roles : new List<string>()
            };
        }

        public bool RegisterUser(string username, string password)
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            return _loginDL.RegisterUser(username, passwordHash);
        }
    }
}