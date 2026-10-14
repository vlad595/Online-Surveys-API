namespace DTO
{
    public class SurveyReadDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsPublic { get; set; }
        public bool IsApproved { get; set; }
        public bool IsActive { get; set; }
        public string AuthorName { get; set; } = "Невідомий";
        public int QuestionsCount { get; set; }
        public int ResponsesCount { get; set; }
        public List<QuestionReadDto> Questions { get; set; } = new();
    }

    public class QuestionReadDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new();
    }
}