namespace MyUni.Models.Entities
{
    public class Quiz
    {
        public int Id { get; set; }
        public string Time { get; set; }
        public ICollection<Question> Questions { get; set; }
        public BonusQuestionDetails BonusQuestion { get; set; } // Match this name with the controller

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
            public string InccorectAnswer { get; set; } // Ensure spelling is consistent
        }

        public class BonusQuestionDetails // Use this class name consistently in both model and controller
        {
            public int Id { get; set; }
            public string question { get; set; }
            public ICollection<correctanswers> CorrectAnswers { get; set; }
            public string img { get; set; }
            public ICollection<IncorrectAnswer> IncorrectAnswers { get; set; }
            public int Coins { get; set; } = 3; // Bonus question worth 3 coins
        }
        public class correctanswers {
            public int Id { get; set; }
            public string correctanswer { get; set; }
        }
    }
}

