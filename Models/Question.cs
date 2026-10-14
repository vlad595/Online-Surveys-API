using System;

namespace Models
{
    public enum QuestionType
    {
        SingleChoice = 0,
        MultipleChoice = 1,
        OpenText = 2
    }

    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public QuestionType Type { get; set; } = QuestionType.SingleChoice;
        public int Order { get; set; } = 1;

        public int SurveyId { get; set; }
        public Survey? Survey { get; set; }

        public ICollection<Option> Options { get; set; } = new List<Option>();
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}