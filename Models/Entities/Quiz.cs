namespace MyUni.Models.Entities
{
    public class QuizArchive
    {
        public int Id { get; set; }
        public string Time { get; set; }
        public ICollection<ArchivedQuestion> Questions { get; set; }
        public int? BonusQuestionId { get; set; }
        public ArchivedBonusQuestionDetails? BonusQuestion { get; set; }
    }

    public class ArchivedQuestion
    {
        public int Id { get; set; }
        public string question { get; set; }
        public string correctanswer { get; set; }
        public string img { get; set; } = null;
        public int QuizArchiveId { get; set; }
        public QuizArchive QuizArchive { get; set; }
        public ICollection<ArchivedIncorrectAnswer> IncorrectAnswers { get; set; }
    }

    public class ArchivedIncorrectAnswer
    {
        public int Id { get; set; }
        public string InccorectAnswer { get; set; }
        public int ArchivedQuestionId { get; set; }
        public ArchivedQuestion ArchivedQuestion { get; set; }
    }

    public class ArchivedBonusQuestionDetails
    {
        public int Id { get; set; }
        public string? question { get; set; }
        public string? img { get; set; }
        public int Coins { get; set; } = 3;

        public ICollection<ArchivedCorrectAnswer> CorrectAnswers { get; set; }
        public ICollection<ArchivedIncorrectAnswer> IncorrectAnswers { get; set; }
    }

    public class ArchivedCorrectAnswer
    {
        public int Id { get; set; }
        public string correctanswer { get; set; }
        public int BonusQuestionDetailsId { get; set; }
        public ArchivedBonusQuestionDetails BonusQuestion { get; set; }
    }
}
