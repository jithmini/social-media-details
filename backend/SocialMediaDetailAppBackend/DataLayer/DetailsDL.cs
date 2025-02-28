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

        public List<UserAppDetail> GetUserApps(string userId)
        {
            var userApps = new List<UserAppDetail>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = @"
            SELECT ua.link, a.app_name, a.app_id
            FROM user_app ua
            INNER JOIN app_table a ON ua.app_id = a.app_id
            WHERE ua.user_id = @UserId";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            userApps.Add(new UserAppDetail
                            {
                                Link = reader.GetString(0),
                                AppName = reader.GetString(1),
                                AppId = reader.GetString(2),
                            });
                        }
                    }
                }
            }

            return userApps;
        }

        public bool AddDescription(UserAppDescription description)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO user_app_des (user_id, app_id, description) VALUES (@UserId, @AppId, @Description)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserId", description.UserId);
                cmd.Parameters.AddWithValue("@AppId", description.AppId);
                cmd.Parameters.AddWithValue("@Description", description.Description);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public List<UserAppDescription> GetDescriptions(string userId, string appId)
        {
            List<UserAppDescription> descriptions = new List<UserAppDescription>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT id, user_id, app_id, description FROM user_app_des WHERE user_id = @UserId AND app_id = @AppId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@AppId", appId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    descriptions.Add(new UserAppDescription
                    {
                        Id = reader.GetInt32(0),
                        UserId = reader.GetString(2),
                        AppId = reader.GetString(2),
                        Description = reader.GetString(3)
                    });
                }
            }

            return descriptions;
        }

        public bool DeleteDescription(int id)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM user_app_des WHERE id = @Id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}
