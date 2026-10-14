using Microsoft.AspNetCore.Identity;

namespace Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Survey> CreatedSurveys { get; set; } = new List<Survey>();
        public ICollection<Response> SurveyResponses { get; set; } = new List<Response>();
    }
}