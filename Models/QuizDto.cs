namespace MyUni.Models.Entities
{
    public class Quiz
    {
        public int Id { get; set; }
        public string Time { get; set; }
        public ICollection<Question> Questions { get; set; }
        public BonusQuestionDetails BonusQuestion { get; set; }

        public class Question
        {
            public int Id { get; set; }
            public string question { get; set; }
            public string correctanswer { get; set; }
            public string img { get; set; }
            public ICollection<IncorrectAnswer> IncorrectAnswers { get; set; }
        }

        public class IncorrectAnswer
        {
            public int Id { get; set; }
            public string IncorrectAnswer { get; set; } // Fixed the typo here
        }

        public class BonusQuestionDetails
        {
            public int Id { get; set; }
            public string question { get; set; }
            public string correctanswer { get; set; }
            public string img { get; set; }
            public ICollection<IncorrectAnswer> IncorrectAnswers { get; set; }
            public int Coins { get; set; } = 3;
        }
    }
}
