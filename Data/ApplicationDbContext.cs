using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyUni.Models.Entities;

namespace MyUni.Data
{
    public class ApplicationDbContext:DbContext
    {
            
        public ApplicationDbContext(DbContextOptions options) : base(options) { 
        }
        public DbSet<UniCard> MyUniCard { get; set; }

        public DbSet<EventCard> MyEventCard { get; set; }

        public DbSet<ProgramCard> MyprogramCard { get; set; }


        public DbSet<User> MyUser { get; set; }

        public DbSet<Quiz> MyQuiz { get; set; }

        public DbSet<QuizArchive> MyQuizArchive { get; set; }
        
        public DbSet<UniversityVisit> UniversityVisits { get; set; }  // UniversityVisit should be recognized now
        
        public DbSet<UniversityProgramVisit> UniversityProgramVisits { get; set; }  // UniversityVisit should be recognized now

    }
}
