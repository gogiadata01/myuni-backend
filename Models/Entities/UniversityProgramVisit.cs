namespace MyUni.Models.Entities
{
    public class UniversityProgramVisit
    {
        public int Id { get; set; }
        public string UniversityName { get; set; }
        public string ProgramName { get; set; }
        public DateTime? VisitDate { get; set; } // Nullable DateTime
    }
    }