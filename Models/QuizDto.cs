namespace MyUni.Models
{
    public class QuizDto
    {
        public string Time { get; set; }
        public ICollection<Question> Questions { get; set; }
        public BonusQuestionDetails BonusQuestion { get; set; }

        public class Question
        {
            public string question { get; set; }
            public string correctanswer { get; set; }
            public string img { get; set; } = null;
            public ICollection<IncorrectAnswer> IncorrectAnswers { get; set; }
        }

        public class IncorrectAnswer
        {
            public string Answer { get; set; }  // Fixed typo here
        }

        public class BonusQuestionDetails
        {
            public string question { get; set; }
            public ICollection<correctanswers> CorrectAnswers { get; set; }
            public string img { get; set; }
            public ICollection<IncorrectAnswer> IncorrectAnswers { get; set; }
            public int Coins { get; set; } = 3; // Bonus question worth 3 coins
        }
             
        public class correctanswers {
            public string correctanswer { get; set; }
        }
    }
}
