namespace SocialMediaDetailAppBackend.Model
{
    public class UserDetail
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Title { get; set; }
        
        public Int32 AutoStatus { get; set; }
        public bool ManualStatus { get; set; }

        public string Biography { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Expertise { get; set; }
        public string Language { get; set; }
        public string LifeExperience { get; set; }
        public string Organization { get; set; }
        public DateTime CreatedDate { get; set; }

       
    }

    public class UserDecisionRequest
    {
        public string Id { get; set; }
        public string Result { get; set; } // “accepted” or “rejected”
    }

    public class WebhookPayload
    {
        public string Id { get; set; }
        public string Result { get; set; }
        public ExpertScore Expert_Score { get; set; }
        public List<RejectionReason> Rejection_Reasons { get; set; } = new();
        public List<ReasonOnly> Professional_Rejection_Reasons { get; set; } = new();
        public List<ReasonOnly> Social_Rejection_Reason { get; set; } = new();
        public List<string> Lyrical_Sources { get; set; } = new();
        public DateTime Evaluated_At { get; set; }
    }

    public class ExpertScore
    {
        public int Overall { get; set; }
        public ScoreBreakdown Breakdown { get; set; }
    }

    public class ScoreBreakdown
    {
        public int Academic_Credentials { get; set; }
        public int Professional_Experience { get; set; }
        public int Social_Media_Behavior { get; set; }
    }

    public class RejectionReason
    {
        public string Url { get; set; }
        public string Reasoning { get; set; }
        public string Severity { get; set; }
    }

    public class ReasonOnly
    {
        public string Reason { get; set; }
    }


    public class WebhookResult
    {
        public bool IsSuccess { get; set; }
        public bool NotFound { get; set; }
        public string ErrorMessage { get; set; }
        public object Payload { get; set; }

        // Factory methods to create instances
        public static WebhookResult Success(object payload) =>
            new WebhookResult { IsSuccess = true, Payload = payload };

        public static WebhookResult NotFoundResult(string error) =>
            new WebhookResult { NotFound = true, ErrorMessage = error };

        public static WebhookResult Fail(string error) =>
            new WebhookResult { IsSuccess = false, ErrorMessage = error };
    }

}
