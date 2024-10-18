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
    public class ProgramCardController : ControllerBase

    {
        private readonly ApplicationDbContext dbContext;
        public ProgramCardController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;

        }
[HttpGet]
public async Task<IActionResult> GetAllProgramCards()
{
    var allProgramCards = await dbContext.MyprogramCard
        .Include(card => card.Fields)
            .ThenInclude(field => field.ProgramNames)
        .ToListAsync();

    var result = allProgramCards.Select(card => new
    {
        Fields = card.Fields.Select(field => new 
        {
            field.FieldName, // Only include FieldName
            ProgramNames = field.ProgramNames.Select(program => new
            {
                program.id,
                program.programname // Only include programname
            }).ToList()
        }).ToList()
    });

    return Ok(result);
}
[HttpGet("getProgramCardWithProgramName/{programname}")]
public IActionResult GetProgramCardWithProgramName(string programname)
{
    // Find ProgramCard that has the searched program name
    var result = dbContext.MyprogramCard
        .Include(card => card.Fields)
            .ThenInclude(field => field.ProgramNames)
        .Where(card => card.Fields.Any(field => field.ProgramNames
            .Any(program => program.programname == programname)))
        .SelectMany(card => card.Fields
            .Where(field => field.ProgramNames.Any(program => program.programname == programname))
            .Select(field => new 
            {
                FieldName = field.FieldName,
                ProgramName = field.ProgramNames
                    .Where(program => program.programname == programname)
                    .Select(program => program.programname)
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
[HttpDelete("DeleteProgramName/{programId}")]
public async Task<IActionResult> DeleteProgramNameById(int programId)
{
    // Find the ProgramName by searching through the Fields and their ProgramNames
    var program = await dbContext.MyprogramCard
        .SelectMany(card => card.Fields.SelectMany(field => field.ProgramNames))
        .FirstOrDefaultAsync(p => p.Id == programId);

    if (program == null)
    {
        return NotFound($"Program with ID {programId} not found.");
    }

    // Remove the program from the database
    dbContext.Remove(program);

    // Save the changes to the database
    await dbContext.SaveChangesAsync();

    return Ok($"Program with ID {programId} deleted successfully.");
}




[HttpGet("GetProgramCardDetailsBySubjects")]
public async Task<IActionResult> GetProgramCardDetailsBySubjects([FromQuery] List<string> subjects)
{
    if (subjects == null || !subjects.Any())
    {
        return BadRequest("Please provide at least one subject.");
    }

    var programCardDetails = await dbContext.MyprogramCard
        .Include(card => card.Fields)
            .ThenInclude(field => field.ProgramNames)
                .ThenInclude(program => program.CheckBoxes)
        .Select(card => new 
        {
            ProgramCardId = card.Id,
            ProgramNames = card.Fields.SelectMany(field => field.ProgramNames)
                .Where(program => program.CheckBoxes
                    .Any(checkBox => subjects.Contains(checkBox.ChackBoxName)))
                .Select(program => new 
                {
                    ProgramName = program.programname,
                    CheckBoxes = program.CheckBoxes
                        .Where(checkBox => subjects.Contains(checkBox.ChackBoxName))
                        .Select(checkBox => new 
                        {
                            CheckBoxName = checkBox.ChackBoxName
                        }).ToList()
                }).ToList()
        }).Where(card => card.ProgramNames.Any()) // Filter out cards with no matching programs
        .ToListAsync();

    if (programCardDetails == null || !programCardDetails.Any())
    {
        return NotFound("No matching programs found.");
    }

    return Ok(programCardDetails);
}


[HttpGet("GetFieldProgram")]
public async Task<IActionResult> GetFieldProgram()
{
    var programData = await dbContext.MyprogramCard
        .Include(card => card.Fields)
            .ThenInclude(field => field.ProgramNames)
        .Select(card => new
        {
            cardId = card.Id, // Assuming there's an Id or some identifier for the card
            Fields = card.Fields.Select(field => new
            {
                fieldName = field.FieldName,
                ProgramNames = field.ProgramNames.Select(program => new
                {
                    programName = program.programname // Only include programname
                }).ToList()
            }).ToList()
        })
        .ToListAsync();

    if (programData.Count == 0)
    {
        return NotFound("No program data found");
    }

    return Ok(programData);
}


[HttpGet("GetFieldProgramNames")]
public async Task<IActionResult> GetFieldProgramNames()
{
    var programNamesList = await dbContext.MyprogramCard
        .Include(card => card.Fields)
            .ThenInclude(field => field.ProgramNames)
        .SelectMany(card => card.Fields.SelectMany(field => field.ProgramNames
            .Select(program => new { programName = program.programname }))
        )
        .ToListAsync();

    if (!programNamesList.Any())
    {
        return NotFound("No program names found");
    }

    return Ok(programNamesList);
}

[HttpGet("GetAllFieldNames")]
public async Task<IActionResult> GetAllFieldNames()
{
    var fieldNamesList = await dbContext.MyprogramCard
        .Include(card => card.Fields)
        .SelectMany(card => card.Fields
            .Select(field => new { fieldName = field.FieldName }))
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
    var fields = dbContext.MyprogramCard
        .SelectMany(card => card.Fields)
        .Select(field => new 
        {
            field.FieldName // Only get FieldName
        })
        .ToList();

    return Ok(fields);
}
[HttpGet("GetProgramsByField/{fieldName}")]
public IActionResult GetProgramsByField(string fieldName)
{
    var programs = dbContext.MyprogramCard
        .SelectMany(card => card.Fields)
        .Where(field => field.FieldName == fieldName)
        .Select(field => new 
        {
            ProgramNames = field.ProgramNames.Select(program => new
            {
                program.programname // Only include programname
            }).ToList()
        })
        .FirstOrDefault();

    if (programs == null)
    {
        return NotFound();
    }

    return Ok(programs.ProgramNames);
} 





 [HttpGet("{id}")]
public IActionResult GetProgramCardById(int id)
{
    var programCard = dbContext.MyprogramCard
        .Include(card => card.Fields)
            .ThenInclude(field => field.ProgramNames)
        .FirstOrDefault(card => card.Id == id);

    if (programCard == null)
    {
        return NotFound();
    }

    // Optionally select only the properties you need if you don't want the full entities
    var result = new 
    {
        programCard.Id,
        Fields = programCard.Fields.Select(field => new 
        {
            field.Id,
            field.FieldName,
            ProgramNames = field.ProgramNames.Select(program => new
            {
                program.Id,
                program.programname
            }).ToList()
        }).ToList()
    };

    return Ok(result);
}

[HttpGet("byProgramName/{programname}")]
public IActionResult GetProgramCardByProgramName(string programname)
{
    var programCards = dbContext.MyprogramCard
        .Include(card => card.Fields)
            .ThenInclude(field => field.ProgramNames)
        .Where(card => card.Fields.Any(field => field.ProgramNames
                                          .Any(p => p.programname == programname)))
        .ToList();

    if (programCards == null || !programCards.Any())
    {
        return NotFound();
    }

    // Optionally select only the properties you need if you don't want the full entities
    var result = programCards.Select(card => new
    {
        card.Id,
        Fields = card.Fields.Select(field => new
        {
            field.Id,
            field.FieldName,
            ProgramNames = field.ProgramNames
                .Where(p => p.programname == programname)
                .Select(program => new
                {
                    program.Id,
                    program.programname
                }).ToList()
        }).ToList()
    });

    return Ok(result);
}
[HttpGet("byCheckboxName/{checkboxName}")]
public IActionResult GetProgramNameByCheckboxName(string checkboxName)
{
    var programNames = dbContext.MyprogramCard
        .Include(card => card.Fields)
            .ThenInclude(field => field.ProgramNames)
                .ThenInclude(program => program.CheckBoxes)
        .Where(card => card.Fields
            .Any(field => field.ProgramNames
                .Any(program => program.CheckBoxes
                    .Any(checkbox => checkbox.ChackBoxName == checkboxName))))
        .SelectMany(card => card.Fields)
        .SelectMany(field => field.ProgramNames)
        .Where(program => program.CheckBoxes
            .Any(checkbox => checkbox.ChackBoxName == checkboxName))
        .Select(program => new
        {
            program.Id,
            program.programname
        })
        .ToList();

    if (programNames == null || !programNames.Any())
    {
        return NotFound();
    }

    return Ok(programNames);
}


        // POST: api/ProgramCard
        //[HttpPost]
        //public IActionResult AddProgramCard([FromBody] ProgramCardDto addProgramCardDto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var programCardEntity = new ProgramCard
        //    {
        //        Fields = addProgramCardDto.Fields?.Select(f => new ProgramCard.Field
        //        {
        //            FieldName = f.FieldName,
        //            ProgramNames = f.ProgramNames?.Select(p => new ProgramCard.ProgramNames
        //            {
        //                programname = p.programname,
        //                CheckBoxes = p.CheckBoxes?.Select(c => new ProgramCard.CheckBoxes
        //                {
        //                    ChackBoxName = c.ChackBoxName
        //                }).ToList()
        //            }).ToList()
        //        }).ToList()
        //    };

        //    dbContext.MyprogramCard.Add(programCardEntity);
        //    dbContext.SaveChanges();

        //    return Ok(programCardEntity);
        //}

        [HttpPost]
        public IActionResult AddProgramCard([FromBody] ProgramCardDto addProgramCardDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Log incoming data
            Console.WriteLine("Received ProgramCardDto:");
            foreach (var field in addProgramCardDto.Fields)
            {
                Console.WriteLine($"FieldName: {field.FieldName}");
                foreach (var programName in field.ProgramNames)
                {
                    Console.WriteLine($"ProgramName: {programName.programname}");
                    foreach (var checkBox in programName.CheckBoxes)
                    {
                        Console.WriteLine($"CheckBoxName: {checkBox.ChackBoxName}");
                    }
                }
            }

            var programCardEntity = new ProgramCard
            {
                Fields = addProgramCardDto.Fields?.Select(f => new ProgramCard.Field
                {
                    FieldName = f.FieldName,
                    ProgramNames = f.ProgramNames?.Select(p => new ProgramCard.ProgramNames
                    {
                        programname = p.programname,
                        CheckBoxes = p.CheckBoxes?.Select(c => new ProgramCard.CheckBoxes
                        {
                            ChackBoxName = c.ChackBoxName
                        }).ToList()
                    }).ToList()
                }).ToList()
            };

            dbContext.MyprogramCard.Add(programCardEntity);
            dbContext.SaveChanges();

            return Ok(programCardEntity);
        }
[HttpPut("UpdateFieldName/{fieldId}")]
public async Task<IActionResult> UpdateFieldName(int fieldId, [FromBody] string newFieldName)
{
    if (string.IsNullOrWhiteSpace(newFieldName))
    {
        return BadRequest("New field name cannot be empty.");
    }

    // Find the field by its ID
    var field = await dbContext.MyprogramCard
        .SelectMany(card => card.Fields)
        .FirstOrDefaultAsync(f => f.Id == fieldId);

    if (field == null)
    {
        return NotFound("Field not found.");
    }

    // Update the field's name
    field.FieldName = newFieldName;

    // Save changes to the database
    await dbContext.SaveChangesAsync();

    return Ok("Field name updated successfully.");
}

        [HttpDelete("{id}")]
        public IActionResult DeleteProgramCard(int id)
        {
            // Find the ProgramCard by ID
            var programCard = dbContext.MyprogramCard
                .Include(card => card.Fields)
                    .ThenInclude(field => field.ProgramNames)
                        .ThenInclude(program => program.CheckBoxes)
                .FirstOrDefault(card => card.Id == id);

            // If the ProgramCard is not found, return a 404 Not Found response
            if (programCard == null)
            {
                return NotFound();
            }

            // Remove the ProgramCard from the databas{e
            dbContext.MyprogramCard.Remove(programCard);
            dbContext.SaveChanges();

            // Return a 204 No Content response to indicate successful deletion
            return NoContent();
        }
    }

}
