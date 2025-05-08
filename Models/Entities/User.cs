using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace MyUni.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public string Type { get; set; }
        public string Img { get; set; }
        public int Coin { get; set; }
        public string Token { get; set; } // Token for password reset
        public DateTime? ResetTokenExpiry { get; set; } // Token expiration time
        public DateTime? LastQuizAttempt { get; set; } // Nullable for users who haven't attempted a quiz
        public int RemainingTime { get; set; } // In seconds
        public ICollection<QuizHistory> Quizes { get; set; }  = new List<QuizHistory>();

        public class QuizHistory 
        {     
        public int Id { get; set; }
        public string QuizDate { get; set; } // e.g., '10 მაისი'
        public string QuizTime { get; set; } // e.g., '15:00'
        public string time { get; set; }
        public ICollection<questions> QuizQuestions { get; set; }
        }

        public class questions
        {
            public int Id { get; set; }
            public string question { get; set; }
            public string correctanswer { get; set; }
            public string UserAnswer {get; set; }
            public string? img { get; set; } = null;
            public ICollection<BadAnswer> BadAnswers { get; set; }
        }
        public class BadAnswer
        {
            public int Id { get; set; }
            public string badanswer { get; set; } 
        }
        public DateTime? LastLogin { get; set; } 
        public DateTime? LastCoinAwardTime { get; set; }  

    }

}
