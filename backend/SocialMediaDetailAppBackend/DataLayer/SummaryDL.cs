using Microsoft.Data.SqlClient;
using SocialMediaDetailAppBackend.Model;

namespace SocialMediaDetailAppBackend.DataLayer
{
    public class SummaryDL
    {
        private readonly string _connectionString;

        public SummaryDL(string connectionString)
        {
            _connectionString = connectionString;
        }        

        public List<UserDetail> GetUserDetails()
        {
            var userDetails = new List<UserDetail>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT user_id, full_name, title, status FROM user_details";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            userDetails.Add(new UserDetail
                            {
                                UserId = reader.GetString(0),
                                FullName = reader.GetString(1),
                                Title = reader.GetString(2),
                                Status = reader.GetBoolean(3)
                            });
                        }
                    }
                }
            }

            return userDetails;
        }
    }
}
