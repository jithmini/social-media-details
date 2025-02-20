using Microsoft.Data.SqlClient;
using SocialMediaDetailAppBackend.Model;

namespace SocialMediaDetailAppBackend.DataLayer
{
    public class DetailsDL
    {
        private readonly string _connectionString;

        public DetailsDL(string connectionString)
        {
            _connectionString = connectionString;
        }
        public UserDetail GetUserDetailById(string userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT user_id, full_name, title, status FROM user_details WHERE user_id = @UserId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new UserDetail
                            {
                                UserId = reader.GetString(0),
                                FullName = reader.GetString(1),
                                Title = reader.GetString(2),
                                Status = reader.GetBoolean(3)
                            };
                        }
                    }
                }
            }

            return null;
        }
    }
}
