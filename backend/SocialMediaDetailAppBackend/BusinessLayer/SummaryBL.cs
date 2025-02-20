using SocialMediaDetailAppBackend.DataLayer;
using SocialMediaDetailAppBackend.Model;

namespace SocialMediaDetailAppBackend.BusinessLayer
{
    public class SummaryBL
    {
        private readonly SummaryDL _summaryDL;

        public SummaryBL(SummaryDL summaryDL)
        {
            _summaryDL = summaryDL;
        }

        public List<UserDetail> GetUserDetails()
        {
            return _summaryDL.GetUserDetails();
        }
    }
}
