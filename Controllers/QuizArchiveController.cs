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
        dbContext = dbContext;
    }

    // GET: api/QuizArchive
    [HttpGet]
    public ActionResult GetQuizArchive()
    {
        var quizArchives =  dbContext.MyQuizArchive
                .Include(q => q.Questions)
                    .ThenInclude(qa => qa.IncorrectAnswers)
                .Include(q => q.BonusQuestion)
                    .ThenInclude(bq => bq.CorrectAnswers)
                .Include(q => q.BonusQuestion)
                    .ThenInclude(bq => bq.IncorrectAnswers)
                .ToList();

        return Ok(dbContext);
    }

[HttpPost]
public IActionResult ArchiveQuizzes()
{
    try
    {
        // Fetch quizzes from the database
        var quizzes = dbContext.MyQuiz
            .Include(q => q.Questions)
                .ThenInclude(qa => qa.IncorrectAnswers)
            .Include(q => q.BonusQuestion)
                .ThenInclude(bq => bq.CorrectAnswers)
            .Include(q => q.BonusQuestion)
                .ThenInclude(bq => bq.IncorrectAnswers)
            .ToList();

        // Check if quizzes are found, otherwise return a specific message
        if (quizzes == null || !quizzes.Any())
        {
            return BadRequest(new { message = "No quizzes available to archive." });
        }

        var quizArchives = quizzes.Select(quizEntity =>
        {
            if (quizEntity == null)
            {
                return null; // If the quiz entity is null, skip it
            }

            return new QuizArchive
            {
                Time = quizEntity.Time,
                Questions = quizEntity.Questions?.Select(q => new ArchivedQuestion
                {
                    question = q.question,
                    correctanswer = q.correctanswer,
                    img = q.img,
                    IncorrectAnswers = q.IncorrectAnswers?.Select(ia => new ArchivedIncorrectAnswer
                    {
                        InccorectAnswer = ia.InccorectAnswer
                    }).ToList() ?? new List<ArchivedIncorrectAnswer>()
                }).ToList() ?? new List<ArchivedQuestion>(), // Ensure it's not null

                BonusQuestion = quizEntity.BonusQuestion != null ? new ArchivedBonusQuestionDetails
                {
                    question = quizEntity.BonusQuestion.question,
                    img = quizEntity.BonusQuestion.img,
                    Coins = quizEntity.BonusQuestion.Coins,
                    CorrectAnswers = quizEntity.BonusQuestion.CorrectAnswers?.Select(ca => new ArchivedCorrectAnswer
                    {
                        correctanswer = ca.correctanswer
                    }).ToList() ?? new List<ArchivedCorrectAnswer>(),

                    IncorrectAnswers = quizEntity.BonusQuestion.IncorrectAnswers?.Select(ia => new ArchivedIncorrectAnswer
                    {
                        InccorectAnswer = ia.InccorectAnswer
                    }).ToList() ?? new List<ArchivedIncorrectAnswer>()
                } : null // Handle the case where there is no BonusQuestion
            };
        }).Where(q => q != null).ToList(); // Filter out any null quizzes

        // Check if any valid archives were found to add
        if (quizArchives.Any())
        {
            dbContext.MyQuizArchive.AddRange(quizArchives);
            dbContext.SaveChanges();  // Commit the transaction to the database
        }
        else
        {
            return BadRequest(new { message = "No quizzes found to archive." });
        }

        return Ok(new { message = "Quizzes archived successfully." });
    }
    catch (Exception ex)
    {
        // Log error for debugging purposes (optional logging)
        // _logger.LogError(ex, "Error while archiving quizzes.");
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