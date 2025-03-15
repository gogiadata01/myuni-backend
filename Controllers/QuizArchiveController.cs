using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyUni.Data;
using MyUni.Models;
using MyUni.Models.Entities;
using System;
using System.Globalization;
using System.Linq;


namespace MyUni.Controllers
{
[Route("api/[controller]")]
[ApiController]
public class QuizArchiveController : ControllerBase
{
        private readonly ApplicationDbContext dbContext;

    public QuizArchiveController(ApplicationDbContext dbContext)
    {
            this.dbContext = dbContext;
    }

    // GET: api/QuizArchive
//     [HttpGet]
// public ActionResult GetQuizArchive()
// {
//     var quizArchives = dbContext.MyQuizArchive.ToList();

//     return Ok(quizArchives);
// }

[HttpGet]
public ActionResult GetQuizArchive()
{
    var quizArchives = dbContext.MyQuizArchive
        .Include(qa => qa.Questions)
            .ThenInclude(q => q.IncorrectAnswers)
        .Include(qa => qa.BonusQuestion)
            .ThenInclude(bq => bq.CorrectAnswers)
        .Include(qa => qa.BonusQuestion)
            .ThenInclude(bq => bq.IncorrectAnswers)
        .ToList();

    return Ok(quizArchives);
}

[HttpPost]
public IActionResult ArchiveQuizzes()
{
    try
    {
        // Fetch quizzes from the database
        var quizzes = dbContext.MyQuiz
            .Include(q => q.Questions)
                .ThenInclude(q => q.IncorrectAnswers)
            .Include(q => q.BonusQuestion)
                .ThenInclude(b => b.CorrectAnswers)
            .Include(q => q.BonusQuestion)
                .ThenInclude(b => b.IncorrectAnswers)
            .ToList();

        // Check if quizzes are available
        if (quizzes == null || !quizzes.Any())
        {
            return BadRequest(new { message = "No quizzes available to archive." });
        }

        // Process each quiz
        var quizArchives = quizzes.Select(quizEntity =>
        {
            // Skip null quiz entities
            if (quizEntity == null) return null;

            // Archive the quiz data
            var archivedQuiz = new QuizArchive
            {
                Time = quizEntity.Time ?? "Unknown Time",
                Questions = quizEntity.Questions?.Select(q => new ArchivedQuestion
                {
                    question = q.question ?? "No question provided",
                    correctanswer = q.correctanswer ?? "No correct answer",
                    img = q.img ?? "No image",
                    IncorrectAnswers = q.IncorrectAnswers?.Select(ia => new ArchivedIncorrectAnswer
                    {
                        InccorectAnswer = ia.InccorectAnswer ?? "No incorrect answer"
                    }).ToList() ?? new List<ArchivedIncorrectAnswer>()
                }).ToList() ?? new List<ArchivedQuestion>(), // Ensure list is not null

                // Handle bonus question
                BonusQuestion = quizEntity.BonusQuestion != null ? new ArchivedBonusQuestionDetails
                {
                    question = quizEntity.BonusQuestion.question ?? "No bonus question",
                    img = quizEntity.BonusQuestion.img ?? "No bonus image",
                    Coins = quizEntity.BonusQuestion.Coins,
                    CorrectAnswers = quizEntity.BonusQuestion.CorrectAnswers?.Select(ca => new ArchivedCorrectAnswer
                    {
                        correctanswer = ca.correctanswer ?? "No correct answer"
                    }).ToList() ?? new List<ArchivedCorrectAnswer>(),

                    IncorrectAnswers = quizEntity.BonusQuestion.IncorrectAnswers?.Select(ia => new ArchivedIncorrectAnswer
                    {
                        InccorectAnswer = ia.InccorectAnswer ?? "No incorrect answer"
                    }).ToList() ?? new List<ArchivedIncorrectAnswer>()
                } : null // Handle null BonusQuestion gracefully
            };

            return archivedQuiz;
        }).Where(q => q != null).ToList(); // Filter out any null quizzes

        // If valid archives were found, add them to the archive database
        if (quizArchives.Any())
        {
            dbContext.MyQuizArchive.AddRange(quizArchives);
            dbContext.SaveChanges(); // Commit to the database
            return Ok(new { message = "Quizzes archived successfully." });
        }

        return BadRequest(new { message = "No valid quizzes to archive." });
    }
    catch (Exception ex)
    {
        // Handle any unexpected errors
        return StatusCode(500, new { message = "An error occurred while archiving quizzes.", error = ex.Message });
    }
}





    // DELETE: api/QuizArchive/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQuizArchive(int id)
    {
        var quizArchive = await dbContext.MyQuizArchive.FindAsync(id);
        if (quizArchive == null)
        {
            return NotFound();
        }

        dbContext.MyQuizArchive.Remove(quizArchive);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

}
}