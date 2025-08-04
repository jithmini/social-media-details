using Microsoft.Data.SqlClient;
using SocialMediaDetailAppBackend.Model;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SocialMediaDetailAppBackend.DataLayer
{
    public class LoginDL
    {
        private readonly string _connectionString;

        public LoginDL(string connectionString)
        {
            _connectionString = connectionString;
        }

        private SqlConnection OpenConnectionWithRetry(int maxRetries = 60, int delayMilliseconds = 5000)
        {
            int retries = 0;
            while (true)
            {
                try
                {
                    var connection = new SqlConnection(_connectionString);
                    connection.Open();
                    return connection;
                }
                catch (SqlException ex)
                {
                    retries++;
                    if (retries >= maxRetries)
                    {
                        Console.WriteLine($"❌ Failed to connect after {maxRetries} retries.");
                        throw;
                    }

                    Console.WriteLine($"⏳ DB waking... Retry {retries}/{maxRetries}. Retrying in {delayMilliseconds / 1000}s...");
                    Thread.Sleep(delayMilliseconds);
                }
            }
        }

        public LoginUserDetails GetLoginUserDetails(string username)
        {
            var userDetails = new LoginUserDetails
            {
                Roles = new List<string>()
            };

            using (var connection = OpenConnectionWithRetry())
            {
                var query = "SELECT Password FROM manual_user WHERE UPPER(Username) = UPPER(@Username)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    userDetails.PasswordHash = command.ExecuteScalar() as string;
                }

                var rolesQuery = @"
                    SELECT Role 
                    FROM role R 
                    INNER JOIN manual_user_role MR ON MR.RoleId = R.RoleId 
                    INNER JOIN manual_user U ON U.UserId = MR.ManualUserId 
                    WHERE R.IsActive = 1 AND MR.IsActive = 1 AND U.IsActive = 1 
                    AND UPPER(U.Username) = UPPER(@Username)";

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
            using (var connection = OpenConnectionWithRetry())
            {
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