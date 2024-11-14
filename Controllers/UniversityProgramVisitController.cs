using Microsoft.AspNetCore.Mvc;
using MyUni.Data; 
using MyUni.Models.Entities; 
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyUni.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UniversityProgramVisitController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Constructor to inject ApplicationDbContext
        public UniversityProgramVisitController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Endpoint to log a visit (with both UniversityName and ProgramName)
        [HttpPost("log")]
        public async Task<IActionResult> LogVisitAsync([FromBody] UniversityProgramVisitDto visitDto)
        {
            if (visitDto == null || string.IsNullOrEmpty(visitDto.UniversityName) || string.IsNullOrEmpty(visitDto.ProgramName))
            {
                return BadRequest("University name and program name are required.");
            }

            try
            {
                // Create a new UniversityVisit entity and log the visit
                var visit = new UniversityProgramVisit
                {
                    UniversityName = visitDto.UniversityName,
                    ProgramName = visitDto.ProgramName,
                    VisitDate = DateTime.UtcNow
                };

                // Add the new visit to the context and save the changes
                await _context.UniversityProgramVisits.AddAsync(visit);
                await _context.SaveChangesAsync();

                return Ok("Visit logged successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Endpoint to get the visit count for a specific university and program
        [HttpGet("count")]
        public async Task<IActionResult> GetVisitCountAsync([FromQuery] string universityName, [FromQuery] string programName)
        {
            if (string.IsNullOrEmpty(universityName) || string.IsNullOrEmpty(programName))
            {
                return BadRequest("University name and program name are required.");
            }

            try
            {
                // Get the visit count for the university and program combination
                var visitCount = await _context.UniversityProgramVisits
                    .Where(v => v.UniversityName == universityName && v.ProgramName == programName)
                    .CountAsync();

                return Ok(new { UniversityName = universityName, ProgramName = programName, VisitCount = visitCount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

    // DTO to represent the data coming from the client to log a visit
    public class UniversityProgramVisitDto
    {
        public string UniversityName { get; set; }
        public string ProgramName { get; set; }
    }
}
