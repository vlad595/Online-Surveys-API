namespace Models
{
    public class Response
    {
        public int Id { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public int SurveyId { get; set; }
        public Survey? Survey { get; set; }

        public string? RespondentId { get; set; }
        public ApplicationUser? Respondent { get; set; }

        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}