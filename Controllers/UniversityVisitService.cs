using MyUni.Data;
using MyUni.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace MyUni.Services
{
    public class UniversityVisitService
    {
        private readonly ApplicationDbContext _context;

        public UniversityVisitService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Method to log a visit asynchronously (no UserId)
        public async Task LogVisitAsync(string universityName)
        {
            var visit = new UniversityVisit
            {
                UniversityName = universityName,
                VisitDate = DateTime.UtcNow
            };

            await _context.UniversityVisits.AddAsync(visit);
            await _context.SaveChangesAsync();
        }

        // Method to get the visit count for a specific university
        public async Task<int> GetVisitCountAsync(string universityName)
        {
            return await _context.UniversityVisits
                .CountAsync(v => v.UniversityName == universityName);
        }
    }
}
