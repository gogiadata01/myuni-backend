using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyUni.Data;
using MyUni.Models.Entities;
using MyUni.Models;

namespace MyUni.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgramCardEnController : ControllerBase

    {
        private readonly ApplicationDbContext dbContext;
        public ProgramCardEnController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;

        }
        [HttpGet]
public async Task<IActionResult> GetAllProgramCards()
{
    var allProgramCards = await dbContext.MyprogramCardEn
        .Include(card => card.Fields_en)
            .ThenInclude(field => field.ProgramNames_en)
        .ToListAsync();

    var result = allProgramCards.Select(card => new
    {
        Fields_en = card.Fields_en.Select(field => new 
        {
            field.FieldName_en, // Only include FieldName
            ProgramNames_en = field.ProgramNames_en.Select(program => new
            {
                program.Id,
                program.ProgramName_en // Only include programname
            }).ToList()
        }).ToList()
    });

    return Ok(result);
}
    }

}
