using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using SocialMediaDetailAppBackend.Model;
using System.Data;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;



namespace SocialMediaDetailAppBackend.DataLayer
{
    public class DetailsDL: ControllerBase
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
                var query = "SELECT user_id, full_name, title, autostatus, manualstatus, biography, city, country, expertise, language, lifeExperience, lifeExperience, organization FROM user_details WHERE user_id = @UserId";
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
                                AutoStatus = reader.GetInt32(3),
                                ManualStatus = reader.GetBoolean(4),
                                Biography = reader.GetString(5),
                                City = reader.GetString(6),
                                Country = reader.GetString(7),
                                Expertise = reader.GetString(8),
                                Language = reader.GetString(9),
                                LifeExperience = reader.GetString(10),
                                Organization = reader.GetString(11)
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

        //public bool SendWebhookAccept(WebhookPayload payload)
        //{
        //    try
        //    {
        //        var json = JsonConvert.SerializeObject(payload);
        //        var request = new HttpRequestMessage(HttpMethod.Post, "https://dev.faithandmediaconnect.org/api/webhook/lyrical");

        //        // Headers
        //        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "");
        //        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        //        // Timeout (prevent hangs)
        //        _httpClient.Timeout = TimeSpan.FromSeconds(30);

        //        // Send request
        //        var response = _httpClient.Send(request);

        //        if (!response.IsSuccessStatusCode)
        //        {
        //            var errorContent = response.Content.ReadAsStringAsync();
        //            throw new Exception($"Webhook error: {response.StatusCode} - {errorContent}");
        //        }

        //        return true;
        //    }

        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Webhook failed: {ex.Message}");
        //        return false;
        //    }
        //}

        


        //public bool SendWebhookReject (WebhookPayload payload)
        //{
        //    try
        //    {
        //        WebhookPayload dbPayload = SendWebhook(payload.id);
        //        if (dbPayload == null)
        //            throw new Exception("No descriptions or links found for the given user ID.");
        //        var json = JsonConvert.SerializeObject(dbPayload);
        //        var request = new HttpRequestMessage(HttpMethod.Post, "");
        //        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "");
        //        request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        //        _httpClient.Timeout = TimeSpan.FromSeconds(30);
        //        var response = _httpClient.Send(request);
        //        if (!response.IsSuccessStatusCode)
        //        {
        //            var errorContent = response.Content.ReadAsStringAsync();
        //            throw new Exception($"Webhook error: {response.StatusCode} - {errorContent}");
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Webhook failed: {ex.Message}");
        //        return false;
        //    }
        //}
        //public WebhookPayload SendWebhook(string userId)
        //{
        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        connection.Open();
        //        var query = "SELECT [description], [post_link] FROM [dbo].[user_app_des] WHERE [user_id] = @userId ORDER BY [id]";
        //        using (var command = new SqlCommand(query, connection))
        //        {
        //            command.Parameters.AddWithValue("@UserId", userId);
        //            using (var reader = command.ExecuteReader())
        //            {
        //                var descriptions = new List<string>();
        //                var links = new List<string>();

        //                while (reader.Read())
        //                {
        //                    var description = !reader.IsDBNull(0) ? reader.GetString(0) : null;
        //                    var postLink = !reader.IsDBNull(1) ? reader.GetString(1) : null;

        //                    if (!string.IsNullOrWhiteSpace(description))
        //                        descriptions.Add(description);

        //                    if (!string.IsNullOrWhiteSpace(postLink))
        //                        links.Add(postLink);
        //                }

        //                if (descriptions.Any() || links.Any())
        //                {
        //                    return new WebhookPayload
        //                    {
        //                        id = userId,
        //                        result = "rejected",
        //                        reason = string.Join(" | ", descriptions),
        //                        links = links
        //                    };
        //                }
        //            }
        //        }
        //    }
        //    return null;
        //}

            public List<string> GetAllLinks(string userId)
            {
                List<string> links = new List<string>();

                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    string query = "SELECT url FROM dbo.Sources WHERE user_id = @UserId AND IsArticle = 1 AND System_flag = 'automated' AND isFlagged = 0";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string url = reader["url"].ToString();
                            links.Add(url);
                        }
                    }
                }

                return links;
            }

        public List<FlaggedLinkDetail> GetFlaggedLinksWithDetails(string userId)
        {
            var result = new List<FlaggedLinkDetail>();

            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();

                string linksQuery = @"
            SELECT url, Reasoning, hate_speech_content
            FROM dbo.Sources
            WHERE user_id = @UserId 
              AND IsArticle = 1 
              AND System_flag = 'automated' 
              AND isFlagged = 1";

                using (var cmd = new SqlCommand(linksQuery, con))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new FlaggedLinkDetail
                            {
                                Url = reader["url"].ToString(),
                                Reason = reader["Reasoning"] as string ?? "",
                                HateSpeech = reader["hate_speech_content"] as string ?? ""
                            });
                        }
                    }
                }
            }

            return result;
        }





        public bool DeleteAutomatedLink(string userId, string link)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM dbo.Sources WHERE user_id = @UserId AND url = @Link AND System_flag = 'automated'";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Link", link);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool UpdateReasoning(UpdateReasoningRequest request)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE dbo.Sources
                         SET Reasoning = @Reasoning
                         WHERE user_id = @UserId AND url = @Url AND System_flag = 'automated'";

                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Reasoning", request.Reasoning);
                    cmd.Parameters.AddWithValue("@UserId", request.UserId);
                    cmd.Parameters.AddWithValue("@Url", request.Url);

                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

public async Task<WebhookResult> SendWebhookAsync(string userId, string result)
    {
        result = result.ToLowerInvariant();

            Console.WriteLine(result);

            Console.WriteLine("heeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee");

            var payload = new WebhookPayload
        {
            Id = userId,
            Result = result,
            Evaluated_At = DateTime.UtcNow,
            Rejection_Reasons = new List<RejectionReason>(),
            Professional_Rejection_Reasons = new List<ReasonOnly>(),
            Social_Rejection_Reason = new List<ReasonOnly>(),
            Lyrical_Sources = new List<string>()
        };

        try
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            // Step 1: Get bio scores
            const string scoreQuery = @"
            SELECT Overall_score, Academic_score, Professional_score, Social_score,
                   professional_reason, Social_reason
            FROM dbo.user_bio_score
            WHERE User_id = @UserId";

            using (var cmd = new SqlCommand(scoreQuery, conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);

                using var reader = await cmd.ExecuteReaderAsync();
                if (!reader.HasRows)
                    return WebhookResult.NotFoundResult("User not found");

                await reader.ReadAsync();

                payload.Expert_Score = new ExpertScore
                {
                    Overall = Convert.ToInt32(reader["Overall_score"]),
                    Breakdown = new ScoreBreakdown
                    {
                        Academic_Credentials = Convert.ToInt32(reader["Academic_score"]),
                        Professional_Experience = Convert.ToInt32(reader["Professional_score"]),
                        Social_Media_Behavior = Convert.ToInt32(reader["Social_score"])
                    }
                };

                if (result == "rejected")
                {
                    var pro = reader["professional_reason"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(pro))
                        payload.Professional_Rejection_Reasons.Add(new ReasonOnly { Reason = pro });

                    var soc = reader["Social_reason"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(soc))
                        payload.Social_Rejection_Reason.Add(new ReasonOnly { Reason = soc });
                }
            }

            // Step 2: Get sources
            const string sourceQuery = @"
            SELECT Url, IsFlagged, Reasoning, Severity
            FROM dbo.Sources
            WHERE User_id = @UserId";

            using (var cmd2 = new SqlCommand(sourceQuery, conn))
            {
                cmd2.Parameters.AddWithValue("@UserId", userId);

                using var reader2 = await cmd2.ExecuteReaderAsync();
                while (await reader2.ReadAsync())
                {
                    var url = reader2["Url"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(url))
                        payload.Lyrical_Sources.Add(url);
                       var  T = System.Text.Json.JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
                    if (result == "rejected" && Convert.ToBoolean(reader2["IsFlagged"]))
                    {
                        payload.Rejection_Reasons.Add(new RejectionReason
                        {
                            Url = url,
                            Reasoning = reader2["Reasoning"]?.ToString(),
                            Severity = MapSeverity(Convert.ToInt32(reader2["Severity"]))
                        });
                    }
                }
            }

            // Step 3: Prepare to send webhook
            var webhookUrl = "https://dev.faithandmediaconnect.org/api/webhook/lyrical";
            var bearerToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJkbWNfdG9tIn0.CyQ53XL3L2Z3QAg12lOlMsNx15-C3gN-rKKNCDl53Lg"; // Secure this in config/env vars in production

            if (string.IsNullOrEmpty(webhookUrl) || string.IsNullOrEmpty(bearerToken))
                return WebhookResult.Fail("Webhook URL or token is not configured.");

            // Print payload to console
            Console.WriteLine("Sending Payload:");
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true }));

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await httpClient.PostAsJsonAsync(webhookUrl, payload);

            if (!response.IsSuccessStatusCode)
                return WebhookResult.Fail($"Webhook failed: {response.ReasonPhrase}");

            return WebhookResult.Success(payload);
        }
        catch (Exception ex)
        {
            return WebhookResult.Fail($"Internal server error: {ex.Message}");
        }
    }

    private string MapSeverity(int severity) =>
        severity >= 3 ? "high" : severity == 2 ? "medium" : "low";




}
}
