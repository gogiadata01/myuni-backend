using Microsoft.AspNetCore.Mvc;
using MyUni.Data;  // For ApplicationDbContext
using MyUni.Models.Entities;  // For UniversityVisit
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyUni.Models;


namespace MyUni.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UniversityVisitController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Constructor to inject ApplicationDbContext
        public UniversityVisitController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Endpoint to log a visit (no UserId required)
        [HttpPost("log")]
        public async Task<IActionResult> LogVisitAsync([FromBody] UniversityVisitDto visitDto)
        {
            if (visitDto == null || string.IsNullOrEmpty(visitDto.UniversityName))
            {
                return BadRequest("University name is required.");
            }

            try
            {
                // Create a new UniversityVisit entity and log the visit
                var visit = new UniversityVisit
                {
                    UniversityName = visitDto.UniversityName,
                    VisitDate = DateTime.UtcNow
                };

                // Add the new visit to the context and save the changes
                await _context.UniversityVisits.AddAsync(visit);
                await _context.SaveChangesAsync();

                return Ok("Visit logged successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Endpoint to get the visit count for a specific university
        [HttpGet("count/{universityName}")]
        public async Task<IActionResult> GetVisitCountAsync(string universityName)
        {
            if (string.IsNullOrEmpty(universityName))
            {
                return BadRequest("University name is required.");
            }

            try
            {
                // Get the visit count for the university
                var visitCount = await _context.UniversityVisits
                    .Where(v => v.UniversityName == universityName)
                    .CountAsync();

                return Ok(new { UniversityName = universityName, VisitCount = visitCount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

    // DTO to represent the data coming from the client to log a visit (without UserId)
    public class UniversityVisitDto
    {
        public string UniversityName { get; set; }
    }
}
