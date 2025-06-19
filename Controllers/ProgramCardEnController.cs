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
[HttpGet("GetAllFieldNames")]
public async Task<IActionResult> GetAllFieldNames()
{
    var fieldNamesList = await dbContext.MyprogramCardEn
        .Include(card => card.Fields_en)
        .SelectMany(card => card.Fields_en
            .Select(field => new { FieldName_en = field.FieldName_en }))
        .ToListAsync();

    if (!fieldNamesList.Any())
    {
        return NotFound("No field names found");
    }

    return Ok(fieldNamesList);
}
[HttpGet("GetFields")]
public IActionResult GetAllFields()
{
    var fields = dbContext.MyprogramCardEn
        .SelectMany(card => card.Fields_en)
        .Select(field => new 
        {
            field.FieldName_en // Only get FieldName
        })
        .ToList();

    return Ok(fields);
}
 [HttpGet("{id}")]
public IActionResult GetProgramCardById(int id)
{
    var programCard = dbContext.MyprogramCardEn
        .Include(card => card.Fields_en)
            .ThenInclude(field => field.ProgramNames_en)
        .FirstOrDefault(card => card.Id == id);

    if (programCard == null)
    {
        return NotFound();
    }

    // Optionally select only the properties you need if you don't want the full entities
    var result = new 
    {
        programCard.Id,
        Fields_en = programCard.Fields_en.Select(field => new 
        {
            field.Id,
            field.FieldName_en,
            ProgramNames_en = field.ProgramNames_en.Select(program => new
            {
                program.Id,
                program.ProgramName_en
            }).ToList()
        }).ToList()
    };

    return Ok(result);
}
[HttpGet("byCheckboxName/{checkboxName}")]
public IActionResult GetProgramNameByCheckboxName(string checkboxName)
{
    var programNames = dbContext.MyprogramCardEn
        .Include(card => card.Fields_en)
            .ThenInclude(field => field.ProgramNames_en)
                .ThenInclude(program => program.CheckBoxes_en)
        .Where(card => card.Fields_en
            .Any(field => field.ProgramNames_en
                .Any(program => program.CheckBoxes_en
                    .Any(checkbox => checkbox.CheckBoxName_en == checkboxName))))
        .SelectMany(card => card.Fields_en)
        .SelectMany(field => field.ProgramNames_en)
        .Where(program => program.CheckBoxes_en
            .Any(checkbox => checkbox.CheckBoxName_en == checkboxName))
        .Select(program => new
        {
            program.Id,
            program.ProgramName_en
        })
        .ToList();

    if (programNames == null || !programNames.Any())
    {
        return NotFound();
    }

    return Ok(programNames);
}
[HttpPost("AddProgramCardEn")]
public IActionResult AddProgramCardEn([FromBody] ProgramCardEnDto addProgramCardDto)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    // Log incoming data
    Console.WriteLine("Received ProgramCardEnDto:");
    foreach (var field in addProgramCardDto.Fields_en)
    {
        Console.WriteLine($"FieldName_en: {field.FieldName_en}");
        foreach (var programName in field.ProgramNames_en)
        {
            Console.WriteLine($"ProgramName_en: {programName.ProgramName_en}");
            foreach (var checkBox in programName.CheckBoxes_en)
            {
                Console.WriteLine($"CheckBoxName_en: {checkBox.CheckBoxName_en}");
            }
        }
    }

    var programCardEntity = new ProgramCardEn
    {
        Fields_en = addProgramCardDto.Fields_en?.Select(f => new ProgramCardEn.FieldEn
        {
            FieldName_en = f.FieldName_en,
            ProgramNames_en = f.ProgramNames_en?.Select(p => new ProgramCardEn.ProgramNamesEn
            {
                ProgramName_en = p.ProgramName_en,
                CheckBoxes_en = p.CheckBoxes_en?.Select(c => new ProgramCardEn.CheckBoxesEn
                {
                    CheckBoxName_en = c.CheckBoxName_en
                }).ToList()
            }).ToList()
        }).ToList()
    };

    dbContext.MyprogramCardEn.Add(programCardEntity);
    dbContext.SaveChanges();

    return Ok(programCardEntity);
}
[HttpGet("byProgramName/{programname}")]
public IActionResult GetProgramCardByProgramName(string programname)
{
    var programCards = dbContext.MyprogramCardEn
        .Include(card => card.Fields_en)
            .ThenInclude(field => field.ProgramNames_en)
        .Where(card => card.Fields_en.Any(field => field.ProgramNames_en
                                          .Any(p => p.ProgramName_en == programname)))
        .ToList();

    if (programCards == null || !programCards.Any())
    {
        return NotFound();
    }

    // Optionally select only the properties you need if you don't want the full entities
    var result = programCards.Select(card => new
    {
        card.Id,
        Fields_en = card.Fields_en.Select(field => new
        {
            field.Id,
            field.FieldName_en,
            ProgramNames_en = field.ProgramNames_en
                .Where(p => p.ProgramName_en == programname)
                .Select(program => new
                {
                    program.Id,
                    program.ProgramName_en
                }).ToList()
        }).ToList()
    });

    return Ok(result);
}
        [HttpDelete("{id}")]
        public IActionResult DeleteProgramCard(int id)
        {
            // Find the ProgramCard by ID
            var programCard = dbContext.MyprogramCardEn
                .Include(card => card.Fields_en)
                    .ThenInclude(field => field.ProgramNames_en)
                        .ThenInclude(program => program.CheckBoxes_en)
                .FirstOrDefault(card => card.Id == id);

            // If the ProgramCard is not found, return a 404 Not Found response
            if (programCard == null)
            {
                return NotFound();
            }

            // Remove the ProgramCard from the databas{e
            dbContext.MyprogramCardEn.Remove(programCard);
            dbContext.SaveChanges();

            // Return a 204 No Content response to indicate successful deletion
            return NoContent();
        }
    }
    }

