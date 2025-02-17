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
    public class EventCardController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public EventCardController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // GET: api/EventCard
[HttpGet]
public IActionResult GetAllEventCard()
{
    // Eagerly load the related 'Types' property and select necessary fields
    var allEventCards = dbContext.MyEventCard
        .Include(ec => ec.Types) // Include related 'Types' entity
        .Select(ec => new 
        {
            ec.Id,
            ec.Url,
            ec.Title,
            ec.Time,
            ec.Numbering,
            Types = ec.Types.Select(t => new
            {
                t.Type
            }).ToList() // Select both Id and Type properties from Types
        })
        .ToList();

    return Ok(allEventCards);
}


        // POST: api/EventCard
       
        [HttpPost]
        public IActionResult AddEventCard([FromBody] EventCardDto addEventCardDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Log received data
            Console.WriteLine(JsonConvert.SerializeObject(addEventCardDto, Formatting.Indented));

            // Map DTO to entity
            var eventCardEntity = new EventCard
            {
                Url = addEventCardDto.Url,
                Title = addEventCardDto.Title,
                Text = addEventCardDto.Text,
                Time = addEventCardDto.Time,
                Link = addEventCardDto.Link,                
                isFeatured = addEventCardDto.isFeatured,
                Numbering = addEventCardDto.Numbering,
        	saregistracioForma = addEventCardDto.saregistracioForma, // Ensure this field is mapped
                Types = addEventCardDto.Types.Select(x => new EventType
                {
                    Type = x.Type
                }).ToList() // Convert to List
            };

            // Save entity to database
            dbContext.MyEventCard.Add(eventCardEntity);
            dbContext.SaveChanges();

            return Ok(eventCardEntity);
        }

        // DELETE: api/EventCard/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteEventCard(int id)
        {
            // Find the event card by its ID
            var eventCard = dbContext.MyEventCard
                .Include(ec => ec.Types) // Include related 'Types' to ensure they are also deleted if necessary
                .FirstOrDefault(ec => ec.Id == id);

            // Check if the event card exists
            if (eventCard == null)
            {
                return NotFound(new { message = "Event card not found" });
            }

            // Remove the event card from the database
            dbContext.MyEventCard.Remove(eventCard);
            dbContext.SaveChanges();

            return Ok(new { message = "Event card deleted successfully" });
        }

        // GET: api/EventCard/{id}// GET: api/EventCard/{id}
[HttpGet("{id}")]
public IActionResult GetEventCardById(int id)
{
    // Find the event card by its ID, including related 'Types'
    var eventCard = dbContext.MyEventCard
        .Where(ec => ec.Id == id)
        .Select(ec => new
        {
            ec.Title,
            ec.Text,
            ec.Time,
            ec.Link,
            ec.saregistracioForma // Include the new property
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
// GET: api/EventCard/home
[HttpGet("home")]
public IActionResult GetEventCardForHome()
{
    try
    {
        var eventCards = dbContext.MyEventCard
            .Select(ec => new 
            {
                ec.Id,
                ec.Url,
                ec.Title,
                ec.Time,
                ec.isFeatured,
                Types = ec.Types.Select(t => new { t.Type }) // Select only the Type
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

// PUT: api/EventCard/{id}/isFeatured
[HttpPut("{id}/isFeatured")]
public IActionResult UpdateIsFeatured(int id, [FromBody] bool isFeatured)
{
    // Find the existing event card by its ID
    var existingEventCard = dbContext.MyEventCard.FirstOrDefault(ec => ec.Id == id);

    // Check if the event card exists
    if (existingEventCard == null)
    {
        return NotFound(new { message = "Event card not found" });
    }

    // Update the isFeatured property
    existingEventCard.isFeatured = isFeatured;

    // Save the changes to the database
    dbContext.SaveChanges();

    // Return the updated event card
    return Ok(existingEventCard);
}


        // PUT: api/EventCard/{id}

        [HttpPut("{id}")]
        public IActionResult UpdateEventCard(int id, [FromBody] EventCardDto updateEventCardDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find the existing event card by its ID
            var existingEventCard = dbContext.MyEventCard
                .Include(ec => ec.Types) // Include related 'Types' to update them as well
                .FirstOrDefault(ec => ec.Id == id);

            // Check if the event card exists
            if (existingEventCard == null)
            {
                return NotFound(new { message = "Event card not found" });
            }

            // Update the event card properties
            existingEventCard.Url = updateEventCardDto.Url;
            existingEventCard.Title = updateEventCardDto.Title;
            existingEventCard.Text = updateEventCardDto.Text;
            existingEventCard.Time = updateEventCardDto.Time;
            existingEventCard.Link = updateEventCardDto.Link;
            existingEventCard.isFeatured = updateEventCardDto.isFeatured;
	    existingEventCard.saregistracioForma = updateEventCardDto.saregistracioForma;
            // Clear the existing event types and update with the new ones
            existingEventCard.Types.Clear();
            existingEventCard.Types = updateEventCardDto.Types.Select(x => new EventType
            {
                Type = x.Type
            }).ToList();

            // Save the changes to the database
            dbContext.SaveChanges();

            // Return the updated event card
            return Ok(existingEventCard);
        }
    }
    // PUT: api/EventCard/{id}/numbering
[HttpPut("{id}/numbering")]
public IActionResult UpdateNumberingWithEventId(int id, [FromBody] int numbering)
{
    // Find the existing event card by its ID
    var existingEventCard = dbContext.MyEventCard.FirstOrDefault(ec => ec.Id == id);

    // Check if the event card exists
    if (existingEventCard == null)
    {
        return NotFound(new { message = "Event card not found" });
    }

    // Update the Numbering property
    existingEventCard.Numbering = numbering;

    // Save the changes to the database
    dbContext.SaveChanges();

    // Return the updated event card
    return Ok(new { message = "Numbering updated successfully", existingEventCard });
}

}
