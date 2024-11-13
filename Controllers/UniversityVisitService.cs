// Services/UniversityVisitService.cs
using System;
using System.Threading.Tasks;
using MyUni.Data;
using MyUni.Models;
using Microsoft.EntityFrameworkCore;

namespace MyUni.Controllers
{
    public class UniversityVisitService
    {
        private readonly ApplicationDbContext _context;

        public UniversityVisitService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogVisitAsync(string universityName, string userId)
        {
            var visit = new UniversityVisit
            {
                UniversityName = universityName,
                UserId = userId,
                VisitDate = DateTime.UtcNow
            };

            await _context.UniversityVisits.AddAsync(visit);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetVisitCountAsync(string universityName)
        {
            return await _context.UniversityVisits
                .CountAsync(v => v.UniversityName == universityName);
        }
    }
}
