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

        public List<UserAppDetail> GetUserApps(string userId)
        {
            return _detailsDL.GetUserApps(userId);
        }

        public bool AddDescription(UserAppDescription description)
        {
            return _detailsDL.AddDescription(description);
        }

        public List<UserAppDescription> GetDescriptions(string userId, string appName)
        {
            return _detailsDL.GetDescriptions(userId, appName);
        }

        public bool DeleteDescription(int id)
        {
            return _detailsDL.DeleteDescription(id);
        }
    }
}
