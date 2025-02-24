using Microsoft.Data.SqlClient;
using SocialMediaDetailAppBackend.Model;

namespace SocialMediaDetailAppBackend.DataLayer
{
    public class LoginDL
    {
        private readonly string _connectionString;

        public LoginDL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public LoginUserDetails GetLoginUserDetails(string username)
        {
            var userDetails = new LoginUserDetails();
            userDetails.Roles = new List<string>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT Password FROM manual_user WHERE UPPER(Username) = UPPER(@Username)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    userDetails.PasswordHash = command.ExecuteScalar() as string;
                }

                var rolesQuery = "SELECT Role FROM role R INNER JOIN manual_user_role MR on MR.RoleId = R.RoleId INNER JOIN manual_user U ON U.UserId = MR.ManualUserId WHERE R.IsActive = 1 AND MR.IsActive = 1 AND U.IsActive=1 AND UPPER(U.Username) = UPPER(@Username)";
                using (var command = new SqlCommand(rolesQuery, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            userDetails.Roles.Add(reader["Role"].ToString());
                        }
                    }
                }
            }
            return userDetails;
        }

        public bool RegisterUser(string username, string passwordHash)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var query = "INSERT INTO manual_user (Username, Password, IsActive) VALUES (@Username, @Password, 1)";
                        using (var command = new SqlCommand(query, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@Username", username);
                            command.Parameters.AddWithValue("@Password", passwordHash);
                            var result = command.ExecuteNonQuery();

                            if (result > 0)
                            {
                                transaction.Commit();
                                return true;
                            }
                            else
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}