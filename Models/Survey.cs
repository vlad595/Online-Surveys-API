using System;

namespace Models
{
    public class Survey
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = "Загальне";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsPublic { get; set; } = true;
        public bool IsApproved { get; set; } = false;
        public bool IsActive { get; set; } = true;

        public string? AuthorId { get; set; }
        public ApplicationUser? Author { get; set; }

        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<Response> Responses { get; set; } = new List<Response>();
    }
}