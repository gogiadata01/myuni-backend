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
    public async Task<ActionResult<IEnumerable<QuizArchive>>> GetQuizArchive()
    {
        var quizArchives = await dbContext.MyQuizArchive
            .Include(q => q.Questions)
                .ThenInclude(q => q.CorrectAnswers)
            .Include(q => q.Questions)
                .ThenInclude(q => q.IncorrectAnswers)
            .Include(q => q.BonusQuestion)
            .ToListAsync();

        return Ok(dbContext);
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