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
[HttpGet("GetProgramsByField/{fieldName}")]
public IActionResult GetProgramsByField(string fieldName)
{
    var programs = dbContext.MyprogramCardEn
        .SelectMany(card => card.Fields_en)
        .Where(field => field.FieldName_en == fieldName)
        .Select(field => new 
        {
            ProgramNames_en = field.ProgramNames_en.Select(program => new
            {
                program.ProgramName_en // Only include programname
            }).ToList()
        })
        .FirstOrDefault();

    if (programs == null)
    {
        return NotFound();
    }

    return Ok(programs.ProgramNames_en);
} 
[HttpGet("getProgramCardWithProgramName/{programname}")]
public IActionResult GetProgramCardWithProgramName(string programname)
{
    // Find ProgramCard that has the searched program name
    var result = dbContext.MyprogramCardEn
        .Include(card => card.Fields_en)
            .ThenInclude(field => field.ProgramNames_en)
        .Where(card => card.Fields_en.Any(field => field.ProgramNames_en
            .Any(program => program.ProgramName_en == programname)))
        .SelectMany(card => card.Fields_en
            .Where(field => field.ProgramNames_en.Any(program => program.ProgramName_en == programname))
            .Select(field => new 
            {
                FieldName_en = field.FieldName_en,
                ProgramNames_en = field.ProgramNames_en
                    .Where(program => program.ProgramName_en == programname)
                    .Select(program => program.ProgramName_en)
                    .FirstOrDefault() // This ensures you only get the program name once
            }))
        .ToList();

    // If no matching records are found, return NotFound
    if (!result.Any())
    {
        return NotFound($"No program found with the name '{programname}'.");
    }

    return Ok(result);
}
[HttpGet("GetProgramCardDetailsBySubjects")]
public async Task<IActionResult> GetProgramCardDetailsBySubjects([FromQuery] List<string> subjects)
{
    if (subjects == null || !subjects.Any())
    {
        return BadRequest("Please provide at least one subject.");
    }

    var programCardDetails = await dbContext.MyprogramCardEn
        .Include(card => card.Fields_en)
            .ThenInclude(field => field.ProgramNames_en)
                .ThenInclude(program => program.CheckBoxes_en)
        .Select(card => new 
        {
            ProgramCardId = card.Id,
            ProgramNames_en = card.Fields_en.SelectMany(field => field.ProgramNames_en)
                .Where(program => program.CheckBoxes_en
                    .Any(checkBox => subjects.Contains(checkBox.CheckBoxName_en))) // FIXED LINE
                .Select(program => new 
                {
                    ProgramName_en = program.ProgramName_en,
                    CheckBoxes_en = program.CheckBoxes_en
                        .Where(checkBox => subjects.Contains(checkBox.CheckBoxName_en)) // FIXED LINE
                        .Select(checkBox => new 
                        {
                            CheckBoxName_en = checkBox.CheckBoxName_en
                        }).ToList()
                }).ToList()
        })
        .Where(card => card.ProgramNames_en.Any()) // Filter out cards with no matching programs
        .ToListAsync();

    if (programCardDetails == null || !programCardDetails.Any())
    {
        return NotFound("No matching programs found.");
    }

    return Ok(programCardDetails);
}


    }

}
