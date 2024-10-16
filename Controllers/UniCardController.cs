using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyUni.Data;
using MyUni.Models;
using MyUni.Models.Entities;
using System.Xml;
using Newtonsoft.Json;

namespace MyUni.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UniCardController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        
        public UniCardController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext; 
        }

        [HttpGet]
        public IActionResult GetAllUniCard()
        {
            var AllUniCard = dbContext.MyUniCard
                .Select(card => new 
                {
                    Id = card.Id,        
                    Url = card.Url,    
                    Title = card.Title, 
                    MainText = card.MainText 
                })
                .ToList();

            return Ok(AllUniCard);
        }

        [HttpGet("{id}")]
        public IActionResult GetUniCardDetailsById(int id)
        {
            var UniCard = dbContext.MyUniCard
                .Include(card => card.Events)
                .Include(card => card.Sections)
                    .ThenInclude(section => section.ProgramNames)
                .Where(card => card.Id == id)
                .Select(card => new
                {
                    Id = card.Id,
                    Url = card.Url,
                    Title = card.Title,
                    MainText = card.MainText,
                    History = card.History,
                    ForPupil = card.ForPupil,
                    ScholarshipAndFunding = card.ScholarshipAndFunding,
                    ExchangePrograms = card.ExchangePrograms,
                    Labs = card.Labs,
                    StudentsLife = card.StudentsLife,
                    PaymentMethods = card.PaymentMethods,
                    Events = card.Events.Select(e => new
                    {
                        e.Id,
                        e.Title,
                        e.Text,
                        e.Time
                    }).ToList(),
                    Sections = card.Sections.Select(s => new
                    {
                        s.Id,
                        s.Title,
                        ProgramNames = s.ProgramNames.Select(pn => new
                        {
                            pn.Id,
                            pn.ProgramName
                        }).ToList()
                    }).ToList()
                })
                .FirstOrDefault();

            if (UniCard is null)
            {
                return NotFound();
            }

            return Ok(UniCard);
        }

        [HttpPost]
        public IActionResult AddUniCard([FromBody] UniCardDto addUniCardDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Log received data
            Console.WriteLine(JsonConvert.SerializeObject(addUniCardDto, Newtonsoft.Json.Formatting.Indented));

            // Existing code to map and save entity...
            var UniCardEntity = new UniCard
            {
                Url = addUniCardDto.url,
                Title = addUniCardDto.title,
                MainText = addUniCardDto.mainText,
                History = addUniCardDto.history,
                ForPupil = addUniCardDto.forPupil,
                ScholarshipAndFunding = addUniCardDto.scholarshipAndFunding,
                ExchangePrograms = addUniCardDto.exchangePrograms,
                Labs = addUniCardDto.labs,
                StudentsLife = addUniCardDto.studentsLife,
                PaymentMethods = addUniCardDto.paymentMethods,
                Events = addUniCardDto.events?.Select(e => new UniCard.Event
                {
                    Url = e.url,
                    Title = e.title,
                    Text = e.text
                }).ToList(),
                Sections = addUniCardDto.sections?.Select(s => new UniCard.Section
                {
                    Title = s.title,
                    ProgramNames = s.programNames?.Select(p => new UniCard.Programname
                    {
                        ProgramName = p.programName,
                        Jobs = p.Jobs,
                        SwavlebisEna = p.SwavlebisEna,
                        Kvalifikacia = p.Kvalifikacia,
                        Dafinanseba = p.Dafinanseba,
                        KreditebisRaodenoba = p.KreditebisRaodenoba,
                        AdgilebisRaodenoba = p.AdgilebisRaodenoba,
                        Fasi = p.Fasi,
                        Kodi = p.Kodi,
                        ProgramisAgwera = p.ProgramisAgwera,
                    }).ToList()
                }).ToList(),
                Sections2 = addUniCardDto.sections2?.Select(s2 => new UniCard.Section2
                {
                    Title = s2.title,
                    SavaldebuloSagnebi = s2.savaldebuloSagnebi?.Select(ss => new UniCard.SavaldebuloSagnebi
                    {
                        SagnisSaxeli = ss.sagnisSaxeli,
                        Koeficienti = ss.koeficienti,
                        MinimaluriZgvari = ss.minimaluriZgvari,
                        Prioriteti = ss.prioriteti,
                        AdgilebisRaodenoba = ss.AdgilebisRaodenoba,
                    }).ToList()
                }).ToList(),
                ArchevitiSavaldebuloSaganebi = addUniCardDto.archevitiSavaldebuloSaganebi?.Select(a => new UniCard.ArchevitiSavaldebuloSagani
                {
                    Title = a.title,
                    ArchevitiSavaldebuloSagnebi = a.archevitiSavaldebuloSagnebi?.Select(asb => new UniCard.ArchevitiSavaldebuloSagnebi
                    {
                        SagnisSaxeli = asb.sagnisSaxeli,
                        Koeficienti = asb.koeficienti,
                        MinimaluriZgvari = asb.minimaluriZgvari,
                        Prioriteti = asb.prioriteti,
                        AdgilebisRaodenoba = asb.AdgilebisRaodenoba
                    }).ToList()
                }).ToList()
            };

            dbContext.MyUniCard.Add(UniCardEntity);
            dbContext.SaveChanges();

            return Ok(UniCardEntity);
        }

[HttpGet("search")]
public IActionResult GetUniCardByTitleAndProgramName([FromQuery] string title, [FromQuery] string programName)
{
    try
    {
        var result = dbContext.MyUniCard
            .Include(card => card.Sections)
                .ThenInclude(section => section.ProgramNames)
            .Where(card => card.Title == title &&
                           card.Sections.Any(section => section.ProgramNames
                                                        .Any(program => program.ProgramName == programName)))
            .Select(card => new
            {
                card.Title, // Only returning Title for UniCard
                Sections = card.Sections
                    .Where(section => section.ProgramNames.Any(program => program.ProgramName == programName)) // Filter sections to only include those with matching program names
                    .Select(section => new
                    {
                        section.Title, // Only returning Title for Sections
                        ProgramNames = section.ProgramNames
                            .Where(program => program.ProgramName == programName) // Select only the matching program name
                            .Select(program => new
                            {
                                program.ProgramName,
                                program.Jobs,
                                program.SwavlebisEna,
                                program.Kvalifikacia,
                                program.Dafinanseba,
                                program.KreditebisRaodenoba,
                                program.AdgilebisRaodenoba,
                                program.Fasi,
                                program.Kodi,
                                program.ProgramisAgwera,
                                program.Mizani // Include the new Mizani field
                            }).ToList()
                    }).ToList()
            })
            .ToList();

        if (!result.Any())
        {
            return NotFound();
        }

        return Ok(result);
    }
    catch (Exception ex)
    {
        // Log the exception (you might use a logging framework)
        Console.Error.WriteLine($"Error: {ex.Message}");
        return StatusCode(500, "Internal server error");
    }
}
[HttpGet("searchByTitleMainTextUrl")]
public IActionResult GetUniCardByTitleMainTextUrl([FromQuery] string title)
{
    try
    {
        // Fetch UniCards that match the title and select only the required fields
        var result = dbContext.MyUniCard
            .Where(card => card.Title.Contains(title)) // Searching with partial match (use .Equals() if you want exact match)
            .Select(card => new
            {
                card.Title,
                card.MainText,
                card.Url
            })
            .ToList();

        if (!result.Any())
        {
            return NotFound("No UniCards found with the specified title.");
        }

        return Ok(result);
    }
    catch (Exception ex)
    {
        // Log the exception if needed
        Console.Error.WriteLine($"Error: {ex.Message}");
        return StatusCode(500, "Internal server error");
    }
}

[HttpGet("searchById")]
public IActionResult GetUniCardByIdAndProgramName([FromQuery] int id, [FromQuery] string programName)
{
    try
    {
        var result = dbContext.MyUniCard
            .Include(card => card.Sections)
                .ThenInclude(section => section.ProgramNames)
            .Where(card => card.Id == id &&  // Filter by Id instead of Title
                           card.Sections.Any(section => section.ProgramNames
                                                        .Any(program => program.ProgramName == programName)))
            .Select(card => new
            {
                card.Id, // Returning Id for UniCard
                card.Title, // Optionally, return Title for UniCard
                Sections = card.Sections
                    .Where(section => section.ProgramNames.Any(program => program.ProgramName == programName)) // Filter sections to only include those with matching program names
                    .Select(section => new
                    {
                        section.Title, // Returning Title for Sections
                        ProgramNames = section.ProgramNames
                            .Where(program => program.ProgramName == programName) // Select only the matching program name
                            .Select(program => new
                            {
                                program.ProgramName,
                                program.Jobs,
                                program.SwavlebisEna,
                                program.Kvalifikacia,
                                program.Dafinanseba,
                                program.KreditebisRaodenoba,
                                program.AdgilebisRaodenoba,
                                program.Fasi,
                                program.Kodi,
                                program.ProgramisAgwera,
                                program.Mizani // Include the new Mizani field
                            }).ToList()
                    }).ToList()
            })
            .ToList();

        if (!result.Any())
        {
            return NotFound();
        }

        return Ok(result);
    }
    catch (Exception ex)
    {
        // Log the exception (you might use a logging framework)
        Console.Error.WriteLine($"Error: {ex.Message}");
        return StatusCode(500, "Internal server error");
    }
}


[HttpGet("by-program-name")]
public IActionResult GetUniCardByProgramName(string programName)
{
    var uniCard = dbContext.MyUniCard
    .AsNoTracking() // Add this line
        .Include(card => card.Sections)
            .ThenInclude(section => section.ProgramNames)
        .Where(card => card.Sections
            .Any(section => section.ProgramNames
                .Any(pn => pn.ProgramName == programName)))
        .Select(card => new 
        {
            Id = card.Id,
            Url = card.Url,
            Title = card.Title,
            MainText = card.MainText,
            // Only select the matched programName
            ProgramNames = card.Sections.SelectMany(s => s.ProgramNames
                .Where(pn => pn.ProgramName == programName)
                .Select(pn => new { pn.Id, pn.ProgramName })).ToList()
        })
        .ToList();

    if (!uniCard.Any())
    {
        return NotFound();
    }

    return Ok(uniCard);
}


        [HttpPut("{id}/update-url")]
        public IActionResult UpdateUrl(int id, [FromBody] string newUrl)
        {
            if (string.IsNullOrWhiteSpace(newUrl))
            {
                return BadRequest("URL cannot be null or empty.");
            }

            var uniCard = dbContext.MyUniCard.FirstOrDefault(card => card.Id == id);

            if (uniCard == null)
            {
                return NotFound($"No UniCard found with ID {id}.");
            }

            uniCard.Url = newUrl;

            dbContext.SaveChanges();

            return Ok(uniCard);
        }
[HttpPut("{uniCardId}/event/{eventId}")]
public IActionResult UpdateEventCard(int uniCardId, int eventId, [FromBody] EventDto updatedEvent)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    // Find the UniCard by Id
    var uniCard = dbContext.MyUniCard
        .Include(card => card.Events)
        .FirstOrDefault(card => card.Id == uniCardId);

    if (uniCard == null)
    {
        return NotFound($"No UniCard found with ID {uniCardId}.");
    }

    // Find the Event by eventId
    var eventCard = uniCard.Events.FirstOrDefault(e => e.Id == eventId);

    if (eventCard == null)
    {
        return NotFound($"No Event found with ID {eventId}.");
    }

    // Update the event properties
    eventCard.Url = updatedEvent.Url;
    eventCard.Title = updatedEvent.Title;
    eventCard.Text = updatedEvent.Text;
    eventCard.Time = updatedEvent.Time;

    // Save changes to the database
    dbContext.SaveChanges();

    return Ok(eventCard);
}
[HttpPut("{id}/update-maintext")]
public IActionResult UpdateMainText(int id, [FromBody] string newMainText)
{
    if (string.IsNullOrWhiteSpace(newMainText))
    {
        return BadRequest("MainText cannot be null or empty.");
    }

    var uniCard = dbContext.MyUniCard.FirstOrDefault(card => card.Id == id);

    if (uniCard == null)
    {
        return NotFound($"No UniCard found with ID {id}.");
    }

    // Update the main text
    uniCard.MainText = newMainText;

    dbContext.SaveChanges();

    return Ok(uniCard);
}

 [HttpPost("{uniCardId}/event")]
public IActionResult PostEventCard(int uniCardId, [FromBody] EventDto newEvent)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    // Find the UniCard by Id
    var uniCard = dbContext.MyUniCard
        .Include(card => card.Events)
        .FirstOrDefault(card => card.Id == uniCardId);

    if (uniCard == null)
    {
        return NotFound($"No UniCard found with ID {uniCardId}.");
    }

    // Create a new event and add it to the UniCard
    var eventCard = new UniCard.Event
    {
        Url = newEvent.Url,
        Title = newEvent.Title,
        Text = newEvent.Text,
        Time = newEvent.Time
    };

    uniCard.Events.Add(eventCard);

    // Save changes to the database
    dbContext.SaveChanges();

    return Ok(eventCard);
}

        [HttpDelete("{id}")]
        public IActionResult DeleteUniCard(int id)
        {
            var uniCard = dbContext.MyUniCard
                .Include(card => card.Events)
                .Include(card => card.Sections)
                    .ThenInclude(section => section.ProgramNames)
                .Include(card => card.Sections2)
                    .ThenInclude(section2 => section2.SavaldebuloSagnebi)
                .Include(card => card.ArchevitiSavaldebuloSaganebi)
                    .ThenInclude(archeviti => archeviti.ArchevitiSavaldebuloSagnebi)
                .FirstOrDefault(card => card.Id == id);

            if (uniCard == null)
            {
                return NotFound();
            }

            dbContext.MyUniCard.Remove(uniCard);
            dbContext.SaveChanges();

            return NoContent();
        }
    }
}




