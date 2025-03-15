namespace MyUni.Models.Entities
{
    public class QuizArchive
    {
        public int Id { get; set; }
        public string Time { get; set; }
        public ICollection<Question> Questions { get; set; }
        public BonusQuestionDetails? BonusQuestion { get; set; }

        public class Question
        {
            public int Id { get; set; }
            public string question { get; set; }
            public string correctanswer { get; set; }
            public string img { get; set; } = null;
            public ICollection<IncorrectAnswer> IncorrectAnswers { get; set; }
        }

        public class IncorrectAnswer
        {
            public int Id { get; set; }
            public string InccorectAnswer { get; set; }
        }

        public class BonusQuestionDetails
        {
            public int Id { get; set; }
            public string? question { get; set; }
            public string? img { get; set; }
            public ICollection<correctanswers>? CorrectAnswers { get; set; }
            public ICollection<IncorrectAnswer>? IncorrectAnswers { get; set; }
            public int Coins { get; set; } = 3;
        }

        public class correctanswers
        {
            public int Id { get; set; }
            public string correctanswer { get; set; }
        }
    }
}
