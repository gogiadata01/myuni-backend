using Microsoft.AspNetCore.Mvc;
using MyUni.Controllers;  // Assuming you moved the logic to a service
using System;
using System.Threading.Tasks;
using MyUni.Models;
using MyUni.Models.Entities;

namespace MyUni.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UniversityVisitController : ControllerBase
    {
        private readonly UniversityVisitService _universityVisitService;

        public UniversityVisitController(UniversityVisitService universityVisitService)
        {
            _universityVisitService = universityVisitService;
        }

        // Endpoint to log a visit (no UserId required)
        [HttpPost("log")]
        public async Task<IActionResult> LogVisitAsync([FromBody] UniversityVisit visitDto)
        {
            if (visitDto == null || string.IsNullOrEmpty(visitDto.UniversityName))
            {
                return BadRequest("University name is required.");
            }

            try
            {
                // Call the service method to log the visit
                await _universityVisitService.LogVisitAsync(visitDto.UniversityName);
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
                // Get the visit count from the service
                var visitCount = await _universityVisitService.GetVisitCountAsync(universityName);
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
