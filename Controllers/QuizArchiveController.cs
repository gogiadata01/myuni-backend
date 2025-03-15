using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YourNamespace.Data;
using YourNamespace.Models;

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
