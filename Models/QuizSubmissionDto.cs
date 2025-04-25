using System.ComponentModel.DataAnnotations;

namespace MyUni.Models
{
public class QuizSubmissionDto
{
    public string time { get; set; }
    public ICollection<QuestionDto> QuizQuestions { get; set; }

}

public class QuestionDto
{
    public string Question { get; set; }
    public string CorrectAnswer { get; set; }
    public string UserAnswer { get; set; }
    public string Img { get; set; }
    public ICollection<BadAnswers> BadAnswers { get; set; }
}
public class BadAnswers 
{
    public string badanswer { get; set; }

}
}

