using System;

namespace Models
{
    public class Answer
    {
        public int Id { get; set; }

        public int ResponseId { get; set; }
        public Response? Response { get; set; }

        public int QuestionId { get; set; }
        public Question? Question { get; set; }

        public int? SelectedOptionId { get; set; }
        public Option? SelectedOption { get; set; }

        public string? TextAnswer { get; set; }
    }
}