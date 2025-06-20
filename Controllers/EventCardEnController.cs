using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyUni.Data;
using MyUni.Models.Entities;
using Newtonsoft.Json;
using MyUni.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static MyUni.Models.Entities.EventCard;

namespace MyUni.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Changed to include 'api' in the route
    public class EventCardEnController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public EventCardEnController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
[HttpGet]
public IActionResult GetAllEventCard()
{
    // Eagerly load the related 'Types' property and select necessary fields
    var allEventCards = dbContext.MyEventCardEn
        .Include(ec => ec.Types_en) // Include related 'Types' entity
        .Select(ec => new 
        {
            ec.Id,
            ec.Url_en,
            ec.Title_en,
            ec.Time_en,
            ec.Numbering_en,
            ec.Description_en,
            Types_en = ec.Types_en.Select(t => new
            {
                t.Type_en
            }).ToList() // Select both Id and Type properties from Types
        })
        .ToList();

    return Ok(allEventCards);
}
[HttpPost]
public IActionResult AddEventCard([FromBody] EventCardEnDto addEventCardDto)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    // Log received data
    Console.WriteLine(JsonConvert.SerializeObject(addEventCardDto, Formatting.Indented));

    // Map DTO to entity
    var eventCardEntity = new EventCardEn
    {
        Url_en = addEventCardDto.Url_en,
        Title_en = addEventCardDto.Title_en,
        Text_en = addEventCardDto.Text_en,
        Time_en = addEventCardDto.Time_en,
        Link_en = addEventCardDto.Link_en,
        isFeatured_en = addEventCardDto.isFeatured_en,
        Numbering_en = addEventCardDto.Numbering_en,
        Description_en = addEventCardDto.Description_en,
        saregistracioForma_en = addEventCardDto.saregistracioForma_en,
        Types_en = addEventCardDto.Types_en.Select(x => new EventTypeEn
        {
            Type_en = x.Type_en
        }).ToList()
    };

    dbContext.MyEventCardEn.Add(eventCardEntity);
    dbContext.SaveChanges();

    return Ok(eventCardEntity);
}

            [HttpDelete("{id}")]
        public IActionResult DeleteEventCard(int id)
        {
            // Find the event card by its ID
            var eventCard = dbContext.MyEventCardEn
                .Include(ec => ec.Types_en) // Include related 'Types' to ensure they are also deleted if necessary
                .FirstOrDefault(ec => ec.Id == id);

            // Check if the event card exists
            if (eventCard == null)
            {
                return NotFound(new { message = "Event card not found" });
            }

            // Remove the event card from the database
            dbContext.MyEventCardEn.Remove(eventCard);
            dbContext.SaveChanges();

            return Ok(new { message = "Event card deleted successfully" });
        }
        [HttpGet("{id}")]
public IActionResult GetEventCardById(int id)
{
    // Find the event card by its ID, including related 'Types'
    var eventCard = dbContext.MyEventCardEn
        .Where(ec => ec.Id == id)
        .Select(ec => new
        {
            ec.Title_en,
            ec.Text_en,
            ec.Time_en,
            ec.Link_en,
            ec.saregistracioForma_en // Include the new property
        })
        .FirstOrDefault();

    // Check if the event card exists
    if (eventCard == null)
    {
        return NotFound(new { message = "Event card not found" });
    }

    // Return the found event card
    return Ok(eventCard);
}
[HttpGet("home")]
public IActionResult GetEventCardForHome()
{
    try
    {
        var eventCards = dbContext.MyEventCardEn
            .Select(ec => new 
            {
                ec.Id,
                ec.Url_en,
                ec.Title_en,
                ec.Time_en,
                ec.isFeatured_en,
                ec.Numbering_en,
                ec.Description_en,
                Types_en = ec.Types_en.Select(t => new { t.Type_en }) // Select only the Type
            })
            .ToList();

        return Ok(eventCards);
    }
    catch (Exception ex)
    {
        // Log the exception (you might use a logging framework)
        Console.Error.WriteLine($"Error: {ex.Message}");
        return StatusCode(500, "Internal server error");
    }
}

    }
    
}
