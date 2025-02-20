using SocialMediaDetailAppBackend.DataLayer;
using SocialMediaDetailAppBackend.Model;

namespace SocialMediaDetailAppBackend.BusinessLayer
{
    public class DetailsBL
    {
        private readonly DetailsDL _detailsDL;

        public DetailsBL(DetailsDL loginDL)
        {
            _detailsDL = loginDL;
        }
        public UserDetail GetUserDetailById(string userId)
        {
            return _detailsDL.GetUserDetailById(userId);
        }
    }
}
