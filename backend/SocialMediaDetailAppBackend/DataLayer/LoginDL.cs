using Microsoft.Data.SqlClient;

namespace SocialMediaDetailAppBackend.DataLayer
{
    public class LoginDL
    {
        private readonly string _connectionString;

        public LoginDL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public string GetPasswordHash(string username)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT Password FROM manual_user WHERE UPPER(Username) = UPPER(@Username)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    return command.ExecuteScalar() as string;
                }
            }
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