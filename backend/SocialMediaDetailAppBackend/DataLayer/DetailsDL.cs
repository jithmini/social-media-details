using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using SocialMediaDetailAppBackend.Model;
using System.Data;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace SocialMediaDetailAppBackend.DataLayer
{
    public class DetailsDL
    {
        private readonly string _connectionString;
        private readonly HttpClient _httpClient;

        public DetailsDL(string connectionString, HttpClient httpClient)
        {
            _connectionString = connectionString;
            _httpClient = httpClient;
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
            SELECT ua.link, a.app_name, a.app_id, ISNULL(ua.status,0), COUNT(uad.id) AS description_count
            FROM user_app ua
            INNER JOIN app_table a ON ua.app_id = a.app_id
            LEFT JOIN user_app_des uad ON ua.user_id = uad.user_id AND ua.app_id = uad.app_id
            WHERE ua.user_id = @UserId
            AND ua.link IS NOT NULL
            AND LTRIM(RTRIM(ua.link)) <> ''
            GROUP BY ua.link, a.app_name, a.app_id, ua.status
            ";

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
                                Status = reader.GetBoolean(3),
                                DescriptionCount = reader.GetInt32(4)
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
                string query = "INSERT INTO user_app_des (user_id, app_id, description, post_link) VALUES (@UserId, @AppId, @Description, @PostLink)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserId", description.UserId);
                cmd.Parameters.AddWithValue("@AppId", description.AppId);
                cmd.Parameters.AddWithValue("@Description", description.Description);
                cmd.Parameters.AddWithValue("@PostLink", description.PostLink);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        //public bool updateUserAppStatus(UserAppDescription description)
        //{
        //    using (SqlConnection con = new SqlConnection(_connectionString))
        //    {
        //        string storedProcedure = "SP_UPDATE_USERAPP_STATUS";

        //        SqlCommand cmd = new SqlCommand(storedProcedure, con);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        cmd.Parameters.AddWithValue("@UserId", description.UserId);
        //        cmd.Parameters.AddWithValue("@AppId", description.AppId);

        //        con.Open();

        //        return cmd.ExecuteNonQuery() > 0;
        //    }
        //}

        public bool updateUserAppStatus(SubmitDescription description)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string storedProcedure = "SP_UPDATE_USERAPP_STATUS";

                SqlCommand cmd = new SqlCommand(storedProcedure, con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@UserId", description.UserId);
                //cmd.Parameters.AddWithValue("@AppId", description.AppId);

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

        public bool SendWebhookAccept(WebhookPayload payload)
        {
            try
            {
                var json = JsonConvert.SerializeObject(payload);
                var request = new HttpRequestMessage(HttpMethod.Post, "");

                // Headers
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "");
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                // Timeout (prevent hangs)
                _httpClient.Timeout = TimeSpan.FromSeconds(30);

                // Send request
                var response = _httpClient.Send(request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = response.Content.ReadAsStringAsync();
                    throw new Exception($"Webhook error: {response.StatusCode} - {errorContent}");
                }

                return true;
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Webhook failed: {ex.Message}");
                return false;
            }
        }

        public bool SendWebhookReject (WebhookPayload payload)
        {
            try
            {
                WebhookPayload dbPayload = SendWebhook(payload.id);
                if (dbPayload == null)
                    throw new Exception("No descriptions or links found for the given user ID.");
                var json = JsonConvert.SerializeObject(dbPayload);
                var request = new HttpRequestMessage(HttpMethod.Post, "");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "");
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                _httpClient.Timeout = TimeSpan.FromSeconds(30);
                var response = _httpClient.Send(request);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = response.Content.ReadAsStringAsync();
                    throw new Exception($"Webhook error: {response.StatusCode} - {errorContent}");
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Webhook failed: {ex.Message}");
                return false;
            }
        }
        public WebhookPayload SendWebhook(string userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT [description], [post_link] FROM [dbo].[user_app_des] WHERE [user_id] = @userId ORDER BY [id]";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        var descriptions = new List<string>();
                        var links = new List<string>();

                        while (reader.Read())
                        {
                            var description = !reader.IsDBNull(0) ? reader.GetString(0) : null;
                            var postLink = !reader.IsDBNull(1) ? reader.GetString(1) : null;

                            if (!string.IsNullOrWhiteSpace(description))
                                descriptions.Add(description);

                            if (!string.IsNullOrWhiteSpace(postLink))
                                links.Add(postLink);
                        }

                        if (descriptions.Any() || links.Any())
                        {
                            return new WebhookPayload
                            {
                                id = userId,
                                result = "rejected",
                                reason = string.Join(" | ", descriptions),
                                links = links
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}
