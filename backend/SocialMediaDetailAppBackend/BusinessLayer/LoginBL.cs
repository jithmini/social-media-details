using SocialMediaDetailAppBackend.DataLayer;

namespace SocialMediaDetailAppBackend.BusinessLayer
{
    public class LoginBL
    {
        private readonly LoginDL _loginDL;

        public LoginBL(LoginDL loginDL)
        {
            _loginDL = loginDL;
        }

        public bool ValidateUser(string username, string password)
        {
            var storedPasswordHash = _loginDL.GetPasswordHash(username);
            return storedPasswordHash != null && BCrypt.Net.BCrypt.Verify(password, storedPasswordHash);
        }

        public bool RegisterUser(string username, string password)
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            return _loginDL.RegisterUser(username, passwordHash);
        }
    }
}