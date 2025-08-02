using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyUni.Data;
using MyUni.Models;
using MyUni.Models.Entities;
using System.Xml;
using Newtonsoft.Json;
using MyUni.Controllers;

namespace MyUni.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnicardEnController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        public UnicardEnController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext; 

        }
        [HttpGet]
        public IActionResult GetAllUniCard()
        {
            var AllUniCard = dbContext.MyUniCardEn
                .Select(card => new 
                {
                    Id = card.Id,        
                    Url_en = card.Url_en,    
                    Title_en = card.Title_en, 
                    MainText_en = card.MainText_en,
                    History_en= card.History_en,
                    ForPupil_en = card.ForPupil_en,
                    ScholarshipAndFunding_en = card.ScholarshipAndFunding_en,
                    ExchangePrograms_en = card.ExchangePrograms_en,
                    Labs_en = card.Labs_en,
                    StudentsLife_en = card.StudentsLife_en,
                    PaymentMethods_en = card.PaymentMethods_en,
                    // Events = card.Events,
                    // Sections = card.Sections,
                    // Sections2 = card.Sections2,
                    ArchevitiSavaldebuloSaganebi_en = card.ArchevitiSavaldebuloSaganebi_en,
                    Events_en = card.Events_en.Select(e => new
                    {
                        e.Id,
                        e.Url_en,
                        e.Title_en,
                        e.Time_en,
                        e.Link_en
                    }).ToList(),
                    Section_en = card.Section_en.Select(s => new
                    {
                        s.Id,
                        s.Title_en,
                        ProgramNames_en = s.ProgramNames_en.Select(pn => new
                        {
                            pn.Id,
                            pn.ProgramName_en,
                            pn.Jobs_en,
                            pn.SwavlebisEna_en,
                            pn.Kvalifikacia_en,
                            pn.Dafinanseba_en,
                            pn.KreditebisRaodenoba_en,
                            pn.AdgilebisRaodenoba_en,
                            pn.Fasi_en,
                            pn.Kodi_en,
                            pn.ProgramisAgwera_en,
                        }).ToList()
                    }).ToList()
                })
                .ToList();

            return Ok(AllUniCard);
        }

[HttpPost]
public IActionResult AddUniCardEn([FromBody] UnicardEnDto addUniCardDto)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    Console.WriteLine(JsonConvert.SerializeObject(addUniCardDto, Newtonsoft.Json.Formatting.Indented));

    var uniCardEntity = new UnicardEn
    {
        Url_en = addUniCardDto.Url_en,
        Title_en = addUniCardDto.Title_en,
        MainText_en = addUniCardDto.MainText_en,
        History_en = addUniCardDto.History_en,
        ForPupil_en = addUniCardDto.ForPupil_en,
        ScholarshipAndFunding_en = addUniCardDto.ScholarshipAndFunding_en,
        ExchangePrograms_en = addUniCardDto.ExchangePrograms_en,
        Labs_en = addUniCardDto.Labs_en,
        StudentsLife_en = addUniCardDto.StudentsLife_en,
        PaymentMethods_en = addUniCardDto.PaymentMethods_en,
        Events_en = addUniCardDto.Events_en?.Select(e => new UnicardEn.Event_en
        {
            Url_en = e.Url_en,
            Title_en = e.Title_en,
            Text_en = e.Text_en,
            Time_en = e.Time_en,
            Link_en = e.Link_en
        }).ToList(),
        Sections_en = addUniCardDto.Sections_en?.Select(s => new UnicardEn.Section_en
        {
            Title_en = s.Title_en,
            ProgramNames_en = s.ProgramNames_en?.Select(p => new UnicardEn.Programname_en
            {
                ProgramName_en = p.ProgramName_en,
                Jobs_en = p.Jobs_en,
                SwavlebisEna_en = p.SwavlebisEna_en,
                Kvalifikacia_en = p.Kvalifikacia_en,
                Dafinanseba_en = p.Dafinanseba_en,
                KreditebisRaodenoba_en = p.KreditebisRaodenoba_en,
                AdgilebisRaodenoba_en = p.AdgilebisRaodenoba_en,
                Fasi_en = p.Fasi_en,
                Kodi_en = p.Kodi_en,
                ProgramisAgwera_en = p.ProgramisAgwera_en
            }).ToList()
        }).ToList(),
        Sections2_en = addUniCardDto.Sections2_en?.Select(s2 => new UnicardEn.Section2_en
        {
            Title_en = s2.Title_en,
            SavaldebuloSagnebi_en = s2.SavaldebuloSagnebi_en?.Select(ss => new UnicardEn.SavaldebuloSagnebi_en
            {
                SagnisSaxeli_en = ss.SagnisSaxeli_en,
                Koeficienti_en = ss.Koeficienti_en,
                MinimaluriZgvari_en = ss.MinimaluriZgvari_en,
                Prioriteti_en = ss.Prioriteti_en,
                AdgilebisRaodenoba_en = ss.AdgilebisRaodenoba_en
            }).ToList()
        }).ToList(),
        ArchevitiSavaldebuloSaganebi_en = addUniCardDto.ArchevitiSavaldebuloSaganebi_en?.Select(a => new UnicardEn.ArchevitiSavaldebuloSagani_en
        {
            Title_en = a.Title_en,
            ArchevitiSavaldebuloSagnebi_en = a.ArchevitiSavaldebuloSagnebi_en?.Select(asb => new UnicardEn.ArchevitiSavaldebuloSagnebi_en
            {
                SagnisSaxeli_en = asb.SagnisSaxeli_en,
                Koeficienti_en = asb.Koeficienti_en,
                MinimaluriZgvari_en = asb.MinimaluriZgvari_en,
                Prioriteti_en = asb.Prioriteti_en,
                AdgilebisRaodenoba_en = asb.AdgilebisRaodenoba_en
            }).ToList()
        }).ToList()
    };

    dbContext.MyUniCardEn.Add(uniCardEntity);
    dbContext.SaveChanges();

    return Ok(uniCardEntity);
}

        [HttpGet("{id}")]
        public IActionResult GetUniCardDetailsById(int id)
        {
            var UniCard = dbContext.MyUniCardEn
                .Include(card => card.Events_en)
                .Include(card => card.Sections_en)
                    .ThenInclude(section => section.ProgramNames_en)
                .Where(card => card.Id == id)
                .Select(card => new
                {
                    Id = card.Id,
                    Url_en = card.Url_en,
                    Title_en = card.Title_en,
                    MainText_en = card.MainText_en,
                    History_en = card.History_en,
                    ForPupil_en = card.ForPupil_en,
                    ScholarshipAndFunding_en = card.ScholarshipAndFunding_en,
                    ExchangePrograms_en = card.ExchangePrograms_en,
                    Labs_en = card.Labs_en,
                    StudentsLife_en = card.StudentsLife_en,
                    PaymentMethods_en = card.PaymentMethods_en,
                    Events_en = card.Events_en.Select(e => new
                    {
                        e.Id,
                        e.Url_en,
                        e.Title_en,
                        e.Time_en,
                        e.Link_en
                    }).ToList(),
                    Sections_en = card.Sections_en.Select(s => new
                    {
                        s.Id,
                        s.Title_en,
                        ProgramNames_en = s.ProgramNames_en.Select(pn => new
                        {
                            pn.Id,
                            pn.ProgramName_en
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
                    [HttpGet("search")]
public IActionResult GetUniCardByTitleAndProgramName([FromQuery] string title, [FromQuery] string programName)
{
    try
    {
        var result = dbContext.MyUniCardEn
            .Include(card => card.Sections_en)
                .ThenInclude(section => section.ProgramNames_en)
            .Where(card => card.Title_en == title &&
                           card.Sections_en.Any(section => section.ProgramNames_en
                                                        .Any(program => program.ProgramName_en == programName)))
            .Select(card => new
            {
                card.Title_en, // Only returning Title for UniCard
                Sections_en = card.Sections_en
                    .Where(section => section.ProgramNames_en.Any(program => program.ProgramName_en == programName)) // Filter sections to only include those with matching program names
                    .Select(section => new
                    {
                        section.Title_en, // Only returning Title for Sections
                        ProgramNames_en = section.ProgramNames_en
                            .Where(program => program.ProgramName_en == programName) // Select only the matching program name
                            .Select(program => new
                            {
                                program.ProgramName_en,
                                program.Jobs_en,
                                program.SwavlebisEna_en,
                                program.Kvalifikacia_en,
                                program.Dafinanseba_en,
                                program.KreditebisRaodenoba_en,
                                program.AdgilebisRaodenoba_en,
                                program.Fasi_en,
                                program.Kodi_en,
                                program.ProgramisAgwera_en,
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
        var result = dbContext.MyUniCardEn
            .Where(card => card.Title_en.Contains(title)) // Searching with partial match (use .Equals() if you want exact match)
            .Select(card => new
            {
                card.Id,
                card.Title_en,
                card.MainText_en,
                card.Url_en
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
[HttpGet("by-program-name")]
public IActionResult GetUniCardByProgramName(string programName)
{
    var uniCard = dbContext.MyUniCardEn
    .AsNoTracking() // Add this line
        .Include(card => card.Sections_en)
            .ThenInclude(section => section.ProgramNames_en)
        .Where(card => card.Sections_en
            .Any(section => section.ProgramNames_en
                .Any(pn => pn.ProgramName_en == programName)))
        .Select(card => new 
        {
            Id = card.Id,
            Url_en = card.Url_en,
            Title_en = card.Title_en,
            MainText_en = card.MainText_en,
            // Only select the matched programName
            ProgramNames_en = card.Sections_en.SelectMany(s => s.ProgramNames_en
                .Where(pn => pn.ProgramName_en == programName)
                .Select(pn => new { pn.Id, pn.ProgramName_en })).ToList()
        })
        .ToList();

    if (!uniCard.Any())
    {
        return NotFound();
    }

    return Ok(uniCard);
}

[HttpGet("searchById")]
public IActionResult GetUniCardByIdAndProgramName([FromQuery] int id, [FromQuery] string programName)
{
    try
    {
        var result = dbContext.MyUniCardEn
            .Include(card => card.Sections_en)
                .ThenInclude(section => section.ProgramNames_en)
            .Where(card => card.Id == id &&  // Filter by Id instead of Title
                           card.Sections_en.Any(section => section.ProgramNames_en
                                                        .Any(program => program.ProgramName_en == programName)))
            .Select(card => new
            {
                card.Id, // Returning Id for UniCard
                card.Title_en, // Optionally, return Title for UniCard
                Sections_en = card.Sections_en
                    .Where(section => section.ProgramNames_en.Any(program => program.ProgramName_en == programName)) // Filter sections to only include those with matching program names
                    .Select(section => new
                    {
                        section.Title_en, // Returning Title for Sections
                        ProgramNames_en = section.ProgramNames_en
                            .Where(program => program.ProgramName_en == programName) // Select only the matching program name
                            .Select(program => new
                            {
                                program.ProgramName_en,
                                program.Jobs_en,
                                program.SwavlebisEna_en,
                                program.Kvalifikacia_en,
                                program.Dafinanseba_en,
                                program.KreditebisRaodenoba_en,
                                program.AdgilebisRaodenoba_en,
                                program.Fasi_en,
                                program.Kodi_en,
                                program.ProgramisAgwera_en,
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
// [HttpGet("searchById")]
// public IActionResult GetUniCardByIdAndProgramName([FromQuery] int id, [FromQuery] string programName)
// {
//     try
//     {
//         var result = dbContext.MyUniCardEn
//             .Where(card => card.Id == id &&
//                            card.Sections_en.Any(section =>
//                                section.ProgramNames_en.Any(p => p.ProgramName_en == programName)))
//             .Select(card => new
//             {
//                 card.Id,
//                 card.Title_en,
//                 Sections_en = card.Sections_en
//                     .Where(section => section.ProgramNames_en.Any(program => program.ProgramName_en == programName))
//                     .Select(section => new
//                     {
//                         section.Title_en,
//                         ProgramNames_en = section.ProgramNames_en
//                             .Where(program => program.ProgramName_en == programName)
//                             .Select(program => new
//                             {
//                                 program.ProgramName_en,
//                                 program.Jobs_en,
//                                 program.SwavlebisEna_en,
//                                 program.Kvalifikacia_en,
//                                 program.Dafinanseba_en,
//                                 program.KreditebisRaodenoba_en,
//                                 program.AdgilebisRaodenoba_en,
//                                 program.Fasi_en,
//                                 program.Kodi_en,
//                                 program.ProgramisAgwera_en
//                             }).ToList()
//                     }).ToList()
//             })
//             .AsNoTracking() // Important for performance
//             .FirstOrDefault();

//         if (result == null)
//             return NotFound();

//         return Ok(result);
//     }
//     catch (Exception ex)
//     {
//         Console.Error.WriteLine($"Error: {ex.Message}");
//         return StatusCode(500, "Internal server error");
//     }
// }

[HttpGet("{uniCardId}/event/{eventId}")]
public IActionResult GetEventCardById(int uniCardId, int eventId)
{
    // Find the UniCard by ID including its events
    var uniCard = dbContext.MyUniCardEn
        .Include(card => card.Events_en)
        .FirstOrDefault(card => card.Id == uniCardId);

    if (uniCard == null)
    {
        return NotFound($"No UniCard found with ID {uniCardId}.");
    }

    // Find the event within the UniCard
    var eventCard = uniCard.Events_en.FirstOrDefault(e => e.Id == eventId);

    if (eventCard == null)
    {
        return NotFound($"No event found with ID {eventId} in UniCard {uniCardId}.");
    }

    return Ok(eventCard);
} 
        // [HttpDelete("{id}")]
        // public IActionResult DeleteUniCard(int id)
        // {
        //     var uniCard = dbContext.MyUniCardEn
        //         .Include(card => card.Event_en)
        //         .Include(card => card.Section_en)
        //             .ThenInclude(section => section.ProgramNames_en)
        //         .Include(card => card.Section2_en)
        //             .ThenInclude(section2 => section2.SavaldebuloSagnebi_en)
        //         .Include(card => card.ArchevitiSavaldebuloSaganebi_en)
        //             .ThenInclude(archeviti => archeviti.ArchevitiSavaldebuloSagani_en)
        //         .FirstOrDefault(card => card.Id == id);

        //     if (uniCard == null)
        //     {
        //         return NotFound();
        //     }

        //     dbContext.MyUniCardEn.Remove(uniCard);
        //     dbContext.SaveChanges();

        //     return NoContent();
        // }
    }
    
    
}



